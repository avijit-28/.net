<#
.SYNOPSIS
    Automated Git Auto-Push Watcher for Visual Studio
.DESCRIPTION
    Monitors Visual Studio (devenv.exe or Code.exe).
    When Visual Studio is closed, checks if any files/folders were added or modified in D:\All\.net,
    analyzes the changes to identify the exact files, project, and work done (methods, tasks, or comments),
    and automatically commits and pushes them to GitHub on the 'main' branch.
#>

param(
    [switch]$Once,
    [string]$repoPath = "D:\All\.net",
    [string]$branch = "main",
    [string[]]$processNames = @("devenv", "Code"),
    [int]$checkIntervalSeconds = 5
)

# Ensure only one instance of the watcher runs at a time
if (-not $Once) {
    $currentPid = $PID
    Get-CimInstance Win32_Process | Where-Object { 
        $_.CommandLine -like "*auto_push_on_exit.ps1*" -and $_.ProcessId -ne $currentPid 
    } | ForEach-Object {
        try { Stop-Process -Id $_.ProcessId -Force -ErrorAction SilentlyContinue } catch {}
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

        # Get diff of added lines only
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

            # 1. Match C# method declaration: e.g. void DeleteDoctor(...)
            if ($trimmed -match '(?:public|private|protected|internal|static|async)\s+[\w<>,\[\]]+\s+([A-Z]\w+)\s*\(') {
                $method = $Matches[1]
                if ($method -notmatch '^(Main|ToString|Dispose)$') {
                    $hints.Add("add $method method")
                    break
                }
            }
            # 2. Match C# class or interface
            if ($trimmed -match '(?:class|interface|record|struct|enum)\s+([A-Z]\w+)') {
                $hints.Add("create $($Matches[1])")
                break
            }
            # 3. Match comments: e.g. // Delete Doctors or // Update method
            if ($trimmed -match '^//\s*([A-Za-z0-9\s_\-]{4,35})') {
                $hints.Add($Matches[1].Trim())
                break
            }
            # 4. Match Menu/Action strings: e.g. "4. Delete Doctors Details"
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
        return ($hints | Select-Object -Unique -First 1)[0]
    }
    return ""
}

function Generate-SmartCommitMessage {
    param (
        [string[]]$statusLines,
        [string]$targetRepo
    )

    $added = [System.Collections.Generic.List[string]]::new()
    $modified = [System.Collections.Generic.List[string]]::new()
    $deleted = [System.Collections.Generic.List[string]]::new()
    $renamed = [System.Collections.Generic.List[string]]::new()
    $projects = [System.Collections.Generic.HashSet[string]]::new()

    foreach ($line in $statusLines) {
        if ([string]::IsNullOrWhiteSpace($line)) { continue }
        
        $code = $line.Substring(0, 2).Trim()
        $rawPath = $line.Substring(2).Trim().Trim('"')

        # Filter out temporary / build files if any slipped through
        if ($rawPath -match '[\\/](\.vs|obj|bin)[\\/]' -or $rawPath -match '^\.vs[\\/]') {
            continue
        }

        # Identify project/module name if inside a subfolder
        $parts = $rawPath.Split('\/')
        if ($parts.Length -gt 1) {
            $null = $projects.Add($parts[0])
        }

        if ($code -eq '??' -or $code -eq 'A') {
            $added.Add($rawPath)
        } elseif ($code -eq 'D') {
            $deleted.Add($rawPath)
        } elseif ($code -eq 'R') {
            $renamed.Add($rawPath)
        } else {
            $modified.Add($rawPath)
        }
    }

    $totalCount = $added.Count + $modified.Count + $deleted.Count + $renamed.Count
    if ($totalCount -eq 0) {
        return $null
    }

    # Helper function to get simple base filename
    $getFileName = { param([string]$p) [System.IO.Path]::GetFileName($p) }

    # Extract work description hint from code changes
    $allTouchFiles = @($modified) + @($added)
    $workHint = Get-WorkHintFromDiff -targetRepo $targetRepo -files $allTouchFiles

    # Build concise subject title
    $actionParts = [System.Collections.Generic.List[string]]::new()

    if ($modified.Count -gt 0) {
        if ($modified.Count -le 2) {
            $names = ($modified | ForEach-Object { & $getFileName $_ }) -join ', '
            $actionParts.Add("Update $names")
        } else {
            $actionParts.Add("Update $($modified.Count) files")
        }
    }

    if ($added.Count -gt 0) {
        if ($added.Count -le 2) {
            $names = ($added | ForEach-Object { & $getFileName $_ }) -join ', '
            $actionParts.Add("Add $names")
        } else {
            $actionParts.Add("Add $($added.Count) new files")
        }
    }

    if ($deleted.Count -gt 0) {
        if ($deleted.Count -le 2) {
            $names = ($deleted | ForEach-Object { & $getFileName $_ }) -join ', '
            $actionParts.Add("Delete $names")
        } else {
            $actionParts.Add("Delete $($deleted.Count) files")
        }
    }

    if ($renamed.Count -gt 0) {
        $actionParts.Add("Rename $($renamed.Count) files")
    }

    # Add project folder context
    $projContext = ""
    if ($projects.Count -eq 1) {
        $singleProj = [System.Linq.Enumerable]::First($projects)
        $projContext = " in $singleProj"
    } elseif ($projects.Count -gt 1) {
        $firstFew = ($projects | Select-Object -First 2) -join ', '
        $projContext = " in $firstFew"
    }

    # Integrate work hint into title if present
    $hintContext = ""
    if (-not [string]::IsNullOrWhiteSpace($workHint)) {
        $hintContext = " ($workHint)"
    }

    $subject = ($actionParts -join ' and ') + $hintContext + $projContext

    # Fallback or length guard
    if ([string]::IsNullOrWhiteSpace($subject)) {
        $subject = "Auto-update ($totalCount files changed)"
    } elseif ($subject.Length -gt 72) {
        $subject = $subject.Substring(0, 69) + "..."
    }

    # Build detailed body breakdown
    $bodyLines = [System.Collections.Generic.List[string]]::new()
    $bodyLines.Add("Visual Studio Work Summary:")
    $bodyLines.Add("Timestamp: $(Get-Date -Format 'yyyy-MM-dd HH:mm:ss')")
    if (-not [string]::IsNullOrWhiteSpace($workHint)) {
        $bodyLines.Add("Task/Feature: $workHint")
    }
    $bodyLines.Add("")

    if ($modified.Count -gt 0) {
        $bodyLines.Add("Modified Files ($($modified.Count)):")
        foreach ($f in $modified) { $bodyLines.Add("  - $f") }
        $bodyLines.Add("")
    }

    if ($added.Count -gt 0) {
        $bodyLines.Add("New Files Added ($($added.Count)):")
        foreach ($f in $added) { $bodyLines.Add("  - $f") }
        $bodyLines.Add("")
    }

    if ($deleted.Count -gt 0) {
        $bodyLines.Add("Deleted Files ($($deleted.Count)):")
        foreach ($f in $deleted) { $bodyLines.Add("  - $f") }
        $bodyLines.Add("")
    }

    if ($renamed.Count -gt 0) {
        $bodyLines.Add("Renamed Files ($($renamed.Count)):")
        foreach ($f in $renamed) { $bodyLines.Add("  - $f") }
        $bodyLines.Add("")
    }

    $body = ($bodyLines -join "`n").TrimEnd()

    return @{
        Subject = $subject
        Body    = $body
    }
}

function Push-Changes {
    Set-Location -Path $repoPath

    $statusOutput = git status --porcelain
    if (-not [string]::IsNullOrWhiteSpace($statusOutput)) {
        $statusLines = $statusOutput -split "`r?`n" | Where-Object { -not [string]::IsNullOrWhiteSpace($_) }
        
        $commitInfo = Generate-SmartCommitMessage -statusLines $statusLines -targetRepo $repoPath
        if ($null -eq $commitInfo) {
            Write-Host "[$(Get-Date -Format 'HH:mm:ss')] No trackable code changes detected." -ForegroundColor DarkGray
            return
        }

        $subject = $commitInfo.Subject
        $body = $commitInfo.Body

        Write-Host "[$(Get-Date -Format 'HH:mm:ss')] Commit Title: $subject" -ForegroundColor Cyan
        Write-Host "Staging files..." -ForegroundColor Gray
        git add -A

        Write-Host "Committing..." -ForegroundColor Gray
        git commit -m "$subject" -m "$body"

        Write-Host "Pushing to origin $branch..." -ForegroundColor Gray
        $pushOutput = git push origin $branch 2>&1

        if ($LASTEXITCODE -eq 0) {
            Write-Host "[$(Get-Date -Format 'HH:mm:ss')] Successfully pushed to GitHub!" -ForegroundColor Green
            Show-Notification -Title "Git Push Success" -Message "$subject"
        } else {
            Write-Host "[$(Get-Date -Format 'HH:mm:ss')] Push failed: $pushOutput" -ForegroundColor Red
            Show-Notification -Title "Git Push Failed" -Message "Check terminal for details."
        }
    } else {
        Write-Host "[$(Get-Date -Format 'HH:mm:ss')] Visual Studio closed, but no file changes were detected." -ForegroundColor DarkGray
    }
}

# If run with -Once, execute push check once and exit
if ($Once) {
    Push-Changes
    return
}

# Continuous Watcher Mode
Write-Host "=======================================================" -ForegroundColor Cyan
Write-Host " Git Auto-Push Monitor for Visual Studio (Smart Commit)" -ForegroundColor Green
Write-Host " Watching Repo : $repoPath" -ForegroundColor Yellow
Write-Host " Target Branch : $branch" -ForegroundColor Yellow
Write-Host " Monitoring    : $($processNames -join ', ')" -ForegroundColor Yellow
Write-Host "=======================================================" -ForegroundColor Cyan

$wasRunning = $false

while ($true) {
    # Check if any monitored process is running
    $runningProcesses = Get-Process -Name $processNames -ErrorAction SilentlyContinue

    if ($runningProcesses) {
        if (-not $wasRunning) {
            Write-Host "[$(Get-Date -Format 'HH:mm:ss')] Visual Studio is now RUNNING. Monitoring for changes..." -ForegroundColor Cyan
            $wasRunning = $true
        }
    } else {
        if ($wasRunning) {
            Write-Host "[$(Get-Date -Format 'HH:mm:ss')] Visual Studio was CLOSED. Analyzing and pushing changes..." -ForegroundColor Yellow
            $wasRunning = $false
            Push-Changes
        }
    }

    Start-Sleep -Seconds $checkIntervalSeconds
}
