@echo off
title Compilando PS2 to PS4 PKG Builder...
echo ========================================================
echo       COMPILADOR AUTOMATICO DE PS2 TO PS4 BUILDER
echo ========================================================
echo.

set CSC=C:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc.exe

if not exist "%CSC%" (
    echo [ERROR] No se encontro el compilador csc.exe en tu sistema.
    pause
    exit /b 1
)

echo Compilando codigo fuente C#...
set ICON_ARG=
if exist "%~dp0app.ico" set ICON_ARG=/win32icon:"%~dp0app.ico"
"%CSC%" /target:winexe %ICON_ARG% /out:"%~dp0PS2toPS4Builder.exe" /r:System.dll,System.Core.dll,System.Drawing.dll,System.Windows.Forms.dll /recurse:"%~dp0src\*.cs"

if %ERRORLEVEL% equ 0 (
    echo.
    echo ========================================================
    echo  [OK] Compilacion exitosa: PS2toPS4Builder.exe generado.
    echo ========================================================
) else (
    echo.
    echo [ERROR] Hubo errores durante la compilacion.
)

pause
