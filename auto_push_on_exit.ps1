<#
.SYNOPSIS
    Automated Git Auto-Push Watcher for Visual Studio & Directory Changes
.DESCRIPTION
    1. Monitors Visual Studio (devenv.exe or Code.exe). When closed, commits and pushes all work.
    2. Also monitors the folder: whenever any new file or folder is added (even without VS), 
       it commits each file/folder individually with its own specific message and pushes to GitHub.
#>

param(
    [switch]$Once,
    [string]$repoPath = "D:\All\.net",
    [string]$branch = "main",
    [string[]]$processNames = @("devenv", "Code"),
    [int]$checkIntervalSeconds = 5
)

# Ensure only one instance of this specific watcher runs at a time
if (-not $Once) {
    $isNew = $false
    $mutex = New-Object System.Threading.Mutex($true, "GitAutoPush_NetWatcher", [ref]$isNew)
    if (-not $isNew) {
        Write-Host "Another instance of .net watcher is already running."
        return
    }
}

function Show-Notification {
    param (
        [string]$Title,
        [string]$Message
    )
    try {
        [void][Windows.UI.Notifications.ToastNotificationManager, Windows.UI.Notifications, ContentType = WindowsRuntime]
        [void][Windows.Data.Xml.Dom.XmlDocument, Windows.Data.Xml.Dom.XmlDocument, ContentType = WindowsRuntime]
        $template = @"
<toast>
    <visual>
        <binding template="ToastGeneric">
            <text>$Title</text>
            <text>$Message</text>
        </binding>
    </visual>
</toast>
"@
        $xml = New-Object Windows.Data.Xml.Dom.XmlDocument
        $xml.LoadXml($template)
        $toast = [Windows.UI.Notifications.ToastNotification]::new($xml)
        [Windows.UI.Notifications.ToastNotificationManager]::CreateToastNotifier("Git Auto-Push").Show($toast)
    } catch {
        Write-Host "[$Title] $Message" -ForegroundColor Magenta
    }
}

function Get-WorkHintFromDiff {
    param (
        [string]$targetRepo,
        [string[]]$files
    )

    $hints = [System.Collections.Generic.List[string]]::new()

    foreach ($file in $files) {
        if (-not ($file.EndsWith(".cs") -or $file.EndsWith(".sql") -or $file.EndsWith(".md"))) { continue }

        $diffLines = git -C $targetRepo diff -U0 -- $file 2>$null | Where-Object { $_ -match '^\+[^\+]' }
        if (-not $diffLines) {
            $fullPath = Join-Path $targetRepo $file
            if (Test-Path $fullPath) {
                $diffLines = Get-Content $fullPath -TotalCount 25 | ForEach-Object { "+ $_" }
            }
        }

        foreach ($line in $diffLines) {
            $trimmed = $line.Substring(1).Trim()
            if ([string]::IsNullOrWhiteSpace($trimmed)) { continue }

            # Match method declaration
            if ($trimmed -match '(?:public|private|protected|internal|static|async)\s+[\w<>,\[\]]+\s+([A-Z]\w+)\s*\(') {
                $method = $Matches[1]
                if ($method -notmatch '^(Main|ToString|Dispose)$') {
                    $hints.Add("add $method method")
                    break
                }
            }
            # Match class / interface
            if ($trimmed -match '(?:class|interface|record|struct|enum)\s+([A-Z]\w+)') {
                $hints.Add("create $($Matches[1])")
                break
            }
            # Match comments
            if ($trimmed -match '^//\s*([A-Za-z0-9\s_\-]{4,35})') {
                $hints.Add($Matches[1].Trim())
                break
            }
            # Match descriptive strings
            if ($trimmed -match '"(?:\d+\.\s*)?([A-Za-z\s]{4,30})"') {
                $actionText = $Matches[1].Trim()
                if ($actionText -notmatch '^(Enter|Invalid|Error|Success|Exit|Select)$') {
                    $hints.Add($actionText)
                    break
                }
            }
        }

        if ($hints.Count -ge 1) { break }
    }

    if ($hints.Count -gt 0) {
        return [string]($hints | Select-Object -Unique -First 1)
    }
    return ""
}

function Push-Changes {
    Set-Location -Path $repoPath

    $statusOutput = git status --porcelain
    if ([string]::IsNullOrWhiteSpace($statusOutput)) {
        return
    }

    $statusLines = $statusOutput -split "`r?`n" | Where-Object { -not [string]::IsNullOrWhiteSpace($_) }

    # Group changes by top-level item (Folder name or Root file name)
    $groups = @{}

    foreach ($line in $statusLines) {
        $code = $line.Substring(0, 2).Trim()
        $rawPath = $line.Substring(2).Trim().Trim('"')

        # Filter out temporary Visual Studio build/cache files
        if ($rawPath -match '[\\/](\.vs|obj|bin)[\\/]' -or $rawPath -match '^\.vs[\\/]') {
            continue
        }

        $normPath = $rawPath.Replace('/', '\')
        $parts = $normPath.Split('\')
        $topLevelItem = $parts[0]

        if (-not $groups.ContainsKey($topLevelItem)) {
            $groups[$topLevelItem] = [System.Collections.Generic.List[PSCustomObject]]::new()
        }

        $groups[$topLevelItem].Add([PSCustomObject]@{
            Code     = $code
            Path     = $rawPath
        })
    }

    if ($groups.Keys.Count -eq 0) {
        return
    }

    Write-Host "[$(Get-Date -Format 'HH:mm:ss')] Changes detected across $($groups.Keys.Count) folder(s)/file(s)." -ForegroundColor Yellow
    $committedItems = [System.Collections.Generic.List[string]]::new()

    foreach ($topItem in $groups.Keys) {
        $items = $groups[$topItem]

        # Stage only this specific folder or file
        git add -- "$topItem"

        # Check what was staged for this item
        $staged = git status --porcelain -- "$topItem"
        if ([string]::IsNullOrWhiteSpace($staged)) {
            continue
        }

        $added = [System.Collections.Generic.List[string]]::new()
        $modified = [System.Collections.Generic.List[string]]::new()
        $deleted = [System.Collections.Generic.List[string]]::new()

        foreach ($it in $items) {
            $fname = [System.IO.Path]::GetFileName($it.Path)
            if ($it.Code -eq '??' -or $it.Code -eq 'A') {
                $added.Add($fname)
            } elseif ($it.Code -eq 'D') {
                $deleted.Add($fname)
            } else {
                $modified.Add($fname)
            }
        }

        $isFolder = (Test-Path -Path (Join-Path $repoPath $topItem) -PathType Container)
        $allFiles = ($items | ForEach-Object { $_.Path })
        $workHint = Get-WorkHintFromDiff -targetRepo $repoPath -files $allFiles

        $subject = ""

        if (-not $isFolder) {
            # Root file (e.g. C#Concepts.md, notes.txt)
            if ($added.Count -gt 0) {
                $subject = "Add $topItem"
            } elseif ($deleted.Count -gt 0) {
                $subject = "Delete $topItem"
            } else {
                $subject = "Update $topItem"
            }
        } else {
            # Folder (e.g. MedicalDepartment, Day 6, ConsoleApp1)
            $folderName = $topItem

            # If brand new folder with multiple files
            if ($added.Count -gt 0 -and $modified.Count -eq 0 -and $deleted.Count -eq 0 -and $added.Count -ge 3) {
                $subject = "$($folderName): Add new $folderName project ($($added.Count) files)"
            } else {
                $actions = [System.Collections.Generic.List[string]]::new()

                if ($modified.Count -gt 0) {
                    if ($modified.Count -le 2) {
                        $actions.Add("Update $(($modified | Select-Object -Unique) -join ', ')")
                    } else {
                        $actions.Add("Update $($modified.Count) files")
                    }
                }
                if ($added.Count -gt 0) {
                    if ($added.Count -le 2) {
                        $actions.Add("Add $(($added | Select-Object -Unique) -join ', ')")
                    } else {
                        $actions.Add("Add $($added.Count) new files")
                    }
                }
                if ($deleted.Count -gt 0) {
                    if ($deleted.Count -le 2) {
                        $actions.Add("Delete $(($deleted | Select-Object -Unique) -join ', ')")
                    } else {
                        $actions.Add("Delete $($deleted.Count) files")
                    }
                }

                $actString = $actions -join ' and '
                if (-not [string]::IsNullOrWhiteSpace($workHint)) {
                    $subject = "$($folderName): $actString ($workHint)"
                } else {
                    $subject = "$($folderName): $actString"
                }
            }
        }

        if ([string]::IsNullOrWhiteSpace($subject)) {
            $subject = "$($topItem): Update changes"
        }

        # Append push Date and Time as requested
        $timestamp = Get-Date -Format 'yyyy-MM-dd HH:mm:ss'
        $subjectWithTime = "$subject - $timestamp"

        Write-Host "Committing [$topItem] -> $subjectWithTime" -ForegroundColor Cyan
        git commit -m "$subjectWithTime"
        $committedItems.Add($subjectWithTime)
    }

    if ($committedItems.Count -gt 0) {
        Write-Host "Pushing all $($committedItems.Count) commit(s) to origin $branch..." -ForegroundColor Gray
        $pushOutput = git push origin $branch 2>&1

        if ($LASTEXITCODE -eq 0) {
            Write-Host "[$(Get-Date -Format 'HH:mm:ss')] Successfully pushed to GitHub!" -ForegroundColor Green
            Show-Notification -Title "Git Push Success" -Message ($committedItems -join "`n")
        } else {
            Write-Host "[$(Get-Date -Format 'HH:mm:ss')] Push failed: $pushOutput" -ForegroundColor Red
            Show-Notification -Title "Git Push Failed" -Message "Check terminal for details."
        }
    }
}

# If run with -Once, execute push check once and exit
if ($Once) {
    Push-Changes
    return
}

# Continuous Watcher Mode
Write-Host "=======================================================" -ForegroundColor Cyan
Write-Host " Git Auto-Push Monitor (VS Close + Real-Time New Files)" -ForegroundColor Green
Write-Host " Watching Repo : $repoPath" -ForegroundColor Yellow
Write-Host " Target Branch : $branch" -ForegroundColor Yellow
Write-Host " Monitoring    : $($processNames -join ', ') & Folder additions" -ForegroundColor Yellow
Write-Host "=======================================================" -ForegroundColor Cyan

$wasRunning = $false

while ($true) {
    # Check if Visual Studio is running
    $runningProcesses = Get-Process -Name $processNames -ErrorAction SilentlyContinue

    if ($runningProcesses) {
        if (-not $wasRunning) {
            Write-Host "[$(Get-Date -Format 'HH:mm:ss')] Visual Studio is now RUNNING. Monitoring for changes..." -ForegroundColor Cyan
            $wasRunning = $true
        }
    } else {
        if ($wasRunning) {
            # Visual Studio was closed
            Write-Host "[$(Get-Date -Format 'HH:mm:ss')] Visual Studio was CLOSED. Analyzing and pushing changes..." -ForegroundColor Yellow
            $wasRunning = $false
            Push-Changes
        } else {
            # Visual Studio is NOT running, but user may have added new files/folders directly
            $statusCheck = git -C $repoPath status --porcelain
            if (-not [string]::IsNullOrWhiteSpace($statusCheck)) {
                # Ensure files are completely copied before pushing (settle time 2s)
                Start-Sleep -Seconds 2
                Write-Host "[$(Get-Date -Format 'HH:mm:ss')] New files/folders detected in repository. Pushing..." -ForegroundColor Yellow
                Push-Changes
            }
        }
    }

    Start-Sleep -Seconds $checkIntervalSeconds
}
