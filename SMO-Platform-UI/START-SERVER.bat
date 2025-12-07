@echo off
echo ================================================
echo   SMO Platform - Starting Local Server
echo ================================================
echo.
echo Server will start on: http://localhost:8000
echo.
echo Press Ctrl+C to stop the server
echo.
echo Opening browser...
echo.

start http://localhost:8000

python -m http.server 8000

pause

