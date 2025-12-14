@echo off
echo =====================================================
echo    SMO PLATFORM - COMPLETE STARTUP SCRIPT
echo    Starting All Modules and Services
echo =====================================================
echo.

:: Set the project root directory
set PROJECT_ROOT=H:\My Drive\SMO ( PM Prototype)
cd /d "%PROJECT_ROOT%"

echo [1/7] Checking Prerequisites...
echo --------------------------------

:: Check if .NET is installed
dotnet --version >nul 2>&1
if %errorlevel% neq 0 (
    echo ERROR: .NET SDK is not installed or not in PATH
    echo Please install .NET 8.0 SDK from https://dotnet.microsoft.com/download
    pause
    exit /b 1
)
echo ✓ .NET SDK found

:: Check if Node.js is installed (for Angular frontend)
node --version >nul 2>&1
if %errorlevel% neq 0 (
    echo WARNING: Node.js is not installed or not in PATH
    echo Angular frontend will not be available
    set NO_ANGULAR=true
) else (
    echo ✓ Node.js found
    set NO_ANGULAR=false
)

:: Check if SQL Server is running (try different instance names)
set SQL_RUNNING=false

:: Try default instance
sc query MSSQLSERVER | find "RUNNING" >nul 2>&1
if %errorlevel% equ 0 (
    set SQL_RUNNING=true
    set SQL_INSTANCE=.
    echo ✓ SQL Server (MSSQLSERVER) is running
)

:: Try SQL Express instance
if "%SQL_RUNNING%"=="false" (
    sc query MSSQL$SQLEXPRESS | find "RUNNING" >nul 2>&1
    if %errorlevel% equ 0 (
        set SQL_RUNNING=true
        set SQL_INSTANCE=.\SQLEXPRESS
        echo ✓ SQL Server Express is running
    )
)

:: Try LocalDB
if "%SQL_RUNNING%"=="false" (
    sqllocaldb info MSSQLLocalDB >nul 2>&1
    if %errorlevel% equ 0 (
        echo Starting SQL Server LocalDB...
        sqllocaldb start MSSQLLocalDB >nul 2>&1
        set SQL_RUNNING=true
        set SQL_INSTANCE=(LocalDB)\MSSQLLocalDB
        echo ✓ SQL Server LocalDB started
    )
)

:: If still not running, try to start services
if "%SQL_RUNNING%"=="false" (
    echo SQL Server is not running. Attempting to start...
    
    :: Try to start default instance
    net start MSSQLSERVER >nul 2>&1
    if %errorlevel% equ 0 (
        set SQL_RUNNING=true
        set SQL_INSTANCE=.
        echo ✓ SQL Server (MSSQLSERVER) started successfully
    ) else (
        :: Try to start SQL Express
        net start MSSQL$SQLEXPRESS >nul 2>&1
        if %errorlevel% equ 0 (
            set SQL_RUNNING=true
            set SQL_INSTANCE=.\SQLEXPRESS
            echo ✓ SQL Server Express started successfully
        )
    )
)

:: Final check
if "%SQL_RUNNING%"=="false" (
    echo.
    echo =====================================================
    echo ERROR: No SQL Server instance could be found or started
    echo =====================================================
    echo.
    echo Please ensure one of the following is installed:
    echo   1. SQL Server (any edition)
    echo   2. SQL Server Express
    echo   3. SQL Server LocalDB
    echo.
    echo You can:
    echo   - Start SQL Server manually from Services
    echo   - Install SQL Server Express from:
    echo     https://www.microsoft.com/sql-server/sql-server-downloads
    echo.
    echo Once SQL Server is running, run this script again.
    echo =====================================================
    pause
    exit /b 1
)

echo ✓ SQL Server checked (Instance: %SQL_INSTANCE%)

echo.
echo [2/7] Setting up Databases...
echo --------------------------------

:: Check and create SMO_VisionCenter database
sqlcmd -S %SQL_INSTANCE% -Q "SELECT name FROM sys.databases WHERE name = 'SMO_VisionCenter'" -h -1 2>nul | findstr "SMO_VisionCenter" >nul 2>&1
if %errorlevel% neq 0 (
    echo Creating SMO_VisionCenter database...
    sqlcmd -S %SQL_INSTANCE% -Q "CREATE DATABASE SMO_VisionCenter" >nul 2>&1
    if %errorlevel% neq 0 (
        echo WARNING: Could not create SMO_VisionCenter database
        echo Trying with Windows Authentication...
        sqlcmd -S %SQL_INSTANCE% -E -Q "CREATE DATABASE SMO_VisionCenter" >nul 2>&1
    )
    echo ✓ SMO_VisionCenter database created
) else (
    echo ✓ SMO_VisionCenter database exists
)

:: Check and create SMO_Commons database
sqlcmd -S %SQL_INSTANCE% -Q "SELECT name FROM sys.databases WHERE name = 'SMO_Commons'" -h -1 2>nul | findstr "SMO_Commons" >nul 2>&1
if %errorlevel% neq 0 (
    echo Creating SMO_Commons database...
    sqlcmd -S %SQL_INSTANCE% -Q "CREATE DATABASE SMO_Commons" >nul 2>&1
    if %errorlevel% neq 0 (
        sqlcmd -S %SQL_INSTANCE% -E -Q "CREATE DATABASE SMO_Commons" >nul 2>&1
    )
    echo ✓ SMO_Commons database created
) else (
    echo ✓ SMO_Commons database exists
)

:: Check and create SMO_Identity database
sqlcmd -S %SQL_INSTANCE% -Q "SELECT name FROM sys.databases WHERE name = 'SMO_Identity'" -h -1 2>nul | findstr "SMO_Identity" >nul 2>&1
if %errorlevel% neq 0 (
    echo Creating SMO_Identity database...
    sqlcmd -S %SQL_INSTANCE% -Q "CREATE DATABASE SMO_Identity" >nul 2>&1
    if %errorlevel% neq 0 (
        sqlcmd -S %SQL_INSTANCE% -E -Q "CREATE DATABASE SMO_Identity" >nul 2>&1
    )
    echo ✓ SMO_Identity database created
) else (
    echo ✓ SMO_Identity database exists
)

:: Run database setup scripts if they exist
cd /d "%PROJECT_ROOT%\src\Database"
if exist "Setup-Database.sql" (
    echo Running database setup scripts...
    sqlcmd -S %SQL_INSTANCE% -i "Setup-Database.sql" >nul 2>&1
    if %errorlevel% neq 0 (
        sqlcmd -S %SQL_INSTANCE% -E -i "Setup-Database.sql" >nul 2>&1
    )
    echo ✓ Database setup completed
) else (
    if exist "Create-SMO-Database.sql" (
        echo Running database creation script...
        sqlcmd -S %SQL_INSTANCE% -i "Create-SMO-Database.sql" >nul 2>&1
        echo ✓ Database setup completed
    )
)

echo.
echo [3/7] Building Solution...
echo --------------------------------
cd /d "%PROJECT_ROOT%"

:: Build the entire solution
echo Building all projects in solution...
dotnet build SMO.sln --configuration Release >nul 2>&1
if %errorlevel% neq 0 (
    echo ERROR: Solution build failed. Running detailed build...
    dotnet build SMO.sln
    pause
    exit /b 1
)
echo ✓ Solution built successfully

echo.
echo [4/7] Starting Backend API...
echo --------------------------------
cd /d "%PROJECT_ROOT%\src\SMO.Api"

:: Start the API in a new window
echo Starting API server...
start "SMO API Server" cmd /k "dotnet run --urls https://localhost:7001"
echo ✓ API server starting on https://localhost:7001

:: Wait for API to start
echo Waiting for API to be ready...
timeout /t 5 /nobreak >nul

echo.
echo [5/7] Starting Angular Frontend (if available)...
echo --------------------------------

if "%NO_ANGULAR%"=="false" (
    cd /d "%PROJECT_ROOT%\src\SMO.Frontend\SMO-Portal"
    
    :: Check if node_modules exist
    if not exist "node_modules" (
        echo Installing Angular dependencies (this may take a few minutes)...
        call npm install >nul 2>&1
        if %errorlevel% neq 0 (
            echo ERROR: Failed to install Angular dependencies
            echo Please run 'npm install' manually in src\SMO.Frontend\SMO-Portal
            set ANGULAR_FAILED=true
        ) else (
            echo ✓ Angular dependencies installed
            set ANGULAR_FAILED=false
        )
    ) else (
        echo ✓ Angular dependencies already installed
        set ANGULAR_FAILED=false
    )
    
    if "%ANGULAR_FAILED%"=="false" (
        echo Starting Angular development server...
        start "SMO Angular Frontend" cmd /k "npm start"
        echo ✓ Angular frontend starting on http://localhost:4200
    )
) else (
    echo ⚠ Skipping Angular frontend (Node.js not found)
)

echo.
echo [6/7] Starting HTML Frontend...
echo --------------------------------
cd /d "%PROJECT_ROOT%\SMO-Platform-UI"

:: Check if Python is available
python --version >nul 2>&1
if %errorlevel% equ 0 (
    echo Starting HTML frontend with Python...
    start "SMO HTML Frontend" cmd /k "python -m http.server 8080"
    echo ✓ HTML frontend server starting on http://localhost:8080
) else (
    :: Try with Node.js
    npx --version >nul 2>&1
    if %errorlevel% equ 0 (
        echo Starting HTML frontend with Node.js...
        start "SMO HTML Frontend" cmd /k "npx http-server -p 8080 -c-1"
        echo ✓ HTML frontend server starting on http://localhost:8080
    ) else (
        echo WARNING: Neither Python nor Node.js found for HTML frontend
        echo You can manually open the HTML files in your browser
    )
)

echo.
echo [7/7] Opening Applications...
echo --------------------------------

:: Wait a moment for servers to fully start
timeout /t 5 /nobreak >nul

:: Open Swagger UI
echo Opening Swagger API Documentation...
start https://localhost:7001/swagger

:: Open HTML Frontend
echo Opening SMO Platform HTML UI...
start http://localhost:8080

:: Open Angular Frontend if available
if "%NO_ANGULAR%"=="false" (
    if "%ANGULAR_FAILED%"=="false" (
        echo Opening SMO Angular Portal...
        start http://localhost:4200
    )
)

echo.
echo =====================================================
echo    ✓ SMO PLATFORM - ALL MODULES RUNNING!
echo =====================================================
echo.
echo   BACKEND SERVICES:
echo   ─────────────────
echo   • API Server:       https://localhost:7001
echo   • Swagger Docs:     https://localhost:7001/swagger
echo.
echo   FRONTEND APPLICATIONS:
echo   ──────────────────────
if "%NO_ANGULAR%"=="false" (
    if "%ANGULAR_FAILED%"=="false" (
        echo   • Angular Portal:   http://localhost:4200 [Modern UI]
    )
)
echo   • HTML Portal:      http://localhost:8080 [Legacy UI]
echo.
echo   KEY PAGES TO TEST:
echo   ──────────────────
echo   • Dashboard:        http://localhost:8080/performance-dashboard.html
echo   • Profile:          http://localhost:8080/profile-enhanced.html
echo   • Vision 2030:      http://localhost:8080/vision.html
echo   • Programs:         http://localhost:8080/performance-programs.html
echo   • Login:            http://localhost:8080/login.html
echo.
echo   DEFAULT CREDENTIALS:
echo   ────────────────────
echo   • Admin:            admin@smo.gov.sa / Admin@123
echo   • User:             user@smo.gov.sa / User@123
echo.
echo   DATABASES:
echo   ──────────
echo   • SMO_VisionCenter  (Main database)
echo   • SMO_Commons       (Shared resources)
echo   • SMO_Identity      (Authentication)
echo.
echo =====================================================
echo   SHUTDOWN INSTRUCTIONS:
echo   • Press Ctrl+C in each window to stop services
echo   • Or close this window to keep services running
echo =====================================================
echo.

pause
