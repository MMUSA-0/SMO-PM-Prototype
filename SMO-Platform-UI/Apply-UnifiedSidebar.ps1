# PowerShell Script to Apply Unified Sidebar to All HTML Files
# Author: SMO Development Team
# Version: 1.0

param(
    [string]$Path = ".",
    [switch]$WhatIf,
    [switch]$NoBackup
)

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  Unified Sidebar Implementation Tool  " -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Create backup directory if needed
if (-not $NoBackup) {
    $backupDir = Join-Path $Path "backup_$(Get-Date -Format 'yyyyMMdd_HHmmss')"
    if (-not (Test-Path $backupDir)) {
        New-Item -ItemType Directory -Path $backupDir | Out-Null
        Write-Host "Created backup directory: $backupDir" -ForegroundColor Green
    }
}

# Get all HTML files
$htmlFiles = Get-ChildItem -Path $Path -Filter "*.html" -File

if ($htmlFiles.Count -eq 0) {
    Write-Host "No HTML files found in the current directory." -ForegroundColor Yellow
    exit
}

Write-Host "Found $($htmlFiles.Count) HTML files to process" -ForegroundColor White
Write-Host ""

$updatedCount = 0
$skippedCount = 0
$errorCount = 0

# CSS to add in head
$cssLink = @"
<!-- Unified Sidebar Styles -->
<link rel="stylesheet" href="assets/css/sidebar-unified.css">
"@

# Sidebar container HTML
$sidebarContainer = @"
<!-- Unified Sidebar Container -->
<div id="sidebarContainer">
    <!-- Sidebar will be loaded here dynamically -->
</div>
"@

# Scripts to add before body close
$scripts = @"

<!-- Unified Sidebar Manager -->
<script src="assets/js/sidebar-manager.js"></script>

<!-- Auto-apply unified sidebar -->
<script src="assets/js/apply-unified-sidebar.js"></script>
"@

foreach ($file in $htmlFiles) {
    Write-Host "Processing: $($file.Name)" -ForegroundColor White
    
    try {
        $content = Get-Content -Path $file.FullName -Raw -Encoding UTF8
        
        # Check if already has unified sidebar
        if ($content -match "sidebar-manager\.js") {
            Write-Host "  ✓ Already has unified sidebar" -ForegroundColor Green
            $skippedCount++
            continue
        }
        
        # Backup the file
        if (-not $NoBackup -and -not $WhatIf) {
            $backupFile = Join-Path $backupDir $file.Name
            Copy-Item -Path $file.FullName -Destination $backupFile
            Write-Host "  → Backed up to: $backupFile" -ForegroundColor Gray
        }
        
        $modified = $false
        
        # Add CSS link before </head>
        if ($content -notmatch "sidebar-unified\.css") {
            $content = $content -replace "(\s*</head>)", "$cssLink`n`$1"
            $modified = $true
            Write-Host "  + Added CSS link" -ForegroundColor Yellow
        }
        
        # Replace existing sidebar with container
        if ($content -match '<aside[^>]*class="[^"]*page-sidebar[^"]*"[^>]*>') {
            # Remove old sidebar (from <aside> to </aside>)
            $pattern = '<aside[^>]*class="[^"]*page-sidebar[^"]*"[^>]*>[\s\S]*?</aside>'
            $content = $content -replace $pattern, $sidebarContainer
            $modified = $true
            Write-Host "  + Replaced existing sidebar" -ForegroundColor Yellow
        } elseif ($content -match '<div[^>]*class="[^"]*page-container[^"]*"[^>]*>') {
            # Add sidebar container after page-container opening
            $pattern = '(<div[^>]*class="[^"]*page-container[^"]*"[^>]*>)'
            $content = $content -replace $pattern, "`$1`n            $sidebarContainer"
            $modified = $true
            Write-Host "  + Added sidebar container" -ForegroundColor Yellow
        }
        
        # Add scripts before </body>
        if ($content -notmatch "sidebar-manager\.js") {
            $content = $content -replace "(\s*</body>)", "$scripts`n`$1"
            $modified = $true
            Write-Host "  + Added JavaScript files" -ForegroundColor Yellow
        }
        
        # Save the modified file
        if ($modified) {
            if ($WhatIf) {
                Write-Host "  [WhatIf] Would update file" -ForegroundColor Cyan
            } else {
                $content | Set-Content -Path $file.FullName -Encoding UTF8 -NoNewline
                Write-Host "  ✓ Updated successfully!" -ForegroundColor Green
            }
            $updatedCount++
        } else {
            Write-Host "  - No changes needed" -ForegroundColor Gray
            $skippedCount++
        }
        
    } catch {
        Write-Host "  ✗ Error: $_" -ForegroundColor Red
        $errorCount++
    }
    
    Write-Host ""
}

# Summary
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Summary:" -ForegroundColor White
Write-Host "  Total files processed: $($htmlFiles.Count)" -ForegroundColor White
Write-Host "  Files updated: $updatedCount" -ForegroundColor Green
Write-Host "  Files skipped: $skippedCount" -ForegroundColor Yellow
if ($errorCount -gt 0) {
    Write-Host "  Errors: $errorCount" -ForegroundColor Red
}
if (-not $NoBackup -and -not $WhatIf) {
    Write-Host "  Backups saved in: $backupDir" -ForegroundColor Gray
}
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

if (-not $WhatIf) {
    Write-Host "Implementation complete!" -ForegroundColor Green
    Write-Host ""
    Write-Host "Next steps:" -ForegroundColor White
    Write-Host "1. Test the updated pages in your browser" -ForegroundColor White
    Write-Host "2. Use Ctrl+B to toggle sidebar" -ForegroundColor White
    Write-Host "3. Use Ctrl+K to search in menu" -ForegroundColor White
    Write-Host ""
    
    # Offer to open implementation guide
    $openGuide = Read-Host "Would you like to open the implementation guide? (Y/N)"
    if ($openGuide -eq 'Y' -or $openGuide -eq 'y') {
        Start-Process "implement-unified-sidebar.html"
    }
}

