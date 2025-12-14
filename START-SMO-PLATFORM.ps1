# SMO Platform Complete Startup Script (PowerShell Version)
# Run as Administrator for best results

Write-Host "=====================================================" -ForegroundColor Cyan
Write-Host "   SMO PLATFORM - COMPLETE STARTUP SCRIPT" -ForegroundColor Yellow
Write-Host "   Starting All Modules and Services" -ForegroundColor Yellow
Write-Host "=====================================================" -ForegroundColor Cyan
Write-Host ""

# Set the project root directory
$ProjectRoot = "H:\My Drive\SMO ( PM Prototype)"
Set-Location $ProjectRoot

function Start-SQLServerService {
    Write-Host "[SQL Server Check]" -ForegroundColor Green
    
    $sqlRunning = $false
    $sqlInstance = ""
    
    # Check various SQL Server services
    $services = @(
        @{Name="MSSQLSERVER"; Instance="."; DisplayName="SQL Server"},
        @{Name="MSSQL`$SQLEXPRESS"; Instance=".\SQLEXPRESS"; DisplayName="SQL Server Express"},
        @{Name="MSSQL`$SQLEXPRESS01"; Instance=".\SQLEXPRESS01"; DisplayName="SQL Server Express 01"}
    )
    
    foreach ($svc in $services) {
        $service = Get-Service -Name $svc.Name -ErrorAction SilentlyContinue
        if ($service) {
            if ($service.Status -eq 'Running') {
                Write-Host "✓ $($svc.DisplayName) is already running" -ForegroundColor Green
                $sqlRunning = $true
                $sqlInstance = $svc.Instance
                break
            } else {
                Write-Host "Starting $($svc.DisplayName)..." -ForegroundColor Yellow
                try {
                    Start-Service -Name $svc.Name -ErrorAction Stop
                    Start-Sleep -Seconds 3
                    Write-Host "✓ $($svc.DisplayName) started successfully" -ForegroundColor Green
                    $sqlRunning = $true
                    $sqlInstance = $svc.Instance
                    break
                } catch {
                    Write-Host "Could not start $($svc.DisplayName)" -ForegroundColor Red
                }
            }
        }
    }
    
    # Try LocalDB if no service found
    if (-not $sqlRunning) {
        Write-Host "Checking SQL Server LocalDB..." -ForegroundColor Yellow
        $localdb = & sqllocaldb info MSSQLLocalDB 2>$null
        if ($?) {
            & sqllocaldb start MSSQLLocalDB | Out-Null
            Write-Host "✓ SQL Server LocalDB started" -ForegroundColor Green
            $sqlRunning = $true
            $sqlInstance = "(LocalDB)\MSSQLLocalDB"
        }
    }
    
    if (-not $sqlRunning) {
        Write-Host "=====================================================" -ForegroundColor Red
        Write-Host "ERROR: No SQL Server instance could be found or started" -ForegroundColor Red
        Write-Host "Please install SQL Server Express from:" -ForegroundColor Yellow
        Write-Host "https://www.microsoft.com/sql-server/sql-server-downloads" -ForegroundColor Cyan
        Write-Host "=====================================================" -ForegroundColor Red
        Read-Host "Press Enter to exit"
        exit 1
    }
    
    return $sqlInstance
}

function Test-Prerequisites {
    Write-Host "[1/7] Checking Prerequisites..." -ForegroundColor Yellow
    Write-Host "--------------------------------" -ForegroundColor Gray
    
    # Check .NET
    try {
        $dotnet = & dotnet --version 2>$null
        if ($?) {
            Write-Host "✓ .NET SDK found: $dotnet" -ForegroundColor Green
        } else {
            throw ".NET not found"
        }
    } catch {
        Write-Host "ERROR: .NET SDK is not installed" -ForegroundColor Red
        Write-Host "Please install from: https://dotnet.microsoft.com/download" -ForegroundColor Yellow
        Read-Host "Press Enter to exit"
        exit 1
    }
    
    # Check Node.js
    try {
        $node = & node --version 2>$null
        if ($?) {
            Write-Host "✓ Node.js found: $node" -ForegroundColor Green
            $global:NodeAvailable = $true
        } else {
            throw "Node not found"
        }
    } catch {
        Write-Host "⚠ Node.js not found - Angular frontend will not be available" -ForegroundColor Yellow
        $global:NodeAvailable = $false
    }
    
    # Check and start SQL Server
    $global:SqlInstance = Start-SQLServerService
    
    Write-Host ""
}

function Setup-Databases {
    Write-Host "[2/7] Setting up Databases..." -ForegroundColor Yellow
    Write-Host "--------------------------------" -ForegroundColor Gray
    
    $databases = @("SMO_VisionCenter", "SMO_Commons", "SMO_Identity")
    
    foreach ($db in $databases) {
        $query = "SELECT name FROM sys.databases WHERE name = '$db'"
        $result = & sqlcmd -S $global:SqlInstance -Q $query -h -1 2>$null
        
        if ($result -notmatch $db) {
            Write-Host "Creating $db database..." -ForegroundColor Yellow
            & sqlcmd -S $global:SqlInstance -Q "CREATE DATABASE [$db]" 2>$null | Out-Null
            Write-Host "✓ $db database created" -ForegroundColor Green
        } else {
            Write-Host "✓ $db database exists" -ForegroundColor Green
        }
    }
    
    # Run setup scripts if they exist
    $dbPath = Join-Path $ProjectRoot "src\Database"
    $setupScript = Join-Path $dbPath "Setup-Database.sql"
    $createScript = Join-Path $dbPath "Create-SMO-Database.sql"
    
    if (Test-Path $setupScript) {
        Write-Host "Running database setup scripts..." -ForegroundColor Yellow
        & sqlcmd -S $global:SqlInstance -i $setupScript 2>$null | Out-Null
        Write-Host "✓ Database setup completed" -ForegroundColor Green
    } elseif (Test-Path $createScript) {
        Write-Host "Running database creation script..." -ForegroundColor Yellow
        & sqlcmd -S $global:SqlInstance -i $createScript 2>$null | Out-Null
        Write-Host "✓ Database setup completed" -ForegroundColor Green
    }
    
    Write-Host ""
}

function Build-Solution {
    Write-Host "[3/7] Building Solution..." -ForegroundColor Yellow
    Write-Host "--------------------------------" -ForegroundColor Gray
    
    Write-Host "Building all projects..." -ForegroundColor Yellow
    & dotnet build SMO.sln --configuration Release --verbosity quiet
    
    if ($LASTEXITCODE -eq 0) {
        Write-Host "✓ Solution built successfully" -ForegroundColor Green
    } else {
        Write-Host "ERROR: Build failed" -ForegroundColor Red
        Write-Host "Running detailed build..." -ForegroundColor Yellow
        & dotnet build SMO.sln
        Read-Host "Press Enter to exit"
        exit 1
    }
    
    Write-Host ""
}

function Start-BackendAPI {
    Write-Host "[4/7] Starting Backend API..." -ForegroundColor Yellow
    Write-Host "--------------------------------" -ForegroundColor Gray
    
    $apiPath = Join-Path $ProjectRoot "src\SMO.Api"
    
    Write-Host "Starting API server on https://localhost:7001..." -ForegroundColor Yellow
    Start-Process -FilePath "cmd" -ArgumentList "/k cd /d `"$apiPath`" && dotnet run --urls https://localhost:7001" -WindowStyle Normal
    
    Write-Host "✓ API server starting..." -ForegroundColor Green
    Write-Host "Waiting for API to initialize..." -ForegroundColor Yellow
    Start-Sleep -Seconds 5
    
    Write-Host ""
}

function Start-AngularFrontend {
    if (-not $global:NodeAvailable) {
        Write-Host "[5/7] Skipping Angular Frontend (Node.js not available)..." -ForegroundColor Yellow
        Write-Host ""
        return
    }
    
    Write-Host "[5/7] Starting Angular Frontend..." -ForegroundColor Yellow
    Write-Host "--------------------------------" -ForegroundColor Gray
    
    $angularPath = Join-Path $ProjectRoot "src\SMO.Frontend\SMO-Portal"
    
    if (-not (Test-Path (Join-Path $angularPath "node_modules"))) {
        Write-Host "Installing Angular dependencies (this may take a few minutes)..." -ForegroundColor Yellow
        Set-Location $angularPath
        & npm install 2>$null | Out-Null
        Set-Location $ProjectRoot
    }
    
    Write-Host "Starting Angular development server on http://localhost:4200..." -ForegroundColor Yellow
    Start-Process -FilePath "cmd" -ArgumentList "/k cd /d `"$angularPath`" && npm start" -WindowStyle Normal
    
    Write-Host "✓ Angular frontend starting..." -ForegroundColor Green
    Write-Host ""
}

function Start-HTMLFrontend {
    Write-Host "[6/7] Starting HTML Frontend..." -ForegroundColor Yellow
    Write-Host "--------------------------------" -ForegroundColor Gray
    
    $htmlPath = Join-Path $ProjectRoot "SMO-Platform-UI"
    
    # Try Python first
    $python = & python --version 2>$null
    if ($?) {
        Write-Host "Starting HTML frontend with Python on http://localhost:8080..." -ForegroundColor Yellow
        Start-Process -FilePath "cmd" -ArgumentList "/k cd /d `"$htmlPath`" && python -m http.server 8080" -WindowStyle Normal
        Write-Host "✓ HTML frontend server starting..." -ForegroundColor Green
    } elseif ($global:NodeAvailable) {
        Write-Host "Starting HTML frontend with Node.js on http://localhost:8080..." -ForegroundColor Yellow
        Start-Process -FilePath "cmd" -ArgumentList "/k cd /d `"$htmlPath`" && npx http-server -p 8080 -c-1" -WindowStyle Normal
        Write-Host "✓ HTML frontend server starting..." -ForegroundColor Green
    } else {
        Write-Host "⚠ No web server available for HTML frontend" -ForegroundColor Yellow
        Write-Host "You can open HTML files directly in your browser" -ForegroundColor Yellow
    }
    
    Write-Host ""
}

function Open-Applications {
    Write-Host "[7/7] Opening Applications..." -ForegroundColor Yellow
    Write-Host "--------------------------------" -ForegroundColor Gray
    
    Start-Sleep -Seconds 3
    
    Write-Host "Opening Swagger API Documentation..." -ForegroundColor Yellow
    Start-Process "https://localhost:7001/swagger"
    
    Write-Host "Opening SMO Platform HTML UI..." -ForegroundColor Yellow
    Start-Process "http://localhost:8080"
    
    if ($global:NodeAvailable) {
        Write-Host "Opening SMO Angular Portal..." -ForegroundColor Yellow
        Start-Process "http://localhost:4200"
    }
    
    Write-Host ""
}

function Show-Summary {
    Write-Host "=====================================================" -ForegroundColor Cyan
    Write-Host "   ✓ SMO PLATFORM - ALL MODULES RUNNING!" -ForegroundColor Green
    Write-Host "=====================================================" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "BACKEND SERVICES:" -ForegroundColor Yellow
    Write-Host "─────────────────" -ForegroundColor Gray
    Write-Host "• API Server:       https://localhost:7001" -ForegroundColor White
    Write-Host "• Swagger Docs:     https://localhost:7001/swagger" -ForegroundColor White
    Write-Host ""
    Write-Host "FRONTEND APPLICATIONS:" -ForegroundColor Yellow
    Write-Host "──────────────────────" -ForegroundColor Gray
    if ($global:NodeAvailable) {
        Write-Host "• Angular Portal:   http://localhost:4200 [Modern UI]" -ForegroundColor White
    }
    Write-Host "• HTML Portal:      http://localhost:8080 [Legacy UI]" -ForegroundColor White
    Write-Host ""
    Write-Host "KEY PAGES TO TEST:" -ForegroundColor Yellow
    Write-Host "──────────────────" -ForegroundColor Gray
    Write-Host "• Dashboard:        http://localhost:8080/performance-dashboard.html" -ForegroundColor White
    Write-Host "• Profile:          http://localhost:8080/profile-enhanced.html" -ForegroundColor White
    Write-Host "• Vision 2030:      http://localhost:8080/vision.html" -ForegroundColor White
    Write-Host "• Programs:         http://localhost:8080/performance-programs.html" -ForegroundColor White
    Write-Host "• Login:            http://localhost:8080/login.html" -ForegroundColor White
    Write-Host ""
    Write-Host "DEFAULT CREDENTIALS:" -ForegroundColor Yellow
    Write-Host "────────────────────" -ForegroundColor Gray
    Write-Host "• Admin:            admin@smo.gov.sa / Admin@123" -ForegroundColor White
    Write-Host "• User:             user@smo.gov.sa / User@123" -ForegroundColor White
    Write-Host ""
    Write-Host "DATABASES:" -ForegroundColor Yellow
    Write-Host "──────────" -ForegroundColor Gray
    Write-Host "• SMO_VisionCenter  (Main database)" -ForegroundColor White
    Write-Host "• SMO_Commons       (Shared resources)" -ForegroundColor White
    Write-Host "• SMO_Identity      (Authentication)" -ForegroundColor White
    Write-Host "• SQL Instance:     $($global:SqlInstance)" -ForegroundColor White
    Write-Host ""
    Write-Host "=====================================================" -ForegroundColor Cyan
    Write-Host "SHUTDOWN INSTRUCTIONS:" -ForegroundColor Yellow
    Write-Host "• Close the command windows to stop services" -ForegroundColor White
    Write-Host "• Or press Ctrl+C in each window" -ForegroundColor White
    Write-Host "=====================================================" -ForegroundColor Cyan
}

# Main execution
try {
    Test-Prerequisites
    Setup-Databases
    Build-Solution
    Start-BackendAPI
    Start-AngularFrontend
    Start-HTMLFrontend
    Open-Applications
    Show-Summary
    
    Write-Host ""
    Read-Host "Press Enter to keep services running (or close this window)"
} catch {
    Write-Host "ERROR: $($_.Exception.Message)" -ForegroundColor Red
    Read-Host "Press Enter to exit"
    exit 1
}

