@echo off
SETLOCAL ENABLEDELAYEDEXPANSION

SET DIR=%~dp0

CALL Tools\CodeGenerator\CodeGenerator.bat "!DIR!" "!DIR!Assets\Generated"

PAUSE
IF %ERRORLEVEL% EQU 0 (
    EXIT /B 0
) ElSE (
    EXIT /B 1
)
