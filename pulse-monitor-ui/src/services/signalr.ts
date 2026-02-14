import * as signalR from '@microsoft/signalr'

const getToken = (): string => localStorage.getItem('accessToken') ?? ''

export function createConnection(_serverId?: string) {
  const base = import.meta.env.VITE_API_URL || ''
  const url = base ? `${base}/hubs/monitoring` : '/hubs/monitoring'
  const conn = new signalR.HubConnectionBuilder()
    .withUrl(url, { accessTokenFactory: getToken })
    .withAutomaticReconnect()
    .build()
  return conn
}

export type MetricUpdateHandler = (metric: import('./api').MetricDto) => void
export type AlertHandler = (alert: { serverId: string; serverName: string; message: string; severity: string; triggeredAt: string }) => void
