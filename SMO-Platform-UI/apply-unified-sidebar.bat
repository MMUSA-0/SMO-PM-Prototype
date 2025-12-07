@echo off
setlocal enabledelayedexpansion

echo ========================================
echo   Unified Sidebar Implementation Tool
echo ========================================
echo.

:: Create backup directory
if not exist "backup" (
    mkdir backup
    echo Created backup directory
)

:: Counter for processed files
set /a count=0
set /a updated=0

echo Processing HTML files...
echo.

:: Process all HTML files
for %%f in (*.html) do (
    set /a count+=1
    echo Processing: %%f
    
    :: Create backup
    copy "%%f" "backup\%%f.bak" >nul 2>&1
    
    :: Check if file already has unified sidebar
    findstr /C:"sidebar-manager.js" "%%f" >nul 2>&1
    if errorlevel 1 (
        echo   - Adding unified sidebar references...
        
        :: Create temporary file with modifications
        (
            for /f "delims=" %%L in ('type "%%f"') do (
                set "line=%%L"
                
                :: Add CSS before </head>
                echo !line! | findstr /C:"</head>" >nul 2>&1
                if not errorlevel 1 (
                    echo ^<!-- Unified Sidebar Styles --^>
                    echo ^<link rel="stylesheet" href="assets/css/sidebar-unified.css"^>
                )
                
                :: Replace old sidebar with container
                echo !line! | findstr /C:"page-sidebar" >nul 2>&1
                if not errorlevel 1 (
                    echo ^<!-- Unified Sidebar Container --^>
                    echo ^<div id="sidebarContainer"^>^</div^>
                    :: Skip the old sidebar content
                    set "skipSidebar=1"
                ) else if defined skipSidebar (
                    echo !line! | findstr /C:"</aside>" >nul 2>&1
                    if not errorlevel 1 (
                        set "skipSidebar="
                    )
                ) else (
                    echo !line!
                )
                
                :: Add scripts before </body>
                echo !line! | findstr /C:"</body>" >nul 2>&1
                if not errorlevel 1 (
                    echo.
                    echo ^<!-- Unified Sidebar Manager --^>
                    echo ^<script src="assets/js/sidebar-manager.js"^>^</script^>
                    echo ^<script src="assets/js/apply-unified-sidebar.js"^>^</script^>
                )
            )
        ) > "%%f.tmp"
        
        :: Replace original with modified
        move /y "%%f.tmp" "%%f" >nul 2>&1
        set /a updated+=1
        echo   - Updated successfully!
    ) else (
        echo   - Already has unified sidebar, skipping...
    )
    echo.
)

echo ========================================
echo Summary:
echo   Total files processed: %count%
echo   Files updated: %updated%
echo   Backups saved in: backup\
echo ========================================
echo.

:: Offer to open the implementation guide
echo Would you like to open the implementation guide?
choice /C YN /M "Press Y for Yes, N for No"
if errorlevel 2 goto end
if errorlevel 1 start implement-unified-sidebar.html

:end
echo.
echo Implementation complete!
echo.
echo Next steps:
echo 1. Test the updated pages in your browser
echo 2. Use Ctrl+B to toggle sidebar
echo 3. Use Ctrl+K to search in menu
echo.
pause

