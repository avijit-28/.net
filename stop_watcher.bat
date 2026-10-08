@echo off
echo Stopping Git Auto-Push Watcher...
powershell -Command "Get-CimInstance Win32_Process | Where-Object { $_.CommandLine -like '*auto_push_on_exit.ps1*' } | ForEach-Object { Stop-Process -Id $_.ProcessId -Force; Write-Host 'Stopped watcher process ID:' $_.ProcessId }"
echo Done.
pause
