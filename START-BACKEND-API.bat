@echo off
echo =====================================================
echo    SMO BACKEND API STARTUP
echo    Starting .NET Core API Services
echo =====================================================
echo.

set PROJECT_ROOT=H:\My Drive\SMO ( PM Prototype)
cd /d "%PROJECT_ROOT%"

echo [1/3] Checking Prerequisites...
echo --------------------------------

:: Check if .NET is installed
dotnet --version >nul 2>&1
if %errorlevel% neq 0 (
    echo ERROR: .NET SDK is not installed
    echo Please install from: https://dotnet.microsoft.com/download
    pause
    exit /b 1
)
echo ✓ .NET SDK found

echo.
echo [2/3] Building API Project...
echo --------------------------------
cd /d "%PROJECT_ROOT%\src\SMO.Api"

echo Building in Release mode...
dotnet build --configuration Release >nul 2>&1
if %errorlevel% neq 0 (
    echo Build failed! Trying with detailed output...
    dotnet build
    pause
    exit /b 1
)
echo ✓ Build successful

echo.
echo [3/3] Starting API Server...
echo --------------------------------

echo Starting API on https://localhost:7001...
echo.
echo =====================================================
echo    API STARTING...
echo =====================================================
echo.
echo   Once started, access at:
echo   - API:     https://localhost:7001
echo   - Swagger: https://localhost:7001/swagger
echo.
echo   Note: If you see database errors, that's OK!
echo   The API will still run with limited functionality.
echo.
echo   Press Ctrl+C to stop the server
echo =====================================================
echo.

:: Run the API (this will keep the window open)
dotnet run --urls https://localhost:7001

