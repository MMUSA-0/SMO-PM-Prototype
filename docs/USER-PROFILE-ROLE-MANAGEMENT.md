# User Profile & Role Management Feature

## Overview

This document describes the complete implementation of the user profile management and role switching feature for the SMO Platform. The feature allows users to have multiple roles and switch between them based on their responsibilities.

## Roles Implemented

The following roles have been added to the system:

### Executive Roles
1. **Executive Office (المكتب التنفيذي)**
   - Full administrative privileges
   - Can manage all aspects of the system
   - Role Name: `ExecutiveOffice`

### Strategic Roles  
2. **Vision Realization Office - VRO (مكتب تحقيق الرؤية)**
   - Strategic oversight of Vision 2030 programs
   - Role Name: `VRO`

3. **Vision Realization Program - VRP (برنامج تحقيق الرؤية)**
   - Management of Vision realization programs
   - Role Name: `VRP`

### Management Roles
4. **Strategic Management Office - SMO (مكتب الإدارة الاستراتيجية)**
   - Performance management and strategic planning
   - Role Name: `SMO`

### Performance Roles
5. **National Center for Performance Management - ADAA (المركز الوطني لقياس الأداء)**
   - Government performance measurement
   - Role Name: `ADAA`

6. **Performance Manager (مدير الأداء)**
   - Manages performance evaluations and KPIs
   - Role Name: `PerformanceManager`

### Program & Initiative Roles
7. **Program Owner (مالك البرنامج)**
   - Ownership of specific Vision 2030 programs
   - Role Name: `ProgramOwner`

8. **Initiative Owner (مالك المبادرة)**
   - Ownership of specific initiatives
   - Role Name: `InitiativeOwner`

### Analytics Roles
9. **Data Analyst (محلل البيانات)**
   - Analyzes performance data and generates reports
   - Role Name: `DataAnalyst`

## Implementation Components

### Backend Components

#### 1. Database
- **Migration File**: `src/Framework/Framework.Identity/Migrations/20241207_SeedSMORoles.cs`
- **SQL Script**: `src/Database/Add-SMO-Roles.sql`
- Tables Modified:
  - `Roles` - Added SMO-specific roles
  - `Users` - Existing ApplicationUser entity
  - `UserRoles` - Junction table for user-role assignments

#### 2. API Controller
- **File**: `src/SMO.Api/Controllers/UserProfileController.cs`
- **Endpoints**:
  ```
  GET  /api/UserProfile/current           - Get current user profile with roles
  PUT  /api/UserProfile/update            - Update user profile information
  POST /api/UserProfile/switch-role       - Switch active role
  GET  /api/UserProfile/available-roles   - Get all available roles (Admin only)
  POST /api/UserProfile/{userId}/assign-roles - Assign roles to user (Admin only)
  ```

#### 3. Entities & DTOs
- Uses existing `ApplicationUser` entity from Framework.Identity
- Uses existing `ApplicationRole` entity with Arabic/English support
- DTOs:
  - `UpdateProfileDto` - For profile updates
  - `SwitchRoleDto` - For role switching
  - `AssignRolesDto` - For role assignment

### Frontend Components

#### 1. Enhanced Profile Page
- **File**: `SMO-Platform-UI/profile-enhanced.html`
- Features:
  - View current profile information
  - Edit profile details
  - Display all assigned roles
  - Switch between active roles
  - View assigned programs and initiatives
  - Upload profile photo

#### 2. Role Switcher Modal
- Interactive role selection interface
- Shows role descriptions in Arabic and English
- Visual indicators for current active role
- Confirmation before switching

## Setup Instructions

### 1. Database Setup

Run the following SQL scripts in order:

```bash
# Navigate to database folder
cd src/Database

# Run the main database creation (if not already done)
sqlcmd -S localhost -d master -i Create-SMO-Database.sql

# Add SMO-specific roles
sqlcmd -S localhost -d SMO_Production -i Add-SMO-Roles.sql
```

Or use the provided batch file:
```bash
Setup-Database.bat
```

### 2. Backend Setup

```bash
# Navigate to API project
cd src/SMO.Api

# Restore packages
dotnet restore

# Run migrations (if using EF Core)
dotnet ef database update

# Run the API
dotnet run
```

### 3. Frontend Setup

```bash
# Navigate to UI folder
cd SMO-Platform-UI

# Start a local server
python -m http.server 8080

# Access the enhanced profile page
http://localhost:8080/profile-enhanced.html
```

## API Usage Examples

### Get Current User Profile
```javascript
const response = await fetch('/api/UserProfile/current', {
    method: 'GET',
    headers: {
        'Authorization': 'Bearer ' + token
    }
});
const profile = await response.json();
```

### Update Profile
```javascript
const updateData = {
    fullNameAr: 'محمد أحمد',
    fullNameEn: 'Mohammed Ahmed',
    phoneNumber: '+966501234567'
};

const response = await fetch('/api/UserProfile/update', {
    method: 'PUT',
    headers: {
        'Content-Type': 'application/json',
        'Authorization': 'Bearer ' + token
    },
    body: JSON.stringify(updateData)
});
```

### Switch Role
```javascript
const response = await fetch('/api/UserProfile/switch-role', {
    method: 'POST',
    headers: {
        'Content-Type': 'application/json',
        'Authorization': 'Bearer ' + token
    },
    body: JSON.stringify({ roleName: 'VRO' })
});
```

## Role-Based Access Control

The system implements role-based access control with the following hierarchy:

1. **SuperAdmin** - System administrator (existing role)
2. **ExecutiveOffice** - Full executive privileges
3. **VRO/VRP** - Strategic oversight roles
4. **SMO** - Management roles
5. **ADAA** - Performance measurement roles
6. **Program/Initiative Owners** - Specific ownership roles
7. **PerformanceManager** - Performance management
8. **DataAnalyst** - Analytics and reporting

## Security Considerations

1. **Authentication Required**: All endpoints require authentication
2. **Role Validation**: System validates user has the role before switching
3. **Token Refresh**: After role switch, new JWT token should be issued
4. **Admin Functions**: Role assignment restricted to Admin/Executive roles
5. **Audit Trail**: All role changes are logged

## Testing

### Test User Created
- **Username**: mohammed.ahmed
- **Email**: mohammed.ahmed@smo.gov.sa
- **Password**: P@ssw0rd123 (for testing only)
- **Assigned Roles**: SMO, VRO, PerformanceManager, ADAA

### Test Scenarios
1. Login as test user
2. Navigate to profile page
3. View all assigned roles
4. Switch between roles
5. Edit profile information
6. Save changes

## Frontend Integration Points

### Existing Pages to Update
1. **index.html** - Add link to enhanced profile page
2. **navigation** - Update user dropdown to show current role
3. **sidebar** - Filter menu items based on active role
4. **dashboard** - Customize widgets based on role

### API Service Integration
The profile page uses the existing `api-service.js` for API calls:
```javascript
// Already integrated in profile-enhanced.html
apiService.get('/api/UserProfile/current')
apiService.put('/api/UserProfile/update', data)
apiService.post('/api/UserProfile/switch-role', data)
```

## Known Limitations

1. **JWT Token Refresh**: Currently requires re-login after role switch
2. **Photo Upload**: Profile photo upload stores locally, needs backend integration
3. **Program/Initiative Assignment**: Mock data used, needs database integration
4. **Role Permissions**: Detailed permission matrix not yet implemented

## Future Enhancements

1. **Real-time Role Switch**: Implement JWT refresh without re-login
2. **Permission Matrix**: Define detailed permissions per role
3. **Role Request Workflow**: Allow users to request additional roles
4. **Delegation Feature**: Temporary role delegation to other users
5. **Role Analytics**: Track role usage and access patterns
6. **Mobile Support**: Optimize role switcher for mobile devices

## Support

For issues or questions regarding the user profile and role management feature:
1. Check the API logs at `src/SMO.Api/Logs/`
2. Review browser console for frontend errors
3. Ensure all database migrations have been applied
4. Verify user has proper role assignments in database

## Conclusion

The user profile and role management feature provides a comprehensive solution for managing multiple user roles in the SMO platform. Users can easily switch between their assigned roles, with the interface and permissions updating accordingly. The implementation leverages existing authentication infrastructure while adding SMO-specific business roles.


