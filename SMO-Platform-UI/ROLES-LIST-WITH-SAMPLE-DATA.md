# List of Possible Roles with Sample Data

## Top 5 Roles with Sample Real Data

### 1. Executive Office (المكتب التنفيذي)
**Role Code:** `ExecutiveOffice`  
**Role Group:** Executive  
**Color Code:** `#0d6efd` (Blue)

**Sample Data:**
```json
{
  "name": "ExecutiveOffice",
  "code": 10,
  "displayNameAr": "المكتب التنفيذي",
  "displayNameEn": "Executive Office",
  "descriptionAr": "المكتب التنفيذي مع كامل الصلاحيات الإدارية",
  "descriptionEn": "Executive Office with full administrative privileges",
  "roleGroup": "Executive",
  "isDefault": false,
  "permissions": [
    "Full system access",
    "User management",
    "Role assignment",
    "All program oversight",
    "Report approval"
  ]
}
```

---

### 2. Vision Realization Office - VRO (مكتب تحقيق الرؤية)
**Role Code:** `VRO`  
**Role Group:** Strategic  
**Color Code:** `#0dcaf0` (Cyan)

**Sample Data:**
```json
{
  "name": "VRO",
  "code": 11,
  "displayNameAr": "مكتب تحقيق الرؤية",
  "displayNameEn": "Vision Realization Office",
  "descriptionAr": "مكتب تحقيق الرؤية للإشراف الاستراتيجي",
  "descriptionEn": "Vision Realization Office for strategic oversight",
  "roleGroup": "Strategic",
  "isDefault": false,
  "permissions": [
    "Strategic oversight of Vision 2030 programs",
    "Program review and approval",
    "Strategic planning access",
    "High-level reporting",
    "Cross-program coordination"
  ]
}
```

---

### 3. Strategic Management Office - SMO (مكتب الإدارة الاستراتيجية)
**Role Code:** `SMO`  
**Role Group:** Management  
**Color Code:** `#198754` (Green)

**Sample Data:**
```json
{
  "name": "SMO",
  "code": 13,
  "displayNameAr": "مكتب الإدارة الاستراتيجية",
  "displayNameEn": "Strategic Management Office",
  "descriptionAr": "مكتب الإدارة الاستراتيجية لإدارة الأداء",
  "descriptionEn": "Strategic Management Office for performance management",
  "roleGroup": "Management",
  "isDefault": true,
  "permissions": [
    "Performance management",
    "Strategic planning",
    "KPI management",
    "Report generation",
    "Program coordination"
  ]
}
```

---

### 4. Vision Realization Program - VRP (برنامج تحقيق الرؤية)
**Role Code:** `VRP`  
**Role Group:** Program  
**Color Code:** `#ffc107` (Yellow/Amber)

**Sample Data:**
```json
{
  "name": "VRP",
  "code": 12,
  "displayNameAr": "برنامج تحقيق الرؤية",
  "displayNameEn": "Vision Realization Program",
  "descriptionAr": "إدارة برنامج تحقيق الرؤية",
  "descriptionEn": "Vision Realization Program management",
  "roleGroup": "Program",
  "isDefault": false,
  "permissions": [
    "Program management",
    "Initiative oversight",
    "Milestone tracking",
    "Budget management",
    "Program reporting"
  ]
}
```

---

### 5. National Center for Performance Management - ADAA (المركز الوطني لقياس الأداء)
**Role Code:** `ADAA`  
**Role Group:** Performance  
**Color Code:** `#dc3545` (Red)

**Sample Data:**
```json
{
  "name": "ADAA",
  "code": 14,
  "displayNameAr": "المركز الوطني لقياس الأداء (أداء)",
  "displayNameEn": "National Center for Performance Management (ADAA)",
  "descriptionAr": "المركز الوطني لقياس أداء الأجهزة العامة",
  "descriptionEn": "National Center for Government Performance Management",
  "roleGroup": "Performance",
  "isDefault": false,
  "permissions": [
    "Government performance measurement",
    "KPI monitoring",
    "Performance reporting",
    "Benchmarking",
    "Performance analytics"
  ]
}
```

---

## Additional Roles Available in System

### 6. Performance Manager (مدير الأداء)
**Role Code:** `PerformanceManager`  
**Role Group:** Performance  
**Color Code:** `#6c757d` (Gray)

**Sample Data:**
```json
{
  "name": "PerformanceManager",
  "code": 17,
  "displayNameAr": "مدير الأداء",
  "displayNameEn": "Performance Manager",
  "descriptionAr": "يدير تقييمات الأداء ومؤشرات الأداء الرئيسية",
  "descriptionEn": "Manages performance evaluations and KPIs",
  "roleGroup": "Performance",
  "isDefault": false
}
```

### 7. Program Owner (مالك البرنامج)
**Role Code:** `ProgramOwner`  
**Role Group:** Program

**Sample Data:**
```json
{
  "name": "ProgramOwner",
  "code": 15,
  "displayNameAr": "مالك البرنامج",
  "displayNameEn": "Program Owner",
  "descriptionAr": "مالك برامج رؤية 2030 المحددة",
  "descriptionEn": "Owner of specific Vision 2030 programs",
  "roleGroup": "Program",
  "isDefault": false
}
```

### 8. Initiative Owner (مالك المبادرة)
**Role Code:** `InitiativeOwner`  
**Role Group:** Initiative

**Sample Data:**
```json
{
  "name": "InitiativeOwner",
  "code": 16,
  "displayNameAr": "مالك المبادرة",
  "displayNameEn": "Initiative Owner",
  "descriptionAr": "مالك المبادرات المحددة",
  "descriptionEn": "Owner of specific initiatives",
  "roleGroup": "Initiative",
  "isDefault": false
}
```

### 9. Data Analyst (محلل البيانات)
**Role Code:** `DataAnalyst`  
**Role Group:** Analytics

**Sample Data:**
```json
{
  "name": "DataAnalyst",
  "code": 18,
  "displayNameAr": "محلل البيانات",
  "displayNameEn": "Data Analyst",
  "descriptionAr": "يحلل بيانات الأداء وينشئ التقارير",
  "descriptionEn": "Analyzes performance data and generates reports",
  "roleGroup": "Analytics",
  "isDefault": false
}
```

---

## Complete Role List Summary

| # | Role Code | English Name | Arabic Name | Role Group | Code |
|---|-----------|--------------|-------------|------------|------|
| 1 | ExecutiveOffice | Executive Office | المكتب التنفيذي | Executive | 10 |
| 2 | VRO | Vision Realization Office | مكتب تحقيق الرؤية | Strategic | 11 |
| 3 | VRP | Vision Realization Program | برنامج تحقيق الرؤية | Program | 12 |
| 4 | SMO | Strategic Management Office | مكتب الإدارة الاستراتيجية | Management | 13 |
| 5 | ADAA | National Center for Performance Management | المركز الوطني لقياس الأداء | Performance | 14 |
| 6 | ProgramOwner | Program Owner | مالك البرنامج | Program | 15 |
| 7 | InitiativeOwner | Initiative Owner | مالك المبادرة | Initiative | 16 |
| 8 | PerformanceManager | Performance Manager | مدير الأداء | Performance | 17 |
| 9 | DataAnalyst | Data Analyst | محلل البيانات | Analytics | 18 |

---

## Sample User with Multiple Roles

**Test User Example:**
```json
{
  "userId": "mohammed.ahmed",
  "fullNameEn": "Mohammed Ahmed Al-Salem",
  "fullNameAr": "محمد أحمد السالم",
  "email": "mohammed.ahmed@smo.gov.sa",
  "titleEn": "Performance Management Director",
  "titleAr": "مدير إدارة الأداء",
  "assignedRoles": [
    {
      "name": "SMO",
      "isActive": true
    },
    {
      "name": "VRO",
      "isActive": false
    },
    {
      "name": "PerformanceManager",
      "isActive": false
    },
    {
      "name": "ADAA",
      "isActive": false
    }
  ],
  "currentActiveRole": "SMO"
}
```

---

## Notes

- **VRS** mentioned in your query might refer to **VRP** (Vision Realization Program) or **VRO** (Vision Realization Office)
- **Crop** was not found in the codebase - this might be a custom role or typo
- All roles support bilingual display (Arabic and English)
- Roles are organized by Role Groups for better management
- Users can have multiple roles and switch between them
- The default role is typically **SMO** (Strategic Management Office)

---

## Source Files

- Role Definitions: `src/Database/Add-SMO-Roles.sql`
- Frontend Role Manager: `SMO-Platform-UI/assets/js/role-manager.js`
- Documentation: `docs/USER-PROFILE-ROLE-MANAGEMENT.md`
- Migration: `src/Framework/Framework.Identity/Migrations/20241207_SeedSMORoles.cs`


