<#
================================================================================
 Auto Git Push Script for D:\avi\C#
 Target Branch: mcc_.net (Remote: origin)
 Repository   : https://github.com/avijit-28/.net
 
 Functionality:
 - Monitors D:\avi\C# recursively for new files, folders, and modifications.
 - Automatically tracks newly created empty folders by adding a .gitkeep.
 - Ignores internal Git (.git) and Visual Studio cache (.vs) events.
 - Debounces rapid edits (waiting 3 seconds after the last file change).
 - Formats commit message: "<file_name> - yyyy-MM-dd HH:mm:ss"
 - Ensures current branch is 'mcc_.net' and pushes to 'origin mcc_.net'.
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
Write-Host " Started At      : $(Get-Date -Format 'yyyy-MM-dd HH:mm:ss')" -ForegroundColor Gray
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

    # Step C: Parse modified / created / deleted files from status
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

            # If it's a folder's .gitkeep, represent as folder name
            if ($cleanPath -match "^(.+)/\.gitkeep$") {
                $changedItems += $Matches[1]
            } else {
                $changedItems += $cleanPath
            }
        }
    }

    $changedItems = @($changedItems | Select-Object -Unique)

    if ($changedItems.Count -eq 0) {
        return
    }

    # Prioritize user code / project files over internal .vs cache in commit message
    $meaningfulItems = @($changedItems | Where-Object { $_ -notmatch '(^|[\\/])\.vs([\\/]|$)' })
    if ($meaningfulItems.Count -gt 0) {
        $displayItems = $meaningfulItems
    } else {
        $displayItems = $changedItems
    }

    # Step D: Construct commit message
    $now = Get-Date -Format "yyyy-MM-dd HH:mm:ss"
    if ($displayItems.Count -eq 1) {
        $commitMsg = "$($displayItems[0]) - $now"
    } elseif ($displayItems.Count -le 3) {
        $commitMsg = "$($displayItems -join ', ') - $now"
    } else {
        $firstTwo = ($displayItems | Select-Object -First 2) -join ', '
        $remaining = $displayItems.Count - 2
        $commitMsg = "$firstTwo (+$remaining files) - $now"
    }

    Write-Host "----------------------------------------------------------" -ForegroundColor DarkGray
    Write-Host "[$now] Changes detected in: $($displayItems -join ', ')" -ForegroundColor Cyan
    Write-Host "[$now] Staging changes (git add -A)..." -ForegroundColor Gray
    git add -A

    Write-Host "[$now] Committing: '$commitMsg'..." -ForegroundColor Gray
    git commit -m $commitMsg

    if ($LASTEXITCODE -eq 0) {
        Write-Host "[$now] Pushing to $Remote $Branch..." -ForegroundColor Yellow
        git push $Remote $Branch
        if ($LASTEXITCODE -eq 0) {
            Write-Host "[$now] [SUCCESS] Pushed to $Branch successfully!" -ForegroundColor Green
        } else {
            Write-Host "[$now] [ERROR] Git push failed. Please verify network or credentials." -ForegroundColor Red
        }
    } else {
        Write-Host "[$now] [WARNING] Commit skipped or nothing to commit." -ForegroundColor DarkYellow
    }
    Write-Host "----------------------------------------------------------`n" -ForegroundColor DarkGray
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
