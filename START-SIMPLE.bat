@echo off
echo =====================================================
echo    SMO PLATFORM - SIMPLE STARTUP
echo    Starting Available Components
echo =====================================================
echo.

set PROJECT_ROOT=H:\My Drive\SMO ( PM Prototype)
cd /d "%PROJECT_ROOT%"

echo [Step 1] Starting Frontend Web Server...
echo -----------------------------------------
cd /d "%PROJECT_ROOT%\SMO-Platform-UI"

:: Try Python first (most common)
python --version >nul 2>&1
if %errorlevel% equ 0 (
    echo Starting with Python on port 8080...
    start "SMO Frontend" cmd /k "python -m http.server 8080"
    echo.
    echo ✓ Frontend running at: http://localhost:8080
    echo.
    goto :open_browser
)

:: Try Node.js as fallback
node --version >nul 2>&1
if %errorlevel% equ 0 (
    echo Starting with Node.js on port 8080...
    start "SMO Frontend" cmd /k "npx http-server -p 8080 -c-1"
    echo.
    echo ✓ Frontend running at: http://localhost:8080
    echo.
    goto :open_browser
)

:: If neither available, open directly in browser
echo No web server found. Opening files directly...
echo.

:open_browser
echo [Step 2] Opening SMO Platform in Browser...
echo -----------------------------------------
timeout /t 2 /nobreak >nul

:: Try to open the served version first
curl http://localhost:8080 >nul 2>&1
if %errorlevel% equ 0 (
    start http://localhost:8080
    start http://localhost:8080/performance-dashboard.html
) else (
    :: Open files directly if no server
    cd /d "%PROJECT_ROOT%\SMO-Platform-UI"
    start index.html
    start performance-dashboard.html
)

echo.
echo =====================================================
echo    SMO PLATFORM STARTED!
echo =====================================================
echo.
echo   PAGES OPENED:
echo   - Main Index
echo   - Performance Dashboard
echo.
echo   OTHER KEY PAGES:
echo   - http://localhost:8080/vision.html
echo   - http://localhost:8080/login.html
echo   - http://localhost:8080/profile-enhanced.html
echo.
echo   LOGIN CREDENTIALS:
echo   - Admin: admin@smo.gov.sa / Admin@123
echo   - User:  user@smo.gov.sa / User@123
echo.
echo =====================================================
echo   Press Ctrl+C to stop the server
echo =====================================================
echo.

pause

