# PulseMonitor – Real-Time System Monitoring Dashboard

A full-stack **Real-Time System Monitoring Dashboard** that tracks system metrics, runs background analysis jobs, and pushes live updates to connected clients. Built with **.NET 10**, **Clean Architecture**, **SignalR**, **Hangfire**, and a **React** frontend.

## Features

- **Server/Service monitoring**: Add, edit, delete monitored servers; view list and details with metric history
- **Metrics collection**: Simulated collection every 2 minutes (CPU, memory, disk, response time, status) via Hangfire recurring job
- **Real-time dashboard**: Live charts (Recharts) and SignalR updates for selected server; alert notifications when thresholds are breached
- **Alerting**: Define threshold rules per server; background job evaluates rules and creates alerts; real-time push to dashboard
- **Reports**: Generate performance reports (fire-and-forget Hangfire job); status tracking (Pending/Processing/Completed/Failed); continuation job after completion
- **Background jobs**: Recurring (metrics), fire-and-forget (reports), delayed (maintenance), continuation (post-report); Hangfire dashboard at `/hangfire`
- **Auth**: JWT + role-based (Admin, User); register/login; secure password hashing (BCrypt)
- **API**: REST v1, validation (FluentValidation), global exception handling, health check, Swagger

## Technology Stack

| Layer | Technologies |
|-------|---------------|
| **Backend** | .NET 10, ASP.NET Core, EF Core (SQL Server), MediatR (CQRS), FluentValidation, Hangfire, SignalR, JWT, Serilog |
| **Frontend** | React 18, TypeScript, Vite, React Router, Zustand, Axios, SignalR client, Recharts, Tailwind CSS |
| **Database** | SQL Server (LocalDB for dev; full SQL for Docker/production) |
| **DevOps** | Docker, docker-compose, GitHub Actions (build/test) |

## Architecture (Clean Architecture)

- **Domain**: Entities, enums, domain events, exceptions, interfaces (no dependencies)
- **Application**: Use cases (CQRS/MediatR), DTOs, validators, interfaces, behaviors (logging, validation)
- **Infrastructure**: EF Core (DbContext, configurations, migrations), Hangfire jobs, JWT/password services, persistence
- **API (Presentation)**: Controllers, SignalR hub, middleware (exception handling), auth

Dependencies point inward: API → Infrastructure → Application → Domain.

## Getting Started

### Prerequisites

- .NET 10 SDK
- Node.js 20+
- SQL Server (or LocalDB on Windows)
- (Optional) Docker & docker-compose

### Run locally

1. **Backend**
   ```bash
   cd PulseMonitor
   dotnet restore
   dotnet run --project PulseMonitor.API
   ```
   API: https://localhost:5001 (or port shown). Swagger: https://localhost:5001/swagger  
   DB: Uses `(localdb)\mssqllocaldb` by default; migrations run on startup. Seed creates roles (Admin, User), users `admin` / `Admin123!` and `demo` / `Demo123!`, and 3 sample servers.

2. **Frontend**
   ```bash
   cd pulse-monitor-ui
   npm install
   npm run dev
   ```
   App: http://localhost:3000 (proxies `/api` and `/hubs` to the API).

3. **Log in** with `admin` / `Admin123!` or `demo` / `Demo123!`.

### Run with Docker

From repo root:

```bash
docker-compose up -d
```

- UI: http://localhost:3000  
- API: http://localhost:5000  
- Hangfire: http://localhost:5000/hangfire  

See [DEPLOYMENT.md](DEPLOYMENT.md) for env vars and production notes.

## Testing

- **Backend**
  ```bash
  dotnet test PulseMonitor.Tests/PulseMonitor.Tests.csproj
  ```
  Unit tests: CreateServerCommandHandler, CreateServerCommandValidator, Result. One integration test (Health) is skipped due to Swagger/.NET 10 type load in test host; run the API and call `GET /api/v1/health` for smoke testing.

- **Frontend**
  ```bash
  cd pulse-monitor-ui
  npm run build
  ```

## API Overview

- **Auth**: `POST /api/v1/auth/register`, `POST /api/v1/auth/login`
- **Servers**: `GET/POST /api/v1/servers`, `GET/PUT/DELETE /api/v1/servers/{id}`
- **Metrics**: `GET /api/v1/servers/{id}/metrics`, `GET /api/v1/servers/{id}/metrics/latest`
- **Reports**: `GET/POST /api/v1/reports`, `GET /api/v1/reports/{id}`
- **Alerts**: `GET /api/v1/alerts`
- **Alert rules**: `GET/POST /api/v1/servers/{id}/alert-rules`
- **Users** (Admin): `GET /api/v1/users`
- **Health**: `GET /api/v1/health`

Use **Bearer {token}** for protected endpoints. SignalR hub: `/hubs/monitoring` (send token via `access_token` query or Authorization header).

## Project Layout

```
PulseMonitor/
├── PulseMonitor/                 # Domain (entities, enums, events, interfaces)
├── PulseMonitor.Application/     # CQRS, DTOs, validators, behaviors
├── PulseMonitor.Infrastructure/   # EF Core, Hangfire, JWT, persistence
├── PulseMonitor.API/              # Controllers, Hub, middleware, Program
├── PulseMonitor.Tests/           # Unit + integration tests
├── pulse-monitor-ui/              # React app (Vite, TypeScript)
├── docker-compose.yml
├── README.md
├── ARCHITECTURE.md
└── DEPLOYMENT.md
```

## Pages

- **Login**: Simple form; JWT stored and used for API and SignalR.
- **Dashboard**: Server cards; select one to see live metric chart and recent alerts.
- **Server detail**: Metric history chart (CPU, memory, disk, response time).
- **Reports**: Form to generate report (server + date range); table of report statuses.
- **Alerts**: Table of triggered alerts with server, message, severity.
- **Jobs**: Link opens Hangfire dashboard (recurring, enqueued, succeeded/failed).

