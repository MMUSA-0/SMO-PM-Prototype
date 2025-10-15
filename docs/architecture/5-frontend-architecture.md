# 5. Frontend Architecture

## 5.1 Angular Application Structure

**Project:** `SMO.Frontend/SMO-Portal`

**Key Configuration:**
- Angular 18.2.13 (Module-based, not standalone)
- TypeScript 5.4.5
- Build configurations: Development, Testing, Staging, Production
- Purpose: Vision 2030 Information Center portal for strategic management

**Structure:**
```
SMO.Frontend/SMO-Portal/
├── src/
│   ├── app/
│   │   ├── app-routing.module.ts
│   │   ├── app.module.ts
│   │   └── app.component.ts
│   │   ├── features/
│   │   │   ├── pillars/          # Strategic pillars module
│   │   │   ├── objectives/       # Strategic objectives module
│   │   │   ├── programs/         # VRP management module
│   │   │   ├── initiatives/      # Initiative tracking module
│   │   │   ├── kpis/             # KPI management & dashboards
│   │   │   └── dashboards/       # Performance visualization
│   │   ├── shared/               # Shared components & services
│   │   └── core/                 # Core services & guards
│   ├── assets/
│   ├── environments/
│   │   ├── environment.ts
│   │   ├── environment.production.ts
│   │   ├── environment.staging.ts
│   │   └── environment.testing.ts
│   ├── index.html
│   ├── main.ts
│   ├── styles.css
│   └── web.config         # IIS deployment configuration
├── angular.json
├── package.json
└── tsconfig.json
```

## 5.2 Build Configurations

**Development:**
- No optimization
- Source maps enabled
- Named chunks for debugging

**Production/Staging/Testing:**
- Full optimization
- Output hashing for cache busting
- Bundle size budgets:
  - Initial: 500KB warning, 1MB error
  - Component styles: 2KB warning, 4KB error

## 5.3 Deployment

The application includes `web.config` for IIS deployment with URL rewriting support for SPA routing.

---
