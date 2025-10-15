# 3. Technology Stack

## 3.1 Backend Technologies

| Category | Technology | Version | Purpose |
|----------|-----------|---------|---------|
| **Runtime** | .NET | 8.0 | Application framework |
| **API Framework** | ASP.NET Core | 8.0 | Web API hosting |
| **ORM** | Entity Framework Core | 9.0.8 | Data access |
| **Database** | SQL Server | - | Primary database |
| **Authentication** | ASP.NET Core Identity | 8.0.7 | User authentication |
| **JWT** | Microsoft.AspNetCore.Authentication.JwtBearer | 8.0.20 | JWT token authentication |
| **Validation** | FluentValidation | 11.9.2 | Input validation |
| **Mapping** | AutoMapper | 13.0.1 | Object-to-object mapping |
| **Background Jobs** | Hangfire | 1.8.14 | Background processing |
| **Logging** | NLog | 5.3.2 | Application logging |
| **API Documentation** | Swashbuckle (Swagger) | 6.6.2 | API documentation |
| **Real-time Communication** | SignalR | 9.0.6 | WebSocket-based real-time messaging |
| **JSON Serialization** | Newtonsoft.Json | 13.0.3 | JSON handling |

## 3.2 Framework Core Dependencies

**Document Generation:**
- `itext` (8.0.5) - PDF manipulation
- `iTextSharp` (5.5.13.4) - Legacy PDF support
- `Select.HtmlToPdf.NetCore` (24.1.0) - HTML to PDF conversion
- `EPPlus` (7.2.2) - Excel generation
- `ExcelDataReader` (3.7.0) - Excel reading

**Utilities:**
- `libphonenumber-csharp` (8.13.42) - Phone number validation
- `ZXing.Net` (0.16.9) - Barcode/QR code generation
- `PagedList.Core` (1.17.4) - Pagination support
- `System.Linq.Dynamic.Core` (1.4.4) - Dynamic LINQ queries

## 3.3 Frontend Technologies

| Category | Technology | Version | Purpose |
|----------|-----------|---------|---------|
| **Framework** | Angular | 18.2.13 | SPA framework |
| **Language** | TypeScript | 5.4.5 | Type-safe JavaScript |
| **Build Tool** | Angular CLI | 18.2.13 | Build & development |
| **Testing** | Jasmine + Karma | 4.5.0 / 6.4.0 | Unit testing |
| **Reactive Programming** | RxJS | 7.5.0 | Async data streams |

## 3.4 Frontend UI & Integration Libraries

**UI Component Libraries:**
- `@angular/material` (20.2.0) - Material Design components
- `@ng-bootstrap/ng-bootstrap` (17.0.1) - Bootstrap components
- `ngx-bootstrap` (20.0.1) - Additional Bootstrap widgets
- `@ng-select/ng-select` (12.0.7) - Advanced select dropdowns
- `ng-multiselect-dropdown` (1.0.0) - Multi-select component
- `ng-select2-component` (17.2.7) - Select2 integration

**UI Utilities:**
- `@angular/flex-layout` (15.0.0-beta.42) - Flexbox layouts
- `@fortawesome/fontawesome-free` (7.0.1) - Icon library
- `ngx-bootstrap-icons` (1.9.3) - Bootstrap icons
- `sweetalert2` (11.22.4) - Beautiful alerts/modals

**Rich Text & Data Visualization:**
- `ngx-quill` (28.0.1) - Rich text editor (Quill integration)
- `quill` (2.0.3) - Quill editor core
- `ng2-charts` (8.0.0) - Chart.js wrapper
- `ngx-tagify` (18.0.0) - Tag input component

**File Handling:**
- `exceljs` (4.4.0) - Excel file generation
- `file-saver` (2.0.5) - Client-side file saving
- `ngx-filesaver` (20.0.0) - Angular file saver
- `@pdftron/webviewer` (11.7.0) - PDF viewer/editor

**Authentication & Security:**
- `@auth0/angular-jwt` (5.2.0) - JWT helper utilities
- `crypto-js` (4.2.0) - Cryptographic operations
- `localstorage-slim` (2.7.1) - Enhanced localStorage

**Internationalization:**
- `@ngx-translate/core` (14.0.0) - Translation framework
- `@ngx-translate/http-loader` (8.0.0) - HTTP translation loader
- `hijri-converter` (1.1.1) - Hijri calendar conversion

**Real-time & Media:**
- `@microsoft/signalr` (9.0.6) - SignalR client
- `ngx-webcam` (0.4.1) - Webcam capture
- `ngx-print` (20.0.0) - Print functionality

**Notifications & Feedback:**
- `ngx-toastr` (19.0.0) - Toast notifications
- `ngx-toaster` (1.0.1) - Alternative toaster

**Utilities:**
- `underscore` (1.13.7) - Utility functions
- `@types/select2` (4.0.63) - TypeScript definitions

**Build Configurations:**
- Development (with source maps, no optimization)
- Testing (optimized, environment-specific)
- Staging (optimized, environment-specific)
- Production (fully optimized, hashed output, 2MB/5MB budgets)

---
