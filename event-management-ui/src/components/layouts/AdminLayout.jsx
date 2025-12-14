import { Link, useNavigate, useLocation } from 'react-router-dom'
import { useAuth } from '../../contexts/AuthContext'
import './AdminLayout.css'

const AdminLayout = ({ children }) => {
  const { logout } = useAuth()
  const navigate = useNavigate()
  const location = useLocation()

  const handleLogout = () => {
    logout()
    navigate('/admin/login')
  }

  const isActive = (path) => location.pathname === path

  return (
    <div className="admin-layout">
      <aside className="admin-sidebar">
        <div className="sidebar-header">
          <h2>Admin Panel</h2>
        </div>
        <nav className="sidebar-nav">
          <Link 
            to="/admin" 
            className={`sidebar-link ${isActive('/admin') ? 'active' : ''}`}
          >
            Dashboard
          </Link>
          <Link 
            to="/admin/events" 
            className={`sidebar-link ${isActive('/admin/events') ? 'active' : ''}`}
          >
            Events
          </Link>
          <Link 
            to="/admin/users" 
            className={`sidebar-link ${isActive('/admin/users') ? 'active' : ''}`}
          >
            Users
          </Link>
          <Link 
            to="/admin/bookings" 
            className={`sidebar-link ${isActive('/admin/bookings') ? 'active' : ''}`}
          >
            Bookings
          </Link>
        </nav>
        <div className="sidebar-footer">
          <button onClick={handleLogout} className="btn btn-secondary">
            Logout
          </button>
        </div>
      </aside>
      <main className="admin-main">
        <div className="admin-content">
          {children}
        </div>
      </main>
    </div>
  )
}

export default AdminLayout

