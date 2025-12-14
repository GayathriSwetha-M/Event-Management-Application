import { useState } from 'react'
import { useNavigate } from 'react-router-dom'
import { useAuth } from '../../contexts/AuthContext'
import '../user/Auth.css'

const AdminLogin = () => {
  const [emailOrPhone, setEmailOrPhone] = useState('')
  const [password, setPassword] = useState('')
  const [error, setError] = useState('')
  const [loading, setLoading] = useState(false)
  const { login } = useAuth()
  const navigate = useNavigate()

  const handleSubmit = async (e) => {
    e.preventDefault()
    setError('')
    setLoading(true)

    if (!emailOrPhone || !password) {
      setError('Please fill in all fields')
      setLoading(false)
      return
    }

    const result = await login(emailOrPhone, password)
    
    if (result.success) {
      const user = JSON.parse(localStorage.getItem('user'))
      if (user?.role === 'admin') {
        navigate('/admin')
      } else {
        setError('Access denied. Admin credentials required.')
        localStorage.removeItem('token')
        localStorage.removeItem('user')
      }
    } else {
      setError(result.message || 'Login failed. Please check your credentials.')
    }
    
    setLoading(false)
  }

  return (
    <div className="auth-container">
      <div className="auth-card">
        <h1>Admin Login</h1>
        <p className="auth-subtitle">Please login with your admin credentials.</p>

        {error && <div className="alert alert-error">{error}</div>}

        <form onSubmit={handleSubmit}>
          <div className="form-group">
            <label htmlFor="emailOrPhone">Email or Phone</label>
            <input
              type="text"
              id="emailOrPhone"
              value={emailOrPhone}
              onChange={(e) => setEmailOrPhone(e.target.value)}
              placeholder="Enter your email or phone number"
              disabled={loading}
              required
            />
          </div>

          <div className="form-group">
            <label htmlFor="password">Password</label>
            <input
              type="password"
              id="password"
              value={password}
              onChange={(e) => setPassword(e.target.value)}
              placeholder="Enter your password"
              disabled={loading}
            />
          </div>

          <button 
            type="submit" 
            className="btn btn-primary btn-block"
            disabled={loading}
          >
            {loading ? 'Logging in...' : 'Login'}
          </button>
        </form>
      </div>
    </div>
  )
}

export default AdminLogin

