import { useEffect, useState } from 'react'
import { reportsApi, serversApi, type ReportListItem, type ServerListItem } from '../services/api'

export default function Reports() {
  const [reports, setReports] = useState<ReportListItem[]>([])
  const [servers, setServers] = useState<ServerListItem[]>([])
  const [totalCount, setTotalCount] = useState(0)
  const [page, setPage] = useState(1)
  const [creating, setCreating] = useState(false)
  const [form, setForm] = useState({ serverId: '', fromUtc: '', toUtc: '' })

  useEffect(() => {
    reportsApi.list({ pageNumber: page, pageSize: 10 }).then((r) => {
      setReports(r.data.items)
      setTotalCount(r.data.totalCount)
    })
    serversApi.list({ pageSize: 100 }).then((r) => setServers(r.data.items))
  }, [page])

  const createReport = async (e: React.FormEvent) => {
    e.preventDefault()
    if (!form.serverId || !form.fromUtc || !form.toUtc) return
    setCreating(true)
    try {
      await reportsApi.create({
        serverId: form.serverId,
        fromUtc: new Date(form.fromUtc).toISOString(),
        toUtc: new Date(form.toUtc).toISOString(),
      })
      setForm({ serverId: '', fromUtc: '', toUtc: '' })
      reportsApi.list({ pageNumber: 1, pageSize: 10 }).then((r) => {
        setReports(r.data.items)
        setTotalCount(r.data.totalCount)
      })
    } finally {
      setCreating(false)
    }
  }

  const totalPages = Math.ceil(totalCount / 10)

  const statusStyle = (s: string) => {
    if (s === 'Completed') return { background: 'rgba(34 197 94 / 0.2)', color: 'rgb(74 222 128)' }
    if (s === 'Failed') return { background: 'rgba(239 68 68 / 0.2)', color: 'rgb(248 113 113)' }
    return { background: 'rgba(245 158 11 / 0.2)', color: 'rgb(251 191 36)' }
  }

  return (
    <div>
      <h1 className="page-header">Reports</h1>
      <form onSubmit={createReport} className="card" style={{ marginBottom: '2rem', display: 'flex', flexWrap: 'wrap', alignItems: 'flex-end', gap: '1rem' }}>
        <div>
          <label className="auth-label">Server</label>
          <select
            value={form.serverId}
            onChange={(e) => setForm((f) => ({ ...f, serverId: e.target.value }))}
            className="auth-input"
            style={{ minWidth: 200, marginBottom: 0 }}
            required
          >
            <option value="">Select server</option>
            {servers.map((s) => (
              <option key={s.id} value={s.id}>{s.name}</option>
            ))}
          </select>
        </div>
        <div>
          <label className="auth-label">From</label>
          <input
            type="datetime-local"
            value={form.fromUtc}
            onChange={(e) => setForm((f) => ({ ...f, fromUtc: e.target.value }))}
            className="auth-input"
            style={{ marginBottom: 0 }}
            required
          />
        </div>
        <div>
          <label className="auth-label">To</label>
          <input
            type="datetime-local"
            value={form.toUtc}
            onChange={(e) => setForm((f) => ({ ...f, toUtc: e.target.value }))}
            className="auth-input"
            style={{ marginBottom: 0 }}
            required
          />
        </div>
        <button type="submit" disabled={creating} className="btn-primary">
          {creating ? 'Creating...' : 'Generate report'}
        </button>
      </form>
      <div className="data-table-wrap">
        <table className="data-table">
          <thead>
            <tr>
              <th>Server</th>
              <th>Status</th>
              <th>From</th>
              <th>To</th>
              <th>Created</th>
            </tr>
          </thead>
          <tbody>
            {reports.map((r) => (
              <tr key={r.id}>
                <td className="cell-primary">{r.serverName}</td>
                <td>
                  <span style={{ display: 'inline-block', padding: '0.25rem 0.5rem', borderRadius: '0.375rem', fontSize: '0.75rem', fontWeight: 500, ...statusStyle(r.status) }}>
                    {r.status}
                  </span>
                </td>
                <td style={{ fontSize: '0.875rem' }}>{new Date(r.fromUtc).toLocaleString()}</td>
                <td style={{ fontSize: '0.875rem' }}>{new Date(r.toUtc).toLocaleString()}</td>
                <td style={{ color: 'rgb(100 116 139)', fontSize: '0.875rem' }}>{new Date(r.createdAtUtc).toLocaleString()}</td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
      {totalPages > 1 && (
        <div className="pagination-wrap">
          <button type="button" onClick={() => setPage((p) => Math.max(1, p - 1))} disabled={page <= 1} className="pagination-btn">Previous</button>
          <span className="pagination-info">Page {page} of {totalPages}</span>
          <button type="button" onClick={() => setPage((p) => Math.min(totalPages, p + 1))} disabled={page >= totalPages} className="pagination-btn">Next</button>
        </div>
      )}
    </div>
  )
}
