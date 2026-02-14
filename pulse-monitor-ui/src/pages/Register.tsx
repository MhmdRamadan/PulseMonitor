import { useState } from 'react'
import { Link, useNavigate } from 'react-router-dom'
import { authApi } from '../services/api'
import { useAuthStore } from '../store/authStore'

export default function Register() {
  const [userName, setUserName] = useState('')
  const [email, setEmail] = useState('')
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
      const { data } = await authApi.register(userName, email, password)
      setAuth(data.accessToken, data.userName, data.roles)
      navigate('/')
    } catch (err: unknown) {
      const res = (err as { response?: { data?: { errors?: string[] } } })?.response?.data
      setError(Array.isArray(res?.errors) ? res.errors.join(' ') : 'Registration failed')
    } finally {
      setLoading(false)
    }
  }

  return (
    <div className="auth-page">
      <form onSubmit={submit} className="auth-card">
        <h1 className="auth-title">
          <span>◆</span>
          Register
        </h1>
        <p className="auth-subtitle">Create an account</p>
        {error && <div className="auth-error">{error}</div>}
        <label className="auth-label">User name</label>
        <input
          type="text"
          placeholder="e.g. jdoe"
          value={userName}
          onChange={(e) => setUserName(e.target.value)}
          className="auth-input"
          required
        />
        <label className="auth-label">Email</label>
        <input
          type="email"
          placeholder="you@example.com"
          value={email}
          onChange={(e) => setEmail(e.target.value)}
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
          minLength={6}
        />
        <button type="submit" disabled={loading} className="auth-submit">
          {loading ? 'Creating account...' : 'Register'}
        </button>
        <p className="auth-footer">
          Already have an account? <Link to="/login">Sign in</Link>
        </p>
      </form>
    </div>
  )
}
