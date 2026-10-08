Set WshShell = CreateObject("WScript.Shell")
' Runs the PowerShell watcher silently with hidden window
WshShell.Run "powershell.exe -ExecutionPolicy Bypass -NoProfile -WindowStyle Hidden -File ""D:\All\.net\auto_push_on_exit.ps1""", 0, False
