# Architecture

## Clean Architecture Layers

The solution follows **Clean Architecture** with four layers and dependency rule: dependencies point inward; the Domain has no dependencies on other projects.

### 1. Domain (PulseMonitor)

- **Entities**: `Server`, `Metric`, `User`, `Role`, `UserRole`, `Alert`, `AlertRule`, `Report`; `BaseEntity` for shared identity.
- **Enums**: `ServerStatus`, `ReportStatus`, `AlertSeverity`, `MetricType`.
- **Events**: `IDomainEvent`, `MetricCollectedEvent`, `AlertTriggeredEvent` (for future event handlers).
- **Exceptions**: `DomainException`, `NotFoundException`.
- **Interfaces**: `IAuditableEntity`, `IUnitOfWork` (abstractions for infrastructure).

**SOLID**: Single responsibility (each entity/exception has one purpose); Open/Closed (extend via new entities/events); Liskov (exceptions substitute base); Interface segregation (small interfaces); Dependency inversion (domain defines interfaces, infrastructure implements).

### 2. Application (PulseMonitor.Application)

- **Common/Interfaces**: `IApplicationDbContext`, `IJwtTokenService`, `ICurrentUserService`, `IPasswordHasher`, `IReportJobService`, `IMetricPushService`.
- **Common/Models**: `PagedResult<T>`, `Result` / `Result<T>`.
- **Common/Behaviors**: `ValidationBehavior` (FluentValidation pipeline), `LoggingBehavior` (request timing).
- **Features**: CQRS-style commands and queries per aggregate (Servers, Metrics, Reports, Users, Alerts, AlertRules); each feature folder contains command/query, validator, handler.

**CQRS**: Commands change state (CreateServer, UpdateServer, CreateReport, Login, Register); queries return data (GetServers, GetMetricsByServer, GetAlerts, etc.). MediatR dispatches to handlers.

**SOLID**: Handlers have single responsibility; new features added without modifying existing ones (Open/Closed); application depends on abstractions (Dependency Inversion).

### 3. Infrastructure (PulseMonitor.Infrastructure)

- **Persistence**: `ApplicationDbContext` (implements `IApplicationDbContext`); entity configurations; migrations; `DataSeeder` (roles, admin/demo users, sample servers); `ApplicationDbContextFactory` for EF design-time.
- **BackgroundJobs**: `MetricsCollectionJob` (recurring), `ReportGenerationJob` (fire-and-forget), `MaintenanceJob` (delayed), `ReportCompletedContinuationJob` (continuation).
- **Services**: `JwtTokenService`, `PasswordHasher` (BCrypt), `ReportJobService` (Hangfire enqueue).

Implements all application interfaces that touch persistence, external services, or background processing.

### 4. Presentation / API (PulseMonitor.API)

- **Controllers**: REST v1 (Servers, Metrics, Auth, Reports, Alerts, AlertRules, Users, Health).
- **Hubs**: `MonitoringHub` (SignalR) for metric and alert push; group “Dashboard” and per-server groups.
- **Middleware**: `ExceptionHandlingMiddleware` (map domain/validation exceptions to HTTP status and JSON).
- **Services**: `CurrentUserService` (from HttpContext), `SignalRMetricPushService` (implements `IMetricPushService`).

Program wires Application, Infrastructure, Serilog, CORS, JWT for HTTP and SignalR, Hangfire dashboard, migration + seed on startup, and recurring/delayed job registration.

## Database

- **SQL Server** (LocalDB for dev; full instance for Docker/production).
- **Relationships**: Server → Metrics (one-to-many); User ↔ Role (many-to-many via UserRole); Server → Alerts, AlertRules, Reports.
- **Repository**: No separate repository layer; application uses `IApplicationDbContext` and Infrastructure provides `ApplicationDbContext`. This keeps the surface small and avoids an extra abstraction that would duplicate EF’s `DbSet` usage.

## Design Patterns

- **CQRS**: Commands and queries separated; MediatR pipeline.
- **Pipeline behavior**: Validation and logging wrap every request.
- **Dependency injection**: All layers registered in API/Infrastructure; constructors receive interfaces.
- **Domain events**: Defined in Domain; can be published from Application and handled in Infrastructure (e.g. push to SignalR) without changing domain.

## SOLID in Code

- **SRP**: e.g. `CreateServerCommandHandler` only creates a server; `ValidationBehavior` only validates.
- **OCP**: New use cases = new command/query + handler; no change to existing handlers.
- **LSP**: `NotFoundException` and `DomainException` used where `Exception` is expected.
- **ISP**: `IAuditableEntity` only for entities that need audit fields; `IApplicationDbContext` exposes only required sets.
- **DIP**: Application and API depend on `IApplicationDbContext`, `IJwtTokenService`, `IMetricPushService`, etc.; concrete implementations live in Infrastructure/API.

## Folder Structure (Backend)

```
src/
  PulseMonitor/                    # Domain
    Entities/, Enums/, Events/, Exceptions/, Interfaces/
  PulseMonitor.Application/
    Common/ (Interfaces, Models, Behaviors)
    Features/ (Servers, Metrics, Reports, Users, Alerts, AlertRules)
    DependencyInjection.cs
  PulseMonitor.Infrastructure/
    Persistence/ (Configurations, Migrations, ApplicationDbContext, DataSeeder)
    BackgroundJobs/
    Services/
    DependencyInjection.cs
  PulseMonitor.API/
    Controllers/, Hubs/, Middleware/, Services/
    Program.cs
```

## SignalR Flow

1. Client connects to `/hubs/monitoring` with JWT (query `access_token` or header).
2. Client joins group “Dashboard” and optionally `Server_{serverId}`.
3. Recurring job runs `MetricsCollectionJob`; for each server it saves a metric, evaluates alert rules, then calls `IMetricPushService.PushMetricAsync` and `PushAlertAsync`.
4. `SignalRMetricPushService` sends to SignalR groups “Dashboard” and `Server_{id}`.
5. Frontend subscribes to `MetricUpdate` and `Alert` and updates UI.

## Hangfire Job Types

| Type | Job | Schedule / Trigger |
|------|-----|---------------------|
| Recurring | MetricsCollectionJob | Every 2 minutes |
| Fire-and-forget | ReportGenerationJob | When user creates report |
| Delayed | MaintenanceJob | 5 minutes after app start |
| Continuation | ReportCompletedContinuationJob | After ReportGenerationJob completes |
