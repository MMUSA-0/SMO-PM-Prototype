# Appendix C: Architectural Decision Records (ADRs)

## ADR-001: Clean Architecture Adoption
**Decision:** Implement Clean Architecture with clear layer boundaries
**Rationale:**
- Separation of concerns
- Testability
- Technology independence
- Maintainability

## ADR-002: Generic Repository Pattern
**Decision:** Use generic repository pattern with rich querying
**Rationale:**
- Reduce code duplication
- Consistent data access patterns
- Flexible querying without exposing DbContext
- Easier to mock for testing

## ADR-003: Framework Layer Separation
**Decision:** Extract cross-cutting concerns into Framework projects
**Rationale:**
- Reusability across projects
- Clear separation of domain vs. infrastructure
- Easier to maintain common functionality

## ADR-004: No CQRS Pattern
**Decision:** Do not implement CQRS (Command Query Responsibility Segregation)
**Rationale (per user requirements):**
- Simpler architecture
- Less boilerplate
- Easier to understand for team
- Sufficient for current requirements

## ADR-005: EF Core Code-First
**Decision:** Use Code-First migrations for database schema
**Rationale:**
- Version control for database schema
- Type-safe entity definitions
- Automatic migration generation
- Consistent across environments

## ADR-006: Automatic Auditing
**Decision:** Implement automatic auditing in BaseDbContext
**Rationale:**
- Consistent audit trail
- No manual code in services
- Centralized implementation
- Reduces human error

---
