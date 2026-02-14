import { useState } from 'react'
import { Link, useNavigate } from 'react-router-dom'
import { authApi } from '../services/api'
import { useAuthStore } from '../store/authStore'

export default function Login() {
  const [userName, setUserName] = useState('')
  const [password, setPassword] = useState('')
  const [error, setError] = useState('')
  const [loading, setLoading] = useState(false)
  const setAuth = useAuthStore((s) => s.setAuth)
  const navigate = useNavigate()

  const submit = async (e: React.FormEvent) => {
    e.preventDefault()
    setError('')
    setLoading(true)
    try {
      const { data } = await authApi.login(userName, password)
      setAuth(data.accessToken, data.userName, data.roles ?? [])
      navigate('/', { replace: true })
    } catch (err: unknown) {
      const res = (err as { response?: { data?: { errors?: string[] } } })?.response?.data
      setError(Array.isArray(res?.errors) ? res.errors.join(' ') : 'Login failed')
    } finally {
      setLoading(false)
    }
  }

  return (
    <div className="auth-page">
      <form onSubmit={submit} className="auth-card">
        <h1 className="auth-title">
          <span>◆</span>
          PulseMonitor
        </h1>
        <p className="auth-subtitle">Sign in to the monitoring dashboard</p>
        {error && <div className="auth-error">{error}</div>}
        <label className="auth-label">User name</label>
        <input
          type="text"
          placeholder="e.g. admin"
          value={userName}
          onChange={(e) => setUserName(e.target.value)}
          className="auth-input"
          required
        />
        <label className="auth-label">Password</label>
        <input
          type="password"
          placeholder="••••••••"
          value={password}
          onChange={(e) => setPassword(e.target.value)}
          className="auth-input"
          required
        />
        <button type="submit" disabled={loading} className="auth-submit">
          {loading ? 'Signing in...' : 'Sign in'}
        </button>
        <p className="auth-footer">
          No account? <Link to="/register">Register</Link>
        </p>
      </form>
    </div>
  )
}
