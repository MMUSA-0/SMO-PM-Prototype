@echo off
echo =============================================
echo SMO Platform Database Setup
echo =============================================
echo.

REM Check if SQL Server is running
sc query MSSQLSERVER | find "RUNNING" >nul
if %errorlevel% neq 0 (
    echo ERROR: SQL Server service is not running!
    echo Please start SQL Server service and try again.
    pause
    exit /b 1
)

echo Creating SMO_Production database...
echo.

REM Run the SQL script
sqlcmd -S localhost -E -i "Create-SMO-Database.sql"

if %errorlevel% equ 0 (
    echo.
    echo =============================================
    echo SUCCESS! Database created successfully!
    echo =============================================
    echo.
    echo Database: SMO_Production
    echo Tables created: 15
    echo - Visions
    echo - Programs  
    echo - Initiatives
    echo - KPIs
    echo - KPIValues
    echo - Milestones
    echo - Risks
    echo - Achievements
    echo - WorkflowApprovals
    echo - AuditLogs
    echo - Attachments
    echo - Notifications
    echo - Reports
    echo - Settings
    echo - Thresholds
    echo.
    echo Initial data inserted:
    echo - Settings configured
    echo - Vision 2030 sample data
    echo - 3 sample programs
    echo.
    echo You can now run the API and connect!
) else (
    echo.
    echo ERROR: Database creation failed!
    echo Please check the error messages above.
)

pause
