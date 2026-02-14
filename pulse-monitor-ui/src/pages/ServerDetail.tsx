import { useEffect, useState } from 'react'
import { useParams, Link } from 'react-router-dom'
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
import { serversApi, metricsApi, type ServerDetail as ServerDetailType, type MetricDto } from '../services/api'
import { createConnection } from '../services/signalr'

ChartJS.register(CategoryScale, LinearScale, PointElement, LineElement, Title, Tooltip, Legend)

export default function ServerDetail() {
  const { id } = useParams<{ id: string }>()
  const [server, setServer] = useState<ServerDetailType | null>(null)
  const [metrics, setMetrics] = useState<MetricDto[]>([])

  useEffect(() => {
    if (!id) return
    serversApi.get(id).then((r) => setServer(r.data))
    metricsApi.byServer(id, { limit: 100 }).then((r) => setMetrics(r.data))
  }, [id])

  useEffect(() => {
    if (!id) return
    const conn = createConnection(id)
    conn.on('MetricUpdate', (m: MetricDto) => {
      if (m.serverId === id) setMetrics((prev) => [m, ...prev].slice(0, 100))
    })
    conn.start().then(() => conn.invoke('JoinServerGroup', id))
    return () => {
      conn.invoke('LeaveServerGroup', id).finally(() => conn.stop())
    }
  }, [id])

  if (!server) {
    return (
      <div style={{ display: 'flex', alignItems: 'center', justifyContent: 'center', padding: '4rem' }}>
        <p style={{ color: 'rgb(100 116 139)' }}>Loading...</p>
      </div>
    )
  }

  const sorted = [...metrics].reverse()
  const chartData = {
    labels: sorted.map((m) => new Date(m.timestampUtc).toLocaleTimeString()),
    datasets: [
      { label: 'CPU %', data: sorted.map((m) => m.cpuUsagePercent), borderColor: '#22d3ee', backgroundColor: 'rgba(34, 211, 238, 0.1)', fill: true, tension: 0.3, pointRadius: 0, yAxisID: 'y' },
      { label: 'Memory %', data: sorted.map((m) => m.memoryUsagePercent), borderColor: '#a78bfa', backgroundColor: 'rgba(167, 139, 250, 0.1)', fill: true, tension: 0.3, pointRadius: 0, yAxisID: 'y' },
      { label: 'Disk %', data: sorted.map((m) => m.diskUsagePercent), borderColor: '#34d399', backgroundColor: 'rgba(52, 211, 153, 0.1)', fill: true, tension: 0.3, pointRadius: 0, yAxisID: 'y' },
      { label: 'Response ms', data: sorted.map((m) => m.responseTimeMs), borderColor: '#f59e0b', backgroundColor: 'rgba(245, 158, 11, 0.1)', fill: true, tension: 0.3, pointRadius: 0, yAxisID: 'y1' },
    ],
  }

  const options: ChartOptions<'line'> = {
    responsive: true,
    maintainAspectRatio: false,
    interaction: { mode: 'index', intersect: false },
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
      x: { ticks: { color: '#94a3b8', maxTicksLimit: 12 }, grid: { color: 'rgba(51, 65, 85, 0.5)' } },
      y: {
        type: 'linear',
        position: 'left',
        min: 0,
        max: 100,
        ticks: { color: '#94a3b8' },
        grid: { color: 'rgba(51, 65, 85, 0.5)' },
      },
      y1: {
        type: 'linear',
        position: 'right',
        min: 0,
        ticks: { color: '#94a3b8' },
        grid: { drawOnChartArea: false },
      },
    },
  }

  return (
    <div>
      <Link to="/" className="app-nav-link" style={{ display: 'inline-block', marginBottom: '1.5rem' }}>
        ← Dashboard
      </Link>
      <div className="card" style={{ marginBottom: '1.5rem' }}>
        <h1 style={{ fontSize: '1.5rem', fontWeight: 700, color: 'rgb(248 250 252)', margin: 0 }}>{server.name}</h1>
        <p className="server-card-host" style={{ marginTop: '0.25rem' }}>
          {server.hostName} {server.ipAddress && `· ${server.ipAddress}`}
        </p>
        <span className={`badge ${server.status === 'Up' ? 'badge-up' : 'badge-down'}`}>
          <span className={`badge-dot ${server.status === 'Up' ? 'up' : 'down'}`} style={{ width: 8, height: 8 }} />
          {server.status}
        </span>
      </div>
      <div className="card">
        <h2 className="card-title">Metrics history</h2>
        {metrics.length > 0 ? (
          <div style={{ height: 360 }}>
            <Line data={chartData} options={options} />
          </div>
        ) : (
          <p style={{ color: 'rgb(100 116 139)', padding: '2rem', textAlign: 'center' }}>No metrics yet.</p>
        )}
      </div>
    </div>
  )
}
