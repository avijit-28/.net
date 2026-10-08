@echo off
setlocal
set REPO_PATH=D:\All\.net
set BRANCH=main

echo =======================================================
echo  Launching Visual Studio with Smart Auto-Push on Exit
echo  Repo: %REPO_PATH%
echo =======================================================

cd /d "%REPO_PATH%"

:: Check for devenv (Visual Studio) or Code (VS Code)
where devenv >nul 2>nul
if %ERRORLEVEL% equ 0 (
    echo Starting Visual Studio...
    start /wait devenv .
) else (
    where code >nul 2>nul
    if %ERRORLEVEL% equ 0 (
        echo Starting VS Code...
        start /wait code --wait .
    ) else (
        echo Opening folder in default handler or solution...
        start /wait "" "%REPO_PATH%"
        pause
    )
)

echo.
echo Visual Studio has closed! Running Git auto-push with smart commit message...

powershell -NoProfile -ExecutionPolicy Bypass -File "D:\All\.net\auto_push_on_exit.ps1" -Once

echo.
timeout /t 5
