# SMO Vision Center - Strategic Management Office

> **Vision 2030 Strategic Management System**
> A comprehensive platform for managing strategic objectives, initiatives, KPIs, and performance monitoring aligned with Saudi Vision 2030.

---

## Table of Contents

- [Overview](#overview)
- [Architecture](#architecture)
- [Technology Stack](#technology-stack)
- [Prerequisites](#prerequisites)
- [Getting Started](#getting-started)
- [Project Structure](#project-structure)
- [Configuration](#configuration)
- [Database Setup](#database-setup)
- [Running the Application](#running-the-application)
- [Building for Production](#building-for-production)
- [Deployment](#deployment)
- [Contributing](#contributing)

---

## Overview

The SMO Vision Center is a strategic management platform designed to align organizational objectives with Saudi Vision 2030. The system provides comprehensive tools for:

- **Strategic Planning**: Define and manage strategic pillars, themes, and objectives
- **Initiative Management**: Track programs and initiatives with milestones and deliverables
- **KPI Monitoring**: Real-time performance indicators with dashboards and alerts
- **Reporting**: Generate comprehensive reports on strategic progress
- **Collaboration**: Multi-user access with role-based permissions

---

## Architecture

This project implements **Clean Architecture** (Onion Architecture) principles with clear separation of concerns:

```
┌─────────────────────────────────────────────────┐
│                 SMO.Api (API Layer)             │
│         Controllers, Middleware, Config         │
└────────────────────┬────────────────────────────┘
                     │
┌────────────────────▼────────────────────────────┐
│      SMO.Application (Application Layer)        │
│    Services, DTOs, Business Logic, Validation   │
└────────────────────┬────────────────────────────┘
                     │
┌────────────────────▼────────────────────────────┐
│         SMO.Domain (Domain Layer)               │
│    Entities, Interfaces, Enums, Domain Logic    │
└────────────────────┬────────────────────────────┘
                     │
┌────────────────────▼────────────────────────────┐
│    SMO.Infrastructure (Infrastructure Layer)    │
│  DbContext, Repositories, External Integrations │
└─────────────────────────────────────────────────┘

┌─────────────────────────────────────────────────┐
│           Framework Projects (Shared)           │
│ Framework.Core | Framework.Identity | Resources │
└─────────────────────────────────────────────────┘
```

### Architectural Principles

- **Domain-Centric Design**: Domain layer contains business entities and rules
- **Dependency Inversion**: Dependencies point inward toward domain
- **Technology Independence**: Business logic independent of frameworks
- **Testability**: Each layer can be tested independently
- **No CQRS**: Simplified architecture per project requirements

---

## Technology Stack

### Backend (.NET 8)

| Technology | Version | Purpose |
|------------|---------|---------|
| .NET | 8.0 | Core framework |
| ASP.NET Core | 8.0 | Web API framework |
| Entity Framework Core | 8.0.11 | ORM for data access |
| ASP.NET Core Identity | 8.0.7 | Authentication/Authorization |
| JWT Bearer | 8.0.20 | Token-based authentication |
| AutoMapper | 12.0.1 | Object-to-object mapping |
| FluentValidation | 11.9.2 | Input validation |
| Hangfire | 1.8.14 | Background job processing |
| NLog | 5.3.2 | Logging framework |
| SignalR | 9.0.6 | Real-time communication |
| Swagger/OpenAPI | 6.6.2 | API documentation |

### Frontend (Angular 18)

| Technology | Version | Purpose |
|------------|---------|---------|
| Angular | 18.2.14 | Frontend framework |
| Angular Material | 18.2.14 | UI component library |
| TypeScript | ~5.5.2 | Programming language |
| Bootstrap | 5.3.0 | CSS framework |
| NgBootstrap | 17.0.1 | Bootstrap components |
| Chart.js | 4.4.0 | Data visualization |
| SignalR Client | 9.0.6 | Real-time updates |
| Ngx-Translate | 14.0.0 | Internationalization |
| ExcelJS | 4.4.0 | Excel export |
| PDFTron WebViewer | 11.7.0 | PDF viewing |

### Additional Libraries

- **Document Generation**: iText 8.0.5, iTextSharp 5.5.13.4, EPPlus 7.2.2
- **External Integrations**: Nafath, ADAA, GaStat, SMS Gateway
- **Utilities**: libphonenumber-csharp, ZXing.Net (QR codes), PagedList.Core

---

## Prerequisites

Before you begin, ensure you have the following installed:

### Required

- **[.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)** (8.0 or later)
- **[Node.js](https://nodejs.org/)** (18.x or later) and npm
- **[Angular CLI](https://angular.io/cli)** (18.2.13)
  ```bash
  npm install -g @angular/cli@18.2.13
  ```
- **[SQL Server](https://www.microsoft.com/sql-server)** (2019 or later) or SQL Server Express
- **Git** for version control

### Recommended

- **[Visual Studio 2022](https://visualstudio.microsoft.com/)** (17.8 or later) or **[Visual Studio Code](https://code.visualstudio.com/)**
- **[SQL Server Management Studio (SSMS)](https://learn.microsoft.com/sql/ssms/download-sql-server-management-studio-ssms)**
- **[Postman](https://www.postman.com/)** for API testing

---

## Getting Started

### 1. Clone the Repository

```bash
git clone <repository-url>
cd SMO
```

### 2. Backend Setup

#### Install .NET Dependencies

```bash
dotnet restore
```

#### Configure Database Connection

Edit `SMO.Api/appsettings.Development.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=SMO_VisionCenter_Dev;Trusted_Connection=True;TrustServerCertificate=True;",
    "CommonsConnection": "Server=localhost;Database=SMO_Commons_Dev;Trusted_Connection=True;TrustServerCertificate=True;",
    "IdentityConnection": "Server=localhost;Database=SMO_Identity_Dev;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

#### Apply Database Migrations

```bash
# From solution root
cd SMO.Api
dotnet ef database update
```

#### Configure JWT Settings

Update JWT secret in `appsettings.Development.json`:

```json
{
  "JwtSettings": {
    "SecretKey": "your-secure-secret-key-min-64-characters-change-in-production-use-cryptographically-secure-random-key",
    "Issuer": "SMO.VisionCenter",
    "Audience": "SMO.VisionCenter.Client",
    "ExpirationMinutes": 480
  }
}
```

⚠️ **Security Warning**: Use a strong, randomly generated secret key. Never commit production secrets to version control.

### 3. Frontend Setup

#### Install npm Dependencies

```bash
cd SMO.Frontend/SMO-Portal
npm install
```

#### Configure Environment

Edit `src/environments/environment.ts`:

```typescript
export const environment = {
  production: false,
  enableLogging: true,
  apiUrl: 'https://localhost:7001/api',
  hubUrl: 'https://localhost:7001/api/LiveInform'
};
```

---

## Project Structure

```
SMO/
├── SMO.Api/                          # Web API Project (ASP.NET Core)
│   ├── Controllers/                  # API Controllers
│   ├── appsettings.json              # Configuration
│   ├── nlog.config                   # Logging configuration
│   └── Program.cs                    # Application entry point
│
├── SMO.Application/                  # Application Layer
│   ├── Features/                     # Feature-based organization
│   ├── Services/                     # Application services
│   ├── MappingProfiles/              # AutoMapper profiles
│   └── ServiceCollectionExtensions.cs
│
├── SMO.Domain/                       # Domain Layer (Core Business Logic)
│   ├── Entities/                     # Domain entities
│   ├── Enums/                        # Enumerations
│   └── Interfaces/                   # Repository interfaces
│       ├── IAppDbContext.cs
│       ├── IRepository.cs
│       └── IUnitOfWork.cs
│
├── SMO.Infrastructure/               # Infrastructure Layer
│   ├── Data/                         # EF Core DbContext
│   │   ├── AppDbContext.cs
│   │   ├── Repository.cs
│   │   └── UnitOfWork.cs
│   ├── Repositories/                 # Concrete repositories
│   ├── Mapping/                      # Entity type configurations
│   ├── ApiClients/                   # External API integrations
│   └── ServiceCollectionExtensions.cs
│
├── Framework.Core/                   # Shared Core Framework
│   ├── Data/                         # Base entities, DbContext
│   │   ├── EntityBase.cs
│   │   ├── AuditableEntity.cs
│   │   ├── BaseDbContext.cs
│   │   ├── IBaseDbContext.cs
│   │   ├── IMappingConfiguration.cs
│   │   └── EntityTypeConfiguration.cs
│   ├── AutoMapper/                   # AutoMapper utilities
│   ├── Validators/                   # FluentValidation base
│   ├── Helpers/                      # Utility classes
│   ├── BackgroundJobs/               # Hangfire jobs
│   ├── Notifications/                # Email/SMS/Push services
│   ├── Caching/                      # Caching services
│   ├── Extensions/                   # Extension methods
│   ├── Middlewares/                  # Custom middleware
│   └── SharedServices/               # Shared services
│
├── Framework.Identity/               # Identity & Security Framework
│   ├── Repositories/                 # User/Role repositories
│   ├── DTOs/                         # Auth DTOs
│   └── Seed/                         # Initial user seeding
│
├── Framework.Resources/              # Localization Resources
│   ├── SharedResources.resx          # Default (fallback)
│   ├── SharedResources.ar.resx       # Arabic resources
│   └── SharedResources.en.resx       # English resources
│
└── SMO.Frontend/                     # Angular 18 Frontend
    └── SMO-Portal/
        ├── src/
        │   ├── app/
        │   │   ├── features/         # Feature modules
        │   │   │   ├── pillars/
        │   │   │   ├── objectives/
        │   │   │   ├── programs/
        │   │   │   ├── initiatives/
        │   │   │   ├── kpis/
        │   │   │   └── dashboards/
        │   │   ├── core/             # Core services
        │   │   │   ├── services/     # HTTP, Auth services
        │   │   │   ├── guards/       # Route guards
        │   │   │   └── interceptors/ # HTTP interceptors
        │   │   ├── shared/           # Shared components
        │   │   │   ├── components/
        │   │   │   ├── services/
        │   │   │   ├── models/
        │   │   │   ├── directives/
        │   │   │   └── pipes/
        │   │   └── layout/           # App layout
        │   ├── environments/         # Environment configs
        │   │   ├── environment.ts                  # Development
        │   │   ├── environment.production.ts       # Production
        │   │   ├── environment.staging.ts          # Staging
        │   │   └── environment.testing.ts          # Testing
        │   └── web.config            # IIS deployment config
        ├── angular.json              # Angular configuration
        └── package.json              # npm dependencies
```

---

## Configuration

### Backend Configuration

#### appsettings.json

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=SMO_VisionCenter;...",
    "CommonsConnection": "Server=localhost;Database=SMO_Commons;...",
    "IdentityConnection": "Server=localhost;Database=SMO_Identity;..."
  },
  "JwtSettings": {
    "SecretKey": "your-secret-key-min-64-chars",
    "Issuer": "SMO.VisionCenter",
    "Audience": "SMO.VisionCenter.Client",
    "ExpirationMinutes": 60
  },
  "NotificationSettings": {
    "SmtpHost": "smtp.office365.com",
    "SmtpPort": 587,
    "SmtpUsername": "noreply@smo.gov.sa",
    "SmtpPassword": "change-in-production"
  },
  "ExternalAPIs": {
    "NafathBaseUrl": "https://api.nafath.sa/",
    "NafathClientId": "change-in-production",
    "SmsGatewayUrl": "https://sms.gateway.sa/api/send",
    "AdaaBaseUrl": "https://api.adaa.gov.sa/",
    "GaStatBaseUrl": "https://api.gastat.gov.sa/"
  },
  "CorsOrigins": "http://localhost:4200"
}
```

#### Environment-Specific Configuration

- **Development**: `appsettings.Development.json` (extended JWT expiration, debug logging)
- **Production**: `appsettings.Production.json` (not tracked in git)
- **Staging**: `appsettings.Staging.json` (not tracked in git)
- **Testing**: `appsettings.Testing.json` (not tracked in git)

### Frontend Configuration

#### Environments

| Environment | API URL | Purpose |
|-------------|---------|---------|
| Development | https://localhost:7001/api | Local development |
| Testing | https://test-api.smo.gov.sa/api | QA environment |
| Staging | https://staging-api.smo.gov.sa/api | Pre-production |
| Production | https://api.smo.gov.sa/api | Live system |

---

## Database Setup

### Three Database Architecture

The system uses three separate databases:

1. **SMO_VisionCenter**: Main application database (strategic objectives, initiatives, KPIs)
2. **SMO_Commons**: Shared/commons database (notifications, logs, file uploads)
3. **SMO_Identity**: Identity database (users, roles, permissions)

### Create Databases

```sql
-- Create databases
CREATE DATABASE SMO_VisionCenter_Dev;
CREATE DATABASE SMO_Commons_Dev;
CREATE DATABASE SMO_Identity_Dev;
```

### Apply Migrations

```bash
# From SMO.Api directory
dotnet ef migrations add InitialCreate --context AppDbContext
dotnet ef database update --context AppDbContext
```

---

## Running the Application

### Backend (API)

#### Option 1: Visual Studio
1. Open `SMO.sln` in Visual Studio 2022
2. Set `SMO.Api` as startup project
3. Press F5 to run with debugging

#### Option 2: Command Line

```bash
# From solution root
cd SMO.Api
dotnet run

# Or with hot reload
dotnet watch run
```

The API will start at:
- HTTPS: `https://localhost:7001`
- HTTP: `http://localhost:5000`
- Swagger UI: `https://localhost:7001/swagger`

### Frontend (Angular)

```bash
cd SMO.Frontend/SMO-Portal

# Development server (http://localhost:4200)
ng serve

# Or with specific configuration
ng serve --configuration development

# Open browser automatically
ng serve --open
```

### Access the Application

- **Frontend**: http://localhost:4200
- **Backend API**: https://localhost:7001
- **Swagger Docs**: https://localhost:7001/swagger
- **SignalR Hub**: https://localhost:7001/api/LiveInform

---

## Building for Production

### Backend Build

```bash
# Clean solution
dotnet clean --configuration Release

# Build solution
dotnet build --configuration Release

# Publish API
cd SMO.Api
dotnet publish --configuration Release --output ./publish
```

### Frontend Build

```bash
cd SMO.Frontend/SMO-Portal

# Build for production
ng build --configuration production

# Output location: dist/smo-portal/browser/
```

#### Bundle Size Budget

| Type | Warning | Error |
|------|---------|-------|
| Initial | 500 KB | 1 MB |
| Styles | 2 KB | 4 KB |

Current production build: **259 KB** (raw) / **71 KB** (gzipped) ✅

---

## Deployment

### IIS Deployment (Windows Server)

#### Backend (API)

1. Install .NET 8 Hosting Bundle on IIS server
2. Create IIS application pool (.NET CLR Version: No Managed Code)
3. Copy published files to server
4. Configure `appsettings.Production.json` with production values
5. Set up SSL certificate
6. Configure application pool identity for database access

#### Frontend (Angular)

1. Build for production: `ng build --configuration production`
2. Copy `dist/smo-portal/browser/*` to IIS wwwroot
3. `web.config` is automatically included for URL rewriting
4. Configure IIS to use HTTPS
5. Set up domain binding

### Docker Deployment (Future)

Docker support will be added in a future story.

### Azure Deployment (Future)

Azure App Service deployment will be configured in a future story.

---

## Development Workflow

### Git Workflow

```bash
# Create feature branch
git checkout -b feature/your-feature-name

# Make changes and commit frequently
git add .
git commit -m "Task X Complete: Description"

# Push to remote
git push origin feature/your-feature-name

# Create pull request for review
```

### Commit Message Format

```
Task X Complete: Brief Description

Detailed description of changes:
- Change 1
- Change 2
- Change 3

🤖 Generated with Claude Code
Co-Authored-By: Claude <noreply@anthropic.com>
```

### Running Tests

```bash
# Backend unit tests
dotnet test

# Frontend unit tests
cd SMO.Frontend/SMO-Portal
ng test

# E2E tests
ng e2e
```

---

## API Documentation

### Swagger UI

Once the API is running, visit:
- https://localhost:7001/swagger

### Authentication

All API endpoints (except `/api/auth/login`) require JWT authentication.

#### Login Request

```http
POST /api/auth/login
Content-Type: application/json

{
  "username": "admin",
  "password": "Admin@123"
}
```

#### Response

```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "expiresIn": 3600
}
```

#### Using the Token

```http
GET /api/objectives
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

---

## Troubleshooting

### Common Issues

#### 1. Database Connection Failed

**Error**: `Unable to connect to SQL Server`

**Solution**:
- Verify SQL Server is running
- Check connection string in `appsettings.Development.json`
- Ensure database exists: `dotnet ef database update`
- Check Windows Firewall settings

#### 2. JWT Token Invalid

**Error**: `401 Unauthorized`

**Solution**:
- Verify JWT secret key matches in appsettings.json
- Check token expiration
- Ensure `Authorization: Bearer {token}` header is set
- Verify clock skew (5 minutes tolerance)

#### 3. CORS Error

**Error**: `Access to XMLHttpRequest has been blocked by CORS policy`

**Solution**:
- Add frontend URL to `CorsOrigins` in appsettings.json
- Restart API after configuration change
- Verify CORS middleware is registered in Program.cs

#### 4. Angular Build Fails

**Error**: `Module not found` or `Cannot find module`

**Solution**:
```bash
# Clear cache and reinstall
rm -rf node_modules package-lock.json
npm install

# Clear Angular cache
rm -rf .angular
```

#### 5. NLog Not Writing Logs

**Error**: No log files in `/logs` directory

**Solution**:
- Verify `nlog.config` is copied to output directory
- Check write permissions on logs folder
- Enable NLog internal logging in nlog.config
- Verify connection string for database logging

---

## Contributing

### Code Style Guidelines

#### Backend (C#)
- Follow Microsoft C# Coding Conventions
- Use PascalCase for public members
- Use camelCase for private fields with underscore prefix (`_fieldName`)
- Maximum line length: 120 characters
- Use XML documentation comments for public APIs

#### Frontend (TypeScript/Angular)
- Follow Angular Style Guide
- Use PascalCase for classes, interfaces, types
- Use camelCase for properties, methods, variables
- Use kebab-case for file names
- Prefix interfaces with `I` only when necessary

### Pull Request Process

1. Create feature branch from `main`
2. Implement changes with tests
3. Ensure all tests pass
4. Update documentation as needed
5. Submit pull request with clear description
6. Address code review feedback
7. Squash commits before merging

---

## License

This project is proprietary software developed for the Strategic Management Office (SMO).
© 2025 SMO Vision Center. All rights reserved.

---

## Support

For questions, issues, or feature requests:

- **Issue Tracker**: [GitHub Issues](https://github.com/your-org/smo/issues)
- **Documentation**: [Wiki](https://github.com/your-org/smo/wiki)
- **Email**: support@smo.gov.sa

---

## Acknowledgments

Built with:
- [ASP.NET Core](https://dotnet.microsoft.com/apps/aspnet)
- [Angular](https://angular.io/)
- [Entity Framework Core](https://docs.microsoft.com/ef/core/)
- [Material Design](https://material.angular.io/)
- [NLog](https://nlog-project.org/)
- [Hangfire](https://www.hangfire.io/)

---

**Version**: 1.0.0
**Last Updated**: 2025-10-16
**Status**: Foundation Setup Complete ✅
