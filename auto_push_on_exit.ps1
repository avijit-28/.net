<#
.SYNOPSIS
    Automated Git Auto-Push Watcher for Visual Studio
.DESCRIPTION
    Monitors Visual Studio (devenv.exe or Code.exe).
    When Visual Studio is closed, checks if any files/folders were added or modified in D:\All\.net,
    and automatically commits and pushes them to GitHub on the 'main' branch.
#>

$repoPath = "D:\All\.net"
$branch = "main"
$processNames = @("devenv", "Code") # Monitors both Visual Studio and VS Code
$checkIntervalSeconds = 5

Write-Host "=======================================================" -ForegroundColor Cyan
Write-Host " Git Auto-Push Monitor for Visual Studio" -ForegroundColor Green
Write-Host " Watching Repo : $repoPath" -ForegroundColor Yellow
Write-Host " Target Branch : $branch" -ForegroundColor Yellow
Write-Host " Monitoring    : $($processNames -join ', ')" -ForegroundColor Yellow
Write-Host "=======================================================" -ForegroundColor Cyan

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
        # Fallback if Windows toast notification is not available
        Write-Host "[$Title] $Message" -ForegroundColor Magenta
    }
}

function Push-Changes {
    Set-Location -Path $repoPath

    # Check for changes (untracked, modified, deleted)
    $status = git status --porcelain
    if (-not [string]::IsNullOrWhiteSpace($status)) {
        Write-Host "[$(Get-Date -Format 'HH:mm:ss')] Detected modified or new files in repository." -ForegroundColor Yellow
        
        Write-Host "Staging all changes..." -ForegroundColor Gray
        git add -A

        $commitMsg = "Auto-commit on Visual Studio close: $(Get-Date -Format 'yyyy-MM-dd HH:mm:ss')"
        Write-Host "Committing changes..." -ForegroundColor Gray
        git commit -m "$commitMsg"

        Write-Host "Pushing to origin $branch..." -ForegroundColor Gray
        $pushOutput = git push origin $branch 2>&1

        if ($LASTEXITCODE -eq 0) {
            Write-Host "[$(Get-Date -Format 'HH:mm:ss')] Successfully pushed to GitHub!" -ForegroundColor Green
            Show-Notification -Title "Git Auto-Push" -Message "Changes pushed to GitHub (branch: $branch)"
        } else {
            Write-Host "[$(Get-Date -Format 'HH:mm:ss')] Push failed: $pushOutput" -ForegroundColor Red
            Show-Notification -Title "Git Auto-Push Failed" -Message "Error pushing to GitHub. Check terminal for details."
        }
    } else {
        Write-Host "[$(Get-Date -Format 'HH:mm:ss')] Visual Studio closed, but no file changes were detected." -ForegroundColor DarkGray
    }
}

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
