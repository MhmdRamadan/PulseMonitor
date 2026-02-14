import { useEffect, useState } from 'react'
import { usersApi, type UserListItem } from '../services/api'

export default function Users() {
  const [users, setUsers] = useState<UserListItem[]>([])
  const [totalCount, setTotalCount] = useState(0)
  const [page, setPage] = useState(1)

  useEffect(() => {
    usersApi.list({ pageNumber: page, pageSize: 20 }).then((r) => {
      setUsers(r.data.items)
      setTotalCount(r.data.totalCount)
    })
  }, [page])

  const totalPages = Math.ceil(totalCount / 20)

  return (
    <div>
      <h1 className="page-header">User management</h1>
      <div className="data-table-wrap">
        <table className="data-table">
          <thead>
            <tr>
              <th>User name</th>
              <th>Email</th>
              <th>Roles</th>
              <th>Created</th>
            </tr>
          </thead>
          <tbody>
            {users.map((u) => (
              <tr key={u.id}>
                <td className="cell-primary">{u.userName}</td>
                <td>{u.email}</td>
                <td>
                  <span style={{ display: 'inline-flex', gap: '0.25rem', flexWrap: 'wrap' }}>
                    {u.roles.map((role) => (
                      <span key={role} style={{ padding: '0.25rem 0.5rem', borderRadius: '0.375rem', fontSize: '0.75rem', fontWeight: 500, background: 'rgba(34 211 238 / 0.2)', color: 'rgb(34 211 238)' }}>
                        {role}
                      </span>
                    ))}
                  </span>
                </td>
                <td style={{ color: 'rgb(100 116 139)', fontSize: '0.875rem' }}>{new Date(u.createdAtUtc).toLocaleString()}</td>
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
