<#
================================================================================
 Auto Git Push Script for D:\avi\C#
 Target Branch: mcc_.net (Remote: origin)
 Repository   : https://github.com/avijit-28/.net
 
 Commit Message Rules:
 - When a folder is uploaded/created: "<folder_name> folder dd/MM/yyyy HH:mm"
   (e.g., "avijit folder 08/10/2026 12:27")
 - When file(s) are uploaded/modified: "<file_name> dd/MM/yyyy HH:mm"
   (e.g., "class 10 08/10/2026 12:27", "12 marksheet 08/10/2026 12:27")
================================================================================
#>

[CmdletBinding()]
param(
    [string]$RepoPath = "D:\avi\C#",
    [string]$Branch = "mcc_.net",
    [string]$Remote = "origin",
    [int]$DebounceSeconds = 3
)

# Set working directory to the repo folder
Set-Location -LiteralPath $RepoPath

# 1. Verification of Git repository
if (-not (Test-Path -LiteralPath (Join-Path $RepoPath ".git"))) {
    Write-Host "[ERROR] '$RepoPath' is not a Git repository!" -ForegroundColor Red
    exit 1
}

# 2. Check and switch to the target branch 'mcc_.net'
$currentBranch = (git branch --show-current).Trim()
if ($currentBranch -ne $Branch) {
    Write-Host "[INFO] Currently on branch '$currentBranch'. Switching to '$Branch'..." -ForegroundColor Cyan
    git checkout $Branch
    if ($LASTEXITCODE -ne 0) {
        Write-Host "[ERROR] Failed to switch to branch '$Branch'. Please check your repository branches." -ForegroundColor Red
        exit 1
    }
}

Write-Host "==========================================================" -ForegroundColor Green
Write-Host "         AUTO GIT PUSH SERVICE RUNNING (.NET)            " -ForegroundColor Green
Write-Host "==========================================================" -ForegroundColor Green
Write-Host " Monitoring Path : $RepoPath" -ForegroundColor White
Write-Host " Target Branch   : $Branch (not main)" -ForegroundColor Cyan
Write-Host " Remote Target   : $Remote" -ForegroundColor White
Write-Host " Debounce Time   : $DebounceSeconds seconds" -ForegroundColor White
Write-Host " Started At      : $([DateTime]::Now.ToString('dd\/MM\/yyyy HH:mm'))" -ForegroundColor Gray
Write-Host "==========================================================" -ForegroundColor Green
Write-Host " Press [Ctrl + C] in this window at any time to stop." -ForegroundColor Yellow
Write-Host ""

# Function to commit and push changes
function Invoke-AutoPush {
    # Step A: Check for empty folders and add .gitkeep so Git can track them
    $emptyDirs = Get-ChildItem -LiteralPath $RepoPath -Directory -Recurse | Where-Object { 
        $_.FullName -notmatch '[\\/](\.git|\.vs)([\\/]|$)' -and (Get-ChildItem -LiteralPath $_.FullName -Force).Count -eq 0 
    }
    foreach ($dir in $emptyDirs) {
        $keepFile = Join-Path $dir.FullName ".gitkeep"
        if (-not (Test-Path -LiteralPath $keepFile)) {
            New-Item -Path $keepFile -ItemType File -Force | Out-Null
        }
    }

    # Step B: Check git status
    $statusOutput = git status --porcelain
    if ([string]::IsNullOrWhiteSpace($statusOutput)) {
        return
    }

    # Format timestamp as requested: dd/MM/yyyy HH:mm (e.g., 08/10/2026 12:27)
    $now = [DateTime]::Now.ToString('dd\/MM\/yyyy HH:mm')

    # Step C: Parse all changed items from status
    $statusLines = $statusOutput -split "`r?`n" | Where-Object { -not [string]::IsNullOrWhiteSpace($_) }
    $changedItems = @()

    foreach ($line in $statusLines) {
        if ($line.Length -ge 3) {
            $rawPath = $line.Substring(3).Trim()
            # Handle renames e.g. "old.cs -> new.cs"
            if ($rawPath -match "->") {
                $rawPath = ($rawPath -split "->")[-1].Trim()
            }
            # Remove surrounding quotes and normalize slashes
            $cleanPath = $rawPath.Trim('"').Replace('\', '/')

            # Ignore internal .git and .vs paths
            if ($cleanPath -match '(^|[\\/])(\.git|\.vs)([\\/]|$)') {
                continue
            }

            $changedItems += $cleanPath
        }
    }

    $changedItems = @($changedItems | Select-Object -Unique)
    if ($changedItems.Count -eq 0) {
        return
    }

    # Check if HEAD commit exists
    $hasHead = (git rev-parse --verify HEAD 2>$null)

    # Distinguish newly uploaded/created folders vs individual files
    $newFoldersToCommit = [System.Collections.Generic.Dictionary[string, string]]::new()
    $filesToCommit = @()

    foreach ($item in $changedItems) {
        $parts = $item -split '/'
        if ($parts.Count -gt 1) {
            $foundNewFolder = $false
            if ($hasHead) {
                for ($i = 0; $i -lt $parts.Count - 1; $i++) {
                    $folderRelPath = ($parts[0..$i]) -join '/'
                    $folderName = $parts[$i]
                    $inTree = (git ls-tree HEAD "$folderRelPath" 2>$null)
                    if ([string]::IsNullOrWhiteSpace($inTree)) {
                        # Folder did not exist previously in HEAD -> upload folder
                        if (-not $newFoldersToCommit.ContainsKey($folderRelPath)) {
                            $newFoldersToCommit[$folderRelPath] = $folderName
                        }
                        $foundNewFolder = $true
                        break
                    }
                }
            }
            if (-not $foundNewFolder) {
                $filesToCommit += $item
            }
        } else {
            $filesToCommit += $item
        }
    }

    $committedAny = $false

    # 1. Commit new folders: "<folder_name> folder dd/MM/yyyy HH:mm"
    foreach ($entry in $newFoldersToCommit.GetEnumerator()) {
        $folderRelPath = $entry.Key
        $folderName = $entry.Value
        $commitMsg = "$folderName folder $now"

        Write-Host "----------------------------------------------------------" -ForegroundColor DarkGray
        Write-Host "[$now] Detected new folder: '$folderName' ($folderRelPath)" -ForegroundColor Cyan
        git add -A -- "$folderRelPath"
        git commit -m $commitMsg
        if ($LASTEXITCODE -eq 0) {
            Write-Host "[$now] [SUCCESS] Committed folder: '$commitMsg'" -ForegroundColor Green
            $committedAny = $true
        }
    }

    # 2. Commit individual files: "<file_name> dd/MM/yyyy HH:mm"
    foreach ($filePath in $filesToCommit) {
        if ($filePath -match "^(.+)/\.gitkeep$") {
            $folderName = $Matches[1]
            $commitMsg = "$folderName folder $now"
        } else {
            $fileName = [System.IO.Path]::GetFileName($filePath)
            $commitMsg = "$fileName $now"
        }

        Write-Host "----------------------------------------------------------" -ForegroundColor DarkGray
        Write-Host "[$now] Detected file change: $filePath" -ForegroundColor Cyan
        git add -A -- "$filePath"
        git commit -m $commitMsg
        if ($LASTEXITCODE -eq 0) {
            Write-Host "[$now] [SUCCESS] Committed file: '$commitMsg'" -ForegroundColor Green
            $committedAny = $true
        }
    }

    # Step D: Push all new commits to GitHub
    if ($committedAny) {
        Write-Host "[$now] Pushing commits to $Remote $Branch..." -ForegroundColor Yellow
        git push $Remote $Branch
        if ($LASTEXITCODE -eq 0) {
            Write-Host "[$now] [SUCCESS] Pushed to $Branch successfully!" -ForegroundColor Green
        } else {
            Write-Host "[$now] [ERROR] Git push failed. Please verify network or credentials." -ForegroundColor Red
        }
        Write-Host "----------------------------------------------------------`n" -ForegroundColor DarkGray
    }
}

# 3. Setup FileSystemWatcher
$watcher = New-Object System.IO.FileSystemWatcher
$watcher.Path = $RepoPath
$watcher.IncludeSubdirectories = $true
$watcher.EnableRaisingEvents = $true
$watcher.NotifyFilter = [System.IO.NotifyFilters]'FileName, DirectoryName, LastWrite, CreationTime'

# Register event identifiers
Register-ObjectEvent $watcher "Created" -SourceIdentifier "Watcher_Created_Net" | Out-Null
Register-ObjectEvent $watcher "Changed" -SourceIdentifier "Watcher_Changed_Net" | Out-Null
Register-ObjectEvent $watcher "Deleted" -SourceIdentifier "Watcher_Deleted_Net" | Out-Null
Register-ObjectEvent $watcher "Renamed" -SourceIdentifier "Watcher_Renamed_Net" | Out-Null

$changePending = $false
$lastEventTime = [DateTime]::MinValue
$lastPeriodicCheck = [DateTime]::UtcNow

try {
    while ($true) {
        Start-Sleep -Milliseconds 500

        # Check for FileSystemWatcher events
        $events = Get-Event | Where-Object { $_.SourceIdentifier -like "Watcher_*_Net" }
        $hasNewEvent = $false

        if ($events.Count -gt 0) {
            foreach ($evt in $events) {
                $itemPath = $evt.SourceEventArgs.FullPath
                # Ignore internal .git and .vs cache changes
                if ($itemPath -and ($itemPath -like "*\.git\*" -or $itemPath -like "*\.git" -or $itemPath -like "*\.vs\*" -or $itemPath -like "*\.vs")) {
                    continue
                }
                $hasNewEvent = $true
            }
            $events | Remove-Event
        }

        if ($hasNewEvent) {
            $changePending = $true
            $lastEventTime = [DateTime]::UtcNow
        }

        # Periodic fallback check every 5 seconds
        $timeSinceCheck = ([DateTime]::UtcNow - $lastPeriodicCheck).TotalSeconds
        if (-not $changePending -and $timeSinceCheck -ge 5) {
            $lastPeriodicCheck = [DateTime]::UtcNow
            $quickStatus = git status --porcelain
            if (-not [string]::IsNullOrWhiteSpace($quickStatus)) {
                $changePending = $true
                $lastEventTime = [DateTime]::UtcNow
            }
        }

        # Debounce: wait until no new events have occurred for $DebounceSeconds
        if ($changePending) {
            $elapsedSinceEvent = ([DateTime]::UtcNow - $lastEventTime).TotalSeconds
            if ($elapsedSinceEvent -ge $DebounceSeconds) {
                $changePending = $false
                $lastPeriodicCheck = [DateTime]::UtcNow
                Invoke-AutoPush
            }
        }
    }
}
finally {
    # Unregister events and clean up watcher on exit
    Unregister-Event -SourceIdentifier "Watcher_*_Net" -ErrorAction SilentlyContinue
    $watcher.EnableRaisingEvents = $false
    $watcher.Dispose()
    Write-Host "`nAuto Push Service has been stopped." -ForegroundColor Yellow
}
