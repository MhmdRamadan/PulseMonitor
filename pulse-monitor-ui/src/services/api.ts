import axios from 'axios'

const api = axios.create({
  baseURL: '/api/v1',
  headers: { 'Content-Type': 'application/json' },
  withCredentials: true,
})

function getAccessToken(): string | null {
  const direct = localStorage.getItem('accessToken')
  if (direct) return direct
  try {
    const raw = localStorage.getItem('auth')
    if (raw) {
      const parsed = JSON.parse(raw) as { state?: { accessToken?: string } }
      return parsed?.state?.accessToken ?? null
    }
  } catch {
    // ignore
  }
  return null
}

api.interceptors.request.use((config) => {
  const token = getAccessToken()
  if (token) {
    config.headers.Authorization = `Bearer ${token}`
  }
  return config
})

api.interceptors.response.use(
  (r) => r,
  (err) => {
    if (err.response?.status === 401) {
      localStorage.removeItem('accessToken')
      localStorage.removeItem('auth')
      window.location.href = '/login'
    }
    return Promise.reject(err)
  }
)

export default api

export const serversApi = {
  list: (params?: { search?: string; pageNumber?: number; pageSize?: number }) =>
    api.get<{ items: ServerListItem[]; totalCount: number }>('/servers', { params }),
  get: (id: string) => api.get<ServerDetail>(`/servers/${id}`),
  create: (data: { name: string; hostName: string; ipAddress?: string; description?: string }) =>
    api.post<{ id: string }>('/servers', data),
  update: (id: string, data: { name: string; hostName: string; ipAddress?: string; description?: string }) =>
    api.put(`/servers/${id}`, data),
  delete: (id: string) => api.delete(`/servers/${id}`),
}

export const metricsApi = {
  byServer: (serverId: string, params?: { fromUtc?: string; toUtc?: string; limit?: number }) =>
    api.get<MetricDto[]>(`/servers/${serverId}/metrics`, { params }),
  latest: (serverId: string) => api.get<MetricDto>(`/servers/${serverId}/metrics/latest`),
}

export const authApi = {
  login: (userName: string, password: string) =>
    api.post<AuthResponse>('/auth/login', { userName, password }),
  register: (userName: string, email: string, password: string) =>
    api.post<AuthResponse>('/auth/register', { userName, email, password }),
}

export const reportsApi = {
  list: (params?: { serverId?: string; pageNumber?: number; pageSize?: number }) =>
    api.get<{ items: ReportListItem[]; totalCount: number }>('/reports', { params }),
  get: (id: string) => api.get<ReportDetail>(`/reports/${id}`),
  create: (data: { serverId: string; fromUtc: string; toUtc: string }) =>
    api.post<{ id: string; status: string }>('/reports', data),
}

export const alertsApi = {
  list: (params?: { serverId?: string; pageNumber?: number; pageSize?: number }) =>
    api.get<{ items: AlertDto[]; totalCount: number }>('/alerts', { params }),
}

export const alertRulesApi = {
  byServer: (serverId: string) => api.get<AlertRuleDto[]>(`/servers/${serverId}/alert-rules`),
  create: (serverId: string, data: { metricType: string; operator: string; thresholdValue: number; severity: number }) =>
    api.post<{ id: string }>(`/servers/${serverId}/alert-rules`, data),
}

export const usersApi = {
  list: (params?: { pageNumber?: number; pageSize?: number }) =>
    api.get<{ items: UserListItem[]; totalCount: number }>('/users', { params }),
}

export interface ServerListItem {
  id: string
  name: string
  hostName: string
  ipAddress?: string
  status: string
  lastMetricAt?: string
}
export interface ServerDetail {
  id: string
  name: string
  hostName: string
  ipAddress?: string
  description?: string
  status: string
  createdAtUtc: string
  updatedAtUtc?: string
}
export interface MetricDto {
  id: string
  serverId: string
  cpuUsagePercent: number
  memoryUsagePercent: number
  diskUsagePercent: number
  responseTimeMs: number
  status: string
  timestampUtc: string
}
export interface AuthResponse {
  accessToken: string
  refreshToken: string
  expiresAtUtc: string
  userName: string
  roles: string[]
}
export interface ReportListItem {
  id: string
  serverId: string
  serverName: string
  status: string
  fromUtc: string
  toUtc: string
  createdAtUtc: string
  completedAtUtc?: string
}
export interface ReportDetail extends ReportListItem {
  filePath?: string
  errorMessage?: string
}
export interface AlertDto {
  id: string
  serverId: string
  serverName: string
  message: string
  severity: string
  thresholdValue?: number
  triggeredAtUtc: string
  acknowledgedAtUtc?: string
}
export interface AlertRuleDto {
  id: string
  serverId: string
  metricType: string
  operator: string
  thresholdValue: number
  severity: string
  isActive: boolean
}
export interface UserListItem {
  id: string
  userName: string
  email: string
  createdAtUtc: string
  roles: string[]
}
