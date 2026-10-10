Set WshShell = CreateObject("WScript.Shell")
WshShell.Run "powershell.exe -ExecutionPolicy Bypass -NoProfile -WindowStyle Hidden -File ""D:\All\.net\auto_push_on_exit.ps1""", 0, False
