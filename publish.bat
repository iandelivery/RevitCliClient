@echo off
echo Publishing Revit CLI Client...

set "SCRIPT_DIR=%~dp0"

dotnet publish ^
  "%SCRIPT_DIR%RevitCliClient.csproj" ^
  -c Release ^
  -r win-x64 ^
  --self-contained true ^
  -p:PublishSingleFile=true ^
  -p:IncludeNativeLibrariesForSelfExtract=true ^
  -o "%SCRIPT_DIR%publish"

if %ERRORLEVEL% EQU 0 (
    echo.
    echo Published successfully!
    echo Output: %SCRIPT_DIR%publish
    echo Executable: %SCRIPT_DIR%publish\RevitCliClient.exe
) else (
    echo.
    echo Publish failed!
    exit /b 1
)
