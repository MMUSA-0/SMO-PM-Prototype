# Role Badge Integration Guide

## Overview
This guide explains how to add the persistent role badge to all pages in the SMO platform. The role badge appears in the header and allows users to switch between their assigned roles seamlessly.

## Quick Integration (For All Pages)

### Method 1: Add to Every Page (Recommended)

Add this single line to every HTML page before the closing `</body>` tag:

```html
<script src="assets/js/role-manager.js"></script>
```

That's it! The role manager will automatically:
- Detect if the page has a unified header or not
- Inject the role badge in the appropriate location
- Handle role switching functionality
- Persist the selected role across pages

### Method 2: Add to Template Files

If your pages use a common template, add the script reference to your base template:

```html
<!-- Before </body> tag -->
<script src="assets/js/role-manager.js"></script>
```

## How It Works

### For Pages WITH Unified Header
The role badge appears between the logo and the search/notification icons:

```
[Logo] [Sidebar Toggle] → [ROLE BADGE] → [Search] [Notifications] [User Menu]
```

### For Pages WITHOUT Unified Header
The role badge appears as a floating element in the top-left corner:

```
[Floating Role Badge]
    ↓
[Role Switcher Dropdown]
```

## Features

### 1. Visual Role Indicator
- Shows current active role with Arabic and English names
- Color-coded based on role type
- Animated hover effects

### 2. Quick Role Switching
- Click badge to see available roles
- One-click role switching
- Smooth animations and transitions
- Success notifications

### 3. Persistent Selection
- Selected role saved in localStorage
- Persists across page refreshes
- Maintains selection across different pages

### 4. Available Roles

| Role | Arabic Name | English Name | Color |
|------|------------|--------------|-------|
| SMO | مكتب الإدارة الاستراتيجية | Strategic Management Office | Green |
| ExecutiveOffice | المكتب التنفيذي | Executive Office | Blue |
| VRO | مكتب تحقيق الرؤية | Vision Realization Office | Cyan |
| VRP | برنامج تحقيق الرؤية | Vision Realization Program | Yellow |
| ADAA | المركز الوطني لقياس الأداء | ADAA | Red |
| PerformanceManager | مدير الأداء | Performance Manager | Gray |

## Example Integration

### For Existing Pages

```html
<!DOCTYPE html>
<html lang="ar" dir="rtl">
<head>
    <!-- Your existing head content -->
</head>
<body>
    <!-- Your page content -->
    
    <!-- Add this line before closing body -->
    <script src="assets/js/role-manager.js"></script>
</body>
</html>
```

### For New Pages Using Unified Template

```html
<!DOCTYPE html>
<html lang="ar" dir="rtl">
<head>
    <!-- Standard head content -->
    <link rel="stylesheet" href="css/bootstrap.rtl.min.css">
    <link rel="stylesheet" href="css/styles.min.css">
    <link rel="stylesheet" href="assets/css/sidebar-unified.css">
</head>
<body>
    <!-- Standard header with unified sidebar -->
    
    <!-- Scripts -->
    <script src="js/jquery-3.7.1.min.js"></script>
    <script src="js/bootstrap.bundle.min.js"></script>
    <script src="assets/js/sidebar-manager.js"></script>
    <script src="assets/js/role-manager.js"></script> <!-- Add this -->
</body>
</html>
```

## Customization

### Change Default Role
Edit `role-manager.js` line 8:
```javascript
this.currentRole = localStorage.getItem('currentRole') || 'SMO'; // Change 'SMO' to default role
```

### Add/Remove Roles
Edit the `loadUserRoles()` function in `role-manager.js`:
```javascript
loadUserRoles() {
    return [
        { 
            name: 'NewRole', 
            displayNameAr: 'الدور الجديد',
            displayNameEn: 'New Role',
            color: '#yourcolor'
        },
        // ... other roles
    ];
}
```

### Styling Customization
The role badge styles are embedded in the JavaScript for portability. To customize colors, sizes, or animations, edit the style definitions in `role-manager.js`.

## API Integration

When the backend is ready, the role switching will automatically call:
```javascript
POST /api/UserProfile/switch-role
{
    "roleName": "VRO"
}
```

The system will handle JWT token refresh and re-authentication as needed.

## Browser Support

- Chrome 90+
- Firefox 88+
- Safari 14+
- Edge 90+

## Troubleshooting

### Badge Not Appearing
1. Check browser console for errors
2. Ensure `role-manager.js` is loaded after jQuery/Bootstrap
3. Verify path to script is correct

### Role Not Persisting
1. Check localStorage is enabled in browser
2. Clear browser cache and reload
3. Ensure same domain/protocol

### Styling Issues
1. Check for CSS conflicts with existing styles
2. Ensure Bootstrap is loaded before role manager
3. Try increasing z-index values if badge is hidden

## Testing Checklist

- [ ] Badge appears on all pages
- [ ] Click badge opens role switcher
- [ ] Roles display correctly in Arabic/English  
- [ ] Clicking role switches successfully
- [ ] Selected role persists on page refresh
- [ ] Selected role persists across different pages
- [ ] Notification appears after switching
- [ ] Clicking outside closes dropdown
- [ ] Works on mobile devices
- [ ] Works with and without unified header

## Support

For issues or questions:
1. Check browser console for errors
2. Verify all required scripts are loaded
3. Test in incognito/private mode to rule out cache issues
4. Contact the development team with specific error messages

## Summary

Adding role management to any page is as simple as including one script file. The system handles all the complexity of:
- Detecting page structure
- Injecting appropriate UI
- Managing role state
- Handling user interactions
- Persisting selections

This ensures a consistent role management experience across the entire SMO platform.


