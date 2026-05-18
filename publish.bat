@echo off
echo Publishing Revit CLI Client...

set "SCRIPT_DIR=%~dp0"

if "%1"=="--aot" (
    echo Mode: Native AOT
    dotnet publish ^
      "%SCRIPT_DIR%RevitCliClient.csproj" ^
      -c Release ^
      -r win-x64 ^
      -o "%SCRIPT_DIR%publish"
) else (
    echo Mode: Single-File ^(self-contained^)
    dotnet publish ^
      "%SCRIPT_DIR%RevitCliClient.csproj" ^
      -c Release ^
      -r win-x64 ^
      --self-contained true ^
      -p:PublishSingleFile=true ^
      -p:IncludeNativeLibrariesForSelfExtract=true ^
      -p:PublishAot=false ^
      -o "%SCRIPT_DIR%publish"
)

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
