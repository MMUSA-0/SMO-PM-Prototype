# SMO Platform - Quick Manual Start Guide

## ⚡ Quick Start (Without SQL Server)

If SQL Server isn't working, you can still run the frontend:

### Option 1: Simple Frontend Only
```bash
# Open Command Prompt and run:
cd "H:\My Drive\SMO ( PM Prototype)\SMO-Platform-UI"
python -m http.server 8080

# Then open: http://localhost:8080
```

### Option 2: Start Everything Step by Step

## 📋 Step 1: Start SQL Server Manually

### Check if SQL Server is Installed
Open Command Prompt as Administrator and try:

```cmd
# Try these commands one by one:
net start MSSQLSERVER
net start MSSQL$SQLEXPRESS
net start MSSQL$SQLEXPRESS01
```

### If SQL Server is NOT installed:
1. Download SQL Server Express (Free): https://www.microsoft.com/sql-server/sql-server-downloads
2. Install with default settings
3. Restart your computer

### Alternative: Use SQLite (No Installation)
If you can't install SQL Server, the app can work with mock data.

## 📋 Step 2: Start Backend API

Open a new Command Prompt:
```cmd
cd "H:\My Drive\SMO ( PM Prototype)\src\SMO.Api"
dotnet run --urls https://localhost:7001
```

Keep this window open!

## 📋 Step 3: Start Frontend (HTML Version)

Open another Command Prompt:
```cmd
cd "H:\My Drive\SMO ( PM Prototype)\SMO-Platform-UI"

# If you have Python:
python -m http.server 8080

# OR if you have Node.js:
npx http-server -p 8080 -c-1
```

## 📋 Step 4: Open in Browser

Open these URLs:
- Frontend: http://localhost:8080
- API Docs: https://localhost:7001/swagger
- Main Dashboard: http://localhost:8080/performance-dashboard.html
- Login Page: http://localhost:8080/login.html

## 🔐 Login Credentials
- Admin: admin@smo.gov.sa / Admin@123
- User: user@smo.gov.sa / User@123

## 🚀 Optional: Install Missing Components

### Install Node.js (for Angular frontend)
1. Download from: https://nodejs.org/
2. Install LTS version
3. Restart Command Prompt

### Install Python (for simple web server)
1. Download from: https://www.python.org/downloads/
2. Install with "Add to PATH" checked
3. Restart Command Prompt

## 🛠️ Troubleshooting

### "dotnet" is not recognized
- Install .NET 8.0 SDK: https://dotnet.microsoft.com/download

### "python" is not recognized
- Install Python or use Node.js instead

### SQL Server connection failed
- The frontend will still work with mock data
- API might show errors but basic functionality remains

### Port already in use
- Change 8080 to another port like 8081, 8082, etc.

## 📱 Test Without Backend
You can test the frontend without any backend:
1. Open File Explorer
2. Navigate to: H:\My Drive\SMO ( PM Prototype)\SMO-Platform-UI
3. Double-click any .html file to open in browser

Key files to test:
- index.html
- performance-dashboard.html
- vision.html
- login.html

## 🎯 Minimal Requirements
- **Essential**: Web browser (Chrome, Edge, Firefox)
- **Recommended**: Python or Node.js for web server
- **Optional**: SQL Server for full functionality

