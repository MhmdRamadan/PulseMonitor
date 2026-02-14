# API Reference

Base URL: `/api/v1` (e.g. `https://localhost:5001/api/v1`).

## Authentication

- **Register**: `POST /auth/register`  
  Body: `{ "userName": "string", "email": "string", "password": "string" }`  
  Returns: `{ accessToken, refreshToken, expiresAtUtc, userName, roles }`

- **Login**: `POST /auth/login`  
  Body: `{ "userName": "string", "password": "string" }`  
  Returns: same as register.

Use the `accessToken` in the **Authorization** header: `Bearer {accessToken}` for all protected endpoints.

## Servers

- `GET /servers` – Paginated list (query: `search`, `pageNumber`, `pageSize`, `sortBy`, `sortDescending`). **Auth.**
- `GET /servers/{id}` – Single server. **Auth.**
- `POST /servers` – Create (body: `name`, `hostName`, `ipAddress?`, `description?`). **Admin.**
- `PUT /servers/{id}` – Update. **Admin.**
- `DELETE /servers/{id}` – Delete. **Admin.**

## Metrics

- `GET /servers/{serverId}/metrics` – List (query: `fromUtc`, `toUtc`, `limit`). **Auth.**
- `GET /servers/{serverId}/metrics/latest` – Latest metric. **Auth.**

## Reports

- `GET /reports` – Paginated (query: `serverId`, `pageNumber`, `pageSize`). **Auth.**
- `GET /reports/{id}` – Single report. **Auth.**
- `POST /reports` – Create (body: `serverId`, `fromUtc`, `toUtc`). **Auth.** Enqueues a background job.

## Alerts

- `GET /alerts` – Paginated (query: `serverId`, `pageNumber`, `pageSize`). **Auth.**

## Alert Rules

- `GET /servers/{serverId}/alert-rules` – List. **Auth.**
- `POST /servers/{serverId}/alert-rules` – Create (body: `metricType`, `operator`, `thresholdValue`, `severity`). **Admin.**

## Users

- `GET /users` – Paginated. **Admin.**

## Health

- `GET /health` – Returns 200 + `{ status: "Healthy", timestamp }` if DB is reachable; 503 otherwise. No auth.

## SignalR Hub

- **Endpoint**: `/hubs/monitoring`
- **Auth**: Send JWT via query `access_token` or `Authorization: Bearer {token}`.
- **Server → Client**:
  - `MetricUpdate`: payload is a metric DTO (id, serverId, cpuUsagePercent, memoryUsagePercent, diskUsagePercent, responseTimeMs, status, timestampUtc).
  - `Alert`: payload is `{ serverId, serverName, message, severity, triggeredAt }`.
- **Client → Server**:
  - `JoinServerGroup(serverId)`: subscribe to updates for one server.
  - `LeaveServerGroup(serverId)`: unsubscribe.

## HTTP Status Codes

- 200 OK, 201 Created, 204 No Content
- 400 Bad Request (validation/domain errors)
- 401 Unauthorized (missing/invalid token)
- 403 Forbidden (insufficient role)
- 404 Not Found
- 503 Service Unhealthy (health check)
