import { useEffect, useState } from 'react'
import { Link } from 'react-router-dom'
import { Line } from 'react-chartjs-2'
import {
  Chart as ChartJS,
  CategoryScale,
  LinearScale,
  PointElement,
  LineElement,
  Title,
  Tooltip,
  Legend,
  type ChartOptions,
} from 'chart.js'
import { serversApi, type ServerListItem, type MetricDto } from '../services/api'
import { createConnection } from '../services/signalr'

ChartJS.register(CategoryScale, LinearScale, PointElement, LineElement, Title, Tooltip, Legend)

export default function Dashboard() {
  const [servers, setServers] = useState<ServerListItem[]>([])
  const [selectedId, setSelectedId] = useState<string | null>(null)
  const [metrics, setMetrics] = useState<MetricDto[]>([])
  const [alerts, setAlerts] = useState<{ message: string; severity: string }[]>([])

  useEffect(() => {
    serversApi.list({ pageSize: 50 }).then((r) => setServers(r.data.items))
  }, [])

  useEffect(() => {
    if (!selectedId) {
      setMetrics([])
      return
    }
    const conn = createConnection(selectedId)
    conn.on('MetricUpdate', (m: MetricDto) => {
      if (m.serverId === selectedId) {
        setMetrics((prev) => [m, ...prev].slice(0, 60))
      }
    })
    conn.on('Alert', (a: { serverName: string; message: string; severity: string }) => {
      setAlerts((prev) => [{ message: `${a.serverName}: ${a.message}`, severity: a.severity }, ...prev].slice(0, 10))
    })
    conn.start().then(() => conn.invoke('JoinServerGroup', selectedId))
    return () => {
      conn.invoke('LeaveServerGroup', selectedId).finally(() => conn.stop())
    }
  }, [selectedId])

  useEffect(() => {
    if (!selectedId) return
    const url = `/api/v1/servers/${selectedId}/metrics`
    const token = localStorage.getItem('accessToken')
    fetch(url, { headers: { Authorization: `Bearer ${token}` } })
      .then((r) => r.json())
      .then((data: MetricDto[]) => setMetrics((prev) => (prev.length ? prev : data.slice(0, 30))))
      .catch(() => {})
  }, [selectedId])

  const sorted = [...metrics].reverse()
  const chartData = {
    labels: sorted.map((m) => new Date(m.timestampUtc).toLocaleTimeString()),
    datasets: [
      { label: 'CPU %', data: sorted.map((m) => m.cpuUsagePercent), borderColor: '#22d3ee', backgroundColor: 'rgba(34, 211, 238, 0.1)', fill: true, tension: 0.3, pointRadius: 0 },
      { label: 'Memory %', data: sorted.map((m) => m.memoryUsagePercent), borderColor: '#a78bfa', backgroundColor: 'rgba(167, 139, 250, 0.1)', fill: true, tension: 0.3, pointRadius: 0 },
      { label: 'Disk %', data: sorted.map((m) => m.diskUsagePercent), borderColor: '#34d399', backgroundColor: 'rgba(52, 211, 153, 0.1)', fill: true, tension: 0.3, pointRadius: 0 },
    ],
  }

  const options: ChartOptions<'line'> = {
    responsive: true,
    maintainAspectRatio: false,
    plugins: {
      legend: { labels: { color: '#94a3b8', font: { size: 12 } } },
      tooltip: {
        backgroundColor: '#1e293b',
        titleColor: '#94a3b8',
        bodyColor: '#e2e8f0',
        borderColor: '#334155',
        borderWidth: 1,
      },
    },
    scales: {
      x: { ticks: { color: '#94a3b8', maxTicksLimit: 10 }, grid: { color: 'rgba(51, 65, 85, 0.5)' } },
      y: { min: 0, max: 100, ticks: { color: '#94a3b8' }, grid: { color: 'rgba(51, 65, 85, 0.5)' } },
    },
  }

  return (
    <div>
      <h1 className="page-header">Dashboard</h1>
      {alerts.length > 0 && (
        <div className="card" style={{ marginBottom: '1.5rem', background: 'rgba(245, 158, 11, 0.1)', borderColor: 'rgba(245, 158, 11, 0.3)' }}>
          <p style={{ fontWeight: 600, color: 'rgb(253 186 116)', marginBottom: '0.5rem' }}>Recent alerts</p>
          <ul style={{ fontSize: '0.875rem', color: 'rgb(254 215 170)', paddingLeft: '1.25rem', margin: 0 }}>
            {alerts.map((a, i) => (
              <li key={i} style={{ marginBottom: '0.25rem' }}>{a.message} <span style={{ color: 'rgb(251 191 36)' }}>({a.severity})</span></li>
            ))}
          </ul>
        </div>
      )}
      <p style={{ fontSize: '0.875rem', color: 'rgb(148 163 184)', marginBottom: '0.75rem' }}>Servers</p>
      <div style={{ display: 'grid', gridTemplateColumns: 'repeat(auto-fill, minmax(240px, 1fr))', gap: '1rem', marginBottom: '2rem' }}>
        {servers.map((s) => (
          <Link
            key={s.id}
            to={`/servers/${s.id}`}
            onClick={() => setSelectedId(s.id)}
            className={`server-card ${selectedId === s.id ? 'selected' : ''}`}
          >
            <p className="server-card-name">{s.name}</p>
            <p className="server-card-host">{s.hostName}</p>
            <span className={`badge ${s.status === 'Up' ? 'badge-up' : 'badge-down'}`}>
              <span className={`badge-dot ${s.status === 'Up' ? 'up' : 'down'}`} />
              {s.status}
            </span>
          </Link>
        ))}
      </div>
      {selectedId && (
        <div className="card">
          <h2 className="card-title">Live metrics</h2>
          {metrics.length > 0 ? (
            <div style={{ height: 320 }}>
              <Line data={chartData} options={options} />
            </div>
          ) : (
            <p style={{ color: 'rgb(100 116 139)', padding: '2rem', textAlign: 'center' }}>Waiting for metrics...</p>
          )}
        </div>
      )}
    </div>
  )
}
