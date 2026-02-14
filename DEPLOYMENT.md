# Deployment Guide

## Prerequisites

- .NET 10 SDK (build)
- Node.js 20+ (frontend build)
- SQL Server (2019+ or Azure SQL) or LocalDB (dev only)
- Docker & docker-compose (optional, for containerized run)

## Environment Variables

### API (PulseMonitor.API)

| Variable | Description | Example |
|----------|-------------|---------|
| `ConnectionStrings__DefaultConnection` | SQL Server connection string | `Server=.;Database=PulseMonitor;User Id=sa;Password=***;TrustServerCertificate=True` |
| `Jwt__Key` | Secret key for JWT signing (min 32 chars) | Strong random string |
| `Jwt__Issuer` | JWT issuer | `PulseMonitor` |
| `Jwt__Audience` | JWT audience | `PulseMonitor` |
| `Cors__Origins` | Allowed origins for CORS | `https://your-ui.domain.com` |

### Frontend (pulse-monitor-ui)

| Variable | Description | Example |
|----------|-------------|---------|
| `VITE_API_URL` | Base URL of the API (when not same origin) | `https://api.your.domain.com` |

## Database Setup

1. Create a SQL Server database (or use existing).
2. Set `ConnectionStrings__DefaultConnection` in appsettings or environment.
3. Run migrations on first deploy:
   ```bash
   dotnet ef database update --project PulseMonitor.Infrastructure --startup-project PulseMonitor.API
   ```
   Or let the API run migrations at startup (default in Program.cs):
   ```csharp
   await db.Database.MigrateAsync();
   await DataSeeder.SeedAsync(...);
   ```
4. Seed creates roles (Admin, User), users `admin`/`Admin123!` and `demo`/`Demo123!`, and sample servers. Change default passwords in production.

## Docker Deployment

From repository root:

```bash
docker-compose up -d
```

- **db**: SQL Server 2022; data in volume `sqldata`.
- **api**: Built from `PulseMonitor.API/Dockerfile`; uses `db` connection string; exposes 5000 → 8080.
- **ui**: Built from `pulse-monitor-ui/Dockerfile`; nginx serves static files and proxies `/api`, `/hubs`, `/hangfire` to `api`; exposes 3000 → 80.

For production:

- Override env (e.g. `Jwt__Key`, `ConnectionStrings__DefaultConnection`) via `docker-compose.override.yml` or secrets.
- Use a real SQL server (not the `db` container) if you need persistence and backups.
- Restrict Hangfire dashboard (e.g. auth or network).

## Cloud Deployment (Generic)

- **API**: Publish as a single executable or container; ensure HTTPS and correct CORS origins.
- **Frontend**: Build with `npm run build`; serve `dist/` via static hosting (e.g. Azure Static Web Apps, S3 + CloudFront) or the same host with a reverse proxy.
- **Database**: Use managed SQL (e.g. Azure SQL, RDS); run migrations as part of release or a one-off job.
- **SignalR**: For multiple API instances, use a backplane (e.g. Redis) so SignalR messages are broadcast across instances.
- **Hangfire**: With multiple instances, use a single worker or a shared Hangfire storage (SQL) and ensure only one instance processes recurring jobs (or use distributed locks).

## Scaling

- **Horizontal**: Add more API instances behind a load balancer; add Redis SignalR backplane and shared Hangfire storage.
- **Background jobs**: Run Hangfire in a dedicated worker process or a separate container that shares the same DB and storage.
- **Database**: Tune indexes (e.g. Metrics: ServerId + TimestampUtc); consider retention/archiving for old metrics.

## Monitoring and Logging

- Serilog is configured; add sinks (e.g. file, Seq, Application Insights) via appsettings or code.
- Health check: `GET /api/v1/health` returns 200 when DB is reachable, 503 otherwise.
- Hangfire dashboard: job history and retries at `/hangfire` (restrict in production).

## Troubleshooting

- **502/503 from UI to API**: Check API is running and CORS allows the UI origin; for Docker, ensure `api` service is healthy and UI proxy points to `http://api:8080`.
- **SignalR connection fails**: Ensure JWT is sent (`access_token` query or `Authorization` header); in Docker, WebSockets must be allowed (nginx `proxy_http_version 1.1` and `Upgrade`/`Connection` headers).
- **Migrations fail**: Verify connection string and that the SQL user has rights to create/alter tables; for LocalDB, ensure the instance is running.
- **Hangfire jobs not running**: Confirm Hangfire is using the same connection string as the app; check dashboard for failed jobs and exceptions.
