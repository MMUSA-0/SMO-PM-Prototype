# 10. Deployment Architecture

## 10.1 Environment Configuration

**Backend (appsettings.json):**
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=...;Database=...;",
    "CommonsConnection": "Server=...;Database=Commons;",
    "IdentityConnection": "Server=...;Database=Identity;"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning"
    }
  },
  "JwtSettings": {
    "SecretKey": "...",
    "Issuer": "...",
    "Audience": "...",
    "ExpirationMinutes": 60
  },
  "NotificationSettings": {
    "SmtpHost": "...",
    "SmtpPort": 587,
    "FirebaseServerKey": "..."
  }
}
```

**Environment-Specific Files:**
- `appsettings.json` - Base configuration
- `appsettings.Development.json` - Dev overrides
- `appsettings.Staging.json` - Staging overrides
- `appsettings.Production.json` - Production overrides

**Frontend (environments/):**
```typescript
export const environment = {
  production: false,
  apiUrl: 'https://api.development.com',
  enableLogging: true
};
```

## 10.2 Build & Deployment

**Backend Build:**
```bash
dotnet build --configuration Release
dotnet publish --configuration Release --output ./publish
```

**Frontend Build:**
```bash