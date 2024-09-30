@echo off
cls

dotnet --version >nul 2>&1
if %ERRORLEVEL% neq 0 (
    echo .NET SDK is not installed. Please install .NET SDK to continue.
    pause
    exit /b
)

cd /d "%~dp0"
set rootOutputDir="%~dp0build"

echo Publishing FortBackend to "build/FortBackend" folder...
dotnet publish "FortBackend/FortBackend.csproj" --configuration Release --runtime win-x64 --output "%rootOutputDir%/FortBackend" --self-contained -p:PublishSingleFile=true

if %ERRORLEVEL% neq 0 (
    echo Publish failed for FortBackend.
    pause
    exit /b
)

echo Publishing FortMatchmaker to "build/FortMatchmaker" folder...
dotnet publish "FortMatchmaker/FortMatchmaker.csproj" --configuration Release --runtime win-x64 --output "%rootOutputDir%/FortMatchmaker" --self-contained -p:PublishSingleFile=true

if %ERRORLEVEL% neq 0 (
    echo Publish failed for FortMatchmaker.
    pause
    exit /b
)

echo All projects published successfully to the "build" folder!
pause
