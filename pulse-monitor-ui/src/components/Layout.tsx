import { Outlet, Link, useNavigate, useLocation } from 'react-router-dom'
import { useAuthStore } from '../store/authStore'

export default function Layout() {
  const { userName, logout, isAdmin } = useAuthStore()
  const navigate = useNavigate()
  const location = useLocation()

  const handleLogout = () => {
    logout()
    navigate('/login')
  }

  const isActive = (to: string) =>
    location.pathname === to || (to !== '/' && location.pathname.startsWith(to))

  return (
    <div style={{ minHeight: '100vh', background: 'rgb(15 23 42)', color: 'rgb(248 250 252)' }}>
      <nav className="app-nav">
        <div style={{ display: 'flex', alignItems: 'center', gap: '1rem', flexWrap: 'wrap' }}>
          <Link to="/" className="app-nav-brand">
            <span>◆</span>
            PulseMonitor
          </Link>
          <div className="app-nav-links">
            <Link to="/" className={`app-nav-link ${isActive('/') && location.pathname === '/' ? 'active' : ''}`}>
              Dashboard
            </Link>
            <Link to="/reports" className={`app-nav-link ${isActive('/reports') ? 'active' : ''}`}>
              Reports
            </Link>
            <Link to="/alerts" className={`app-nav-link ${isActive('/alerts') ? 'active' : ''}`}>
              Alerts
            </Link>
            {isAdmin() && (
              <Link to="/users" className={`app-nav-link ${isActive('/users') ? 'active' : ''}`}>
                Users
              </Link>
            )}
            <a
              href={`${import.meta.env.VITE_API_URL || (import.meta.env.DEV ? 'http://localhost:5000' : '')}/hangfire`}
              target="_blank"
              rel="noopener noreferrer"
              className="app-nav-link"
            >
              Jobs
            </a>
          </div>
        </div>
        <div style={{ display: 'flex', alignItems: 'center', gap: '0.75rem' }}>
          <span className="app-nav-user">{userName}</span>
          <button type="button" onClick={handleLogout} className="app-nav-logout">
            Logout
          </button>
        </div>
      </nav>
      <main style={{ padding: '1.5rem', maxWidth: '80rem', margin: '0 auto' }}>
        <Outlet />
      </main>
    </div>
  )
}
