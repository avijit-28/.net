@echo off
title Auto Git Push - mcc_.net
cd /d "%~dp0"
powershell.exe -NoProfile -ExecutionPolicy Bypass -File "%~dp0auto_push.ps1"
if %ERRORLEVEL% NEQ 0 (
    echo.
    echo Script terminated with error code %ERRORLEVEL%.
    pause
)
