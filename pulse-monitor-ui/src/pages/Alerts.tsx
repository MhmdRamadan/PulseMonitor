import { useEffect, useState } from 'react'
import { alertsApi, type AlertDto } from '../services/api'

export default function Alerts() {
  const [alerts, setAlerts] = useState<AlertDto[]>([])
  const [totalCount, setTotalCount] = useState(0)
  const [page, setPage] = useState(1)

  useEffect(() => {
    alertsApi.list({ pageNumber: page, pageSize: 20 }).then((r) => {
      setAlerts(r.data.items)
      setTotalCount(r.data.totalCount)
    })
  }, [page])

  const totalPages = Math.ceil(totalCount / 20)

  const severityStyle = (s: string) => {
    if (s === 'Critical') return { background: 'rgba(239 68 68 / 0.2)', color: 'rgb(248 113 113)' }
    if (s === 'Warning') return { background: 'rgba(245 158 11 / 0.2)', color: 'rgb(251 191 36)' }
    return { background: 'rgba(71 85 105 / 0.3)', color: 'rgb(148 163 184)' }
  }

  return (
    <div>
      <h1 className="page-header">Alerts</h1>
      <div className="data-table-wrap">
        <table className="data-table">
          <thead>
            <tr>
              <th>Server</th>
              <th>Message</th>
              <th>Severity</th>
              <th>Triggered</th>
            </tr>
          </thead>
          <tbody>
            {alerts.map((a) => (
              <tr key={a.id}>
                <td className="cell-primary">{a.serverName}</td>
                <td>{a.message}</td>
                <td>
                  <span style={{ display: 'inline-block', padding: '0.25rem 0.5rem', borderRadius: '0.375rem', fontSize: '0.75rem', fontWeight: 500, ...severityStyle(a.severity) }}>
                    {a.severity}
                  </span>
                </td>
                <td style={{ color: 'rgb(100 116 139)', fontSize: '0.875rem' }}>{new Date(a.triggeredAtUtc).toLocaleString()}</td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
      {alerts.length === 0 && (
        <p style={{ color: 'rgb(100 116 139)', marginTop: '1.5rem', textAlign: 'center', padding: '2rem' }}>No alerts.</p>
      )}
      {totalPages > 1 && (
        <div className="pagination-wrap">
          <button
            type="button"
            onClick={() => setPage((p) => Math.max(1, p - 1))}
            disabled={page <= 1}
            className="pagination-btn"
          >
            Previous
          </button>
          <span className="pagination-info">Page {page} of {totalPages}</span>
          <button
            type="button"
            onClick={() => setPage((p) => Math.min(totalPages, p + 1))}
            disabled={page >= totalPages}
            className="pagination-btn"
          >
            Next
          </button>
        </div>
      )}
    </div>
  )
}
