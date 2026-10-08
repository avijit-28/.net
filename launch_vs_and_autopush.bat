@echo off
setlocal
set REPO_PATH=D:\All\.net
set BRANCH=main

echo =======================================================
echo  Launching Visual Studio with Auto-Push on Exit
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
echo Visual Studio has closed! Checking for modified or new files...
cd /d "%REPO_PATH%"

for /f %%i in ('git status --porcelain') do (
    goto has_changes
)

echo No changes detected in %REPO_PATH%.
goto end

:has_changes
echo Changes detected! Staging all files...
git add -A

set COMMIT_MSG=Auto-commit on exit %DATE% %TIME%
echo Committing changes...
git commit -m "%COMMIT_MSG%"

echo Pushing changes to %BRANCH%...
git push origin %BRANCH%

if %ERRORLEVEL% equ 0 (
    echo.
    echo [SUCCESS] All modified and newly created files have been pushed to GitHub!
) else (
    echo.
    echo [ERROR] Push failed. Please check your internet or git credentials.
)

:end
echo.
timeout /t 5
