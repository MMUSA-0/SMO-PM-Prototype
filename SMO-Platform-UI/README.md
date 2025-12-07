# SMO Platform UI - Development Guide

## Quick Start

### Frontend Setup

1. **Open any HTML file directly in your browser**
   - Simply double-click any HTML file (e.g., `index.html`)
   - Or use a local server like Live Server in VS Code
   - No build process required!

2. **Authentication is bypassed in development mode**
   - On the login page, click the yellow "دخول وضع التطوير (Development Mode)" button
   - Or navigate directly to any page - authentication is automatically bypassed

### Backend Setup (Optional)

The frontend can work without the backend, but if you want to connect to the API:

1. **Navigate to the API project:**
   ```bash
   cd src/SMO.Api
   ```

2. **Run the backend:**
   ```bash
   dotnet run
   ```
   The API will start at `https://localhost:7001`

3. **The backend automatically bypasses authentication in Development mode**
   - No need to configure anything
   - Mock user with all roles is automatically created

## File Structure

```
SMO-Platform-UI/
├── index.html                    # Main landing page with all links
├── login.html                    # Login page (with dev bypass button)
├── performance-dashboard.html    # Main dashboard
├── performance-programs.html     # Program management
├── program-wizard.html          # Program creation wizard
├── performance-initiatives.html  # Initiative management
├── performance-vision.html       # Vision level management
├── performance-approvals.html   # Approval workflow
├── performance-requests.html    # User requests
├── report-generation.html       # Report generation
├── data-sync-status.html        # Data synchronization status
├── js/
│   ├── api-service.js          # API connection service (with bypass)
│   └── scripts.js              # General scripts
└── css/                        # Styles
```

## Features

### Authentication Bypass (Development Only)

- **Frontend**: Automatically provides dummy tokens
- **Backend**: Automatically authenticates all requests in Development environment
- **Configuration**: Set `DevelopmentSettings:BypassAuthentication` to `false` in `appsettings.Development.json` to disable

### All Pages Are Connected

Every HTML page includes:
- Navigation links to all other pages
- API service for backend connectivity
- Automatic authentication bypass in development

### API Endpoints

The system connects to these backend endpoints:
- `/api/health` - Health check (always works)
- `/api/performance/programperformance` - Program management
- `/api/performance/initiatives` - Initiative management
- `/api/performance/dashboard` - Dashboard data
- `/api/reports/generate` - Report generation
- And more...

## Testing the System

1. **Test without backend:**
   - Open `index.html` in your browser
   - Navigate through all pages
   - All static content and UI will work

2. **Test with backend:**
   - Start the backend API
   - Open `index.html`
   - Check the API status indicator (bottom right)
   - Green = Connected, Red = Disconnected

3. **Test specific features:**
   - Programs: Go to `performance-programs.html`
   - Create Program: Use `program-wizard.html`
   - Reports: Use `report-generation.html`
   - Dashboard: View `performance-dashboard.html`

## Production Deployment

**IMPORTANT**: Before deploying to production:

1. **Disable authentication bypass:**
   - Set `DevelopmentSettings:BypassAuthentication` to `false`
   - Or remove the entire `DevelopmentSettings` section

2. **Update API URLs:**
   - Edit `js/api-service.js`
   - Change `BASE_URL` to your production API URL

3. **Enable real authentication:**
   - Integrate with Nafath (النفاذ الوطني الموحد)
   - Remove development bypass code from `login.html`

4. **Update CORS settings:**
   - Configure proper CORS origins in `appsettings.json`
   - Remove development origins like `file://` and `null`

## Troubleshooting

### API Not Connecting?
- Check if backend is running at `https://localhost:7001`
- Check browser console for errors
- Verify CORS settings in `appsettings.Development.json`

### Pages Not Loading?
- Ensure all files are in the same directory structure
- Check browser console for 404 errors
- Verify all CSS and JS files are present

### Authentication Issues?
- In development, just click the yellow bypass button
- Make sure `BypassAuthentication` is `true` in backend config
- Clear browser localStorage and try again

## Support

For issues or questions, please contact the SMO Development Team.