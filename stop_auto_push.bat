@echo off
echo Stopping auto_push background service...
powershell -NoProfile -Command "Get-CimInstance Win32_Process | Where-Object { $_.CommandLine -like '*auto_push.ps1*' } | ForEach-Object { Stop-Process -Id $_.ProcessId -Force; Write-Host 'Stopped auto_push process ID:' $_.ProcessId }"
echo Done.
pause
