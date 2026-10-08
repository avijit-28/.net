<#
.SYNOPSIS
    Automated Git Auto-Push Watcher for Visual Studio
.DESCRIPTION
    Monitors Visual Studio (devenv.exe or Code.exe).
    When Visual Studio is closed, checks if any files/folders were added or modified in D:\All\.net,
    generates a smart, informative commit message describing the modified/added files and tasks,
    and automatically commits and pushes them to GitHub on the 'main' branch.
#>

param(
    [switch]$Once,
    [string]$repoPath = "D:\All\.net",
    [string]$branch = "main",
    [string[]]$processNames = @("devenv", "Code"),
    [int]$checkIntervalSeconds = 5
)

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

function Generate-SmartCommitMessage {
    param (
        [string[]]$statusLines
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

    # Helper function to get simple base filename
    $getFileName = { param([string]$p) [System.IO.Path]::GetFileName($p) }

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

    # Add project/topic context
    $projContext = ""
    if ($projects.Count -eq 1) {
        $singleProj = [System.Linq.Enumerable]::First($projects)
        $projContext = " in $singleProj"
    } elseif ($projects.Count -gt 1) {
        $firstFew = ($projects | Select-Object -First 2) -join ', '
        $projContext = " across $($projects.Count) projects ($firstFew)"
    }

    $subject = ($actionParts -join ' and ') + $projContext

    # Fallback if subject string is empty or overly long
    if ([string]::IsNullOrWhiteSpace($subject)) {
        $subject = "Auto-update ($totalCount files changed)"
    } elseif ($subject.Length -gt 72) {
        $subject = $subject.Substring(0, 69) + "..."
    }

    # Build detailed body breakdown
    $bodyLines = [System.Collections.Generic.List[string]]::new()
    $bodyLines.Add("Work completed in Visual Studio session:")
    $bodyLines.Add("Timestamp: $(Get-Date -Format 'yyyy-MM-dd HH:mm:ss')")
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

    # Check for changes (untracked, modified, deleted)
    $statusOutput = git status --porcelain
    if (-not [string]::IsNullOrWhiteSpace($statusOutput)) {
        $statusLines = $statusOutput -split "`r?`n" | Where-Object { -not [string]::IsNullOrWhiteSpace($_) }
        
        Write-Host "[$(Get-Date -Format 'HH:mm:ss')] Detected $($statusLines.Count) changed file(s)." -ForegroundColor Yellow
        
        # Generate informative commit message based on the actual changes
        $commitInfo = Generate-SmartCommitMessage -statusLines $statusLines
        $subject = $commitInfo.Subject
        $body = $commitInfo.Body

        Write-Host "[Commit Title] $subject" -ForegroundColor Cyan
        Write-Host "Staging all changes..." -ForegroundColor Gray
        git add -A

        Write-Host "Committing changes..." -ForegroundColor Gray
        git commit -m "$subject" -m "$body"

        Write-Host "Pushing to origin $branch..." -ForegroundColor Gray
        $pushOutput = git push origin $branch 2>&1

        if ($LASTEXITCODE -eq 0) {
            Write-Host "[$(Get-Date -Format 'HH:mm:ss')] Successfully pushed to GitHub!" -ForegroundColor Green
            Show-Notification -Title "Git Auto-Push Successful" -Message "$subject"
        } else {
            Write-Host "[$(Get-Date -Format 'HH:mm:ss')] Push failed: $pushOutput" -ForegroundColor Red
            Show-Notification -Title "Git Auto-Push Failed" -Message "Error pushing to GitHub. Check terminal for details."
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
Write-Host " Git Auto-Push Monitor for Visual Studio" -ForegroundColor Green
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
            Write-Host "[$(Get-Date -Format 'HH:mm:ss')] Visual Studio was CLOSED. Checking for changes..." -ForegroundColor Yellow
            $wasRunning = $false
            Push-Changes
        }
    }

    Start-Sleep -Seconds $checkIntervalSeconds
}
