@echo off
echo =====================================================
echo    SQL SERVER EXPRESS INSTALLER HELPER
echo =====================================================
echo.

echo This script will help you install SQL Server Express.
echo.
echo [Option 1] Download SQL Server Express 2022
echo -------------------------------------------
echo Opening download page...
start https://www.microsoft.com/en-us/sql-server/sql-server-downloads
echo.
echo 1. Click "Download now" under SQL Server Express
echo 2. Run the installer
echo 3. Choose "Basic" installation
echo 4. Accept the license terms
echo 5. Choose default installation path
echo 6. Wait for installation to complete
echo.

echo [Option 2] Install SQL Server LocalDB (Lighter)
echo ------------------------------------------------
echo LocalDB is a lightweight version perfect for development.
echo.
echo Opening LocalDB download page...
start https://docs.microsoft.com/en-us/sql/database-engine/configure-windows/sql-server-express-localdb
echo.

pause

echo.
echo [Checking Current SQL Server Status]
echo -------------------------------------

:: Check for SQL Server services
echo Checking for SQL Server installations...
echo.

sc query MSSQLSERVER >nul 2>&1
if %errorlevel% equ 0 (
    echo ✓ SQL Server (default instance) is installed
    sc query MSSQLSERVER | find "RUNNING" >nul 2>&1
    if %errorlevel% equ 0 (
        echo   Status: RUNNING
    ) else (
        echo   Status: STOPPED
        echo   To start: net start MSSQLSERVER
    )
) else (
    echo ✗ SQL Server (default instance) not found
)

echo.

sc query MSSQL$SQLEXPRESS >nul 2>&1
if %errorlevel% equ 0 (
    echo ✓ SQL Server Express is installed
    sc query MSSQL$SQLEXPRESS | find "RUNNING" >nul 2>&1
    if %errorlevel% equ 0 (
        echo   Status: RUNNING
    ) else (
        echo   Status: STOPPED
        echo   To start: net start MSSQL$SQLEXPRESS
    )
) else (
    echo ✗ SQL Server Express not found
)

echo.

sqllocaldb info >nul 2>&1
if %errorlevel% equ 0 (
    echo ✓ SQL Server LocalDB is installed
    sqllocaldb info MSSQLLocalDB >nul 2>&1
    if %errorlevel% equ 0 (
        echo   Instance: MSSQLLocalDB available
    )
) else (
    echo ✗ SQL Server LocalDB not found
)

echo.
echo =====================================================
echo After installation, run START-SMO-PLATFORM.bat again
echo =====================================================
echo.

pause

