@echo off
echo ================================================
echo   SMO Platform - Starting Local Server (Node.js)
echo ================================================
echo.
echo Server will start on: http://localhost:8000
echo.
echo Press Ctrl+C to stop the server
echo.
echo Opening browser...
echo.

start http://localhost:8000

npx -y http-server -p 8000 -o

pause

