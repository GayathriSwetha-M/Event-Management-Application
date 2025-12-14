import { Link, useNavigate } from 'react-router-dom'
import { useAuth } from '../../contexts/AuthContext'
import './UserLayout.css'

const UserLayout = ({ children }) => {
  const { user, logout } = useAuth()
  const navigate = useNavigate()

  const handleLogout = () => {
    logout()
    navigate('/login')
  }

  return (
    <div className="user-layout">
      <nav className="user-nav">
        <div className="nav-container">
          <Link to="/" className="nav-logo">
            EventHub
          </Link>
          <div className="nav-links">
            <Link to="/" className="nav-link">Home</Link>
            <Link to="/my-bookings" className="nav-link">My Bookings</Link>
            <div className="nav-user">
              <span className="nav-user-name">{user?.name}</span>
              <button onClick={handleLogout} className="btn btn-secondary btn-sm">
                Logout
              </button>
            </div>
          </div>
        </div>
      </nav>
      <main className="user-main">
        {children}
      </main>
    </div>
  )
}

export default UserLayout

