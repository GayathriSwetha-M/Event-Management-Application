import { useState, useEffect } from 'react'
import api from '../../services/api'
import './AdminDashboard.css'

const AdminDashboard = () => {
  const [overview, setOverview] = useState({
    totalEvents: 0,
    totalUsers: 0,
    totalBookings: 0
  })
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState('')

  useEffect(() => {
    fetchOverview()
  }, [])

  const fetchOverview = async () => {
    try {
      setLoading(true)
      const response = await api.get('/api/admin/overview')
      
      if (response.data.success) {
        setOverview(response.data.data)
      } else {
        setError(response.data.message || 'Failed to load dashboard data')
      }
    } catch (error) {
      setError(error.response?.data?.message || 'Failed to load dashboard data')
    } finally {
      setLoading(false)
    }
  }

  if (loading) {
    return (
      <div className="admin-dashboard">
        <div className="spinner"></div>
      </div>
    )
  }

  return (
    <div className="admin-dashboard">
      <h1>Dashboard</h1>
      <p className="dashboard-subtitle">Overview of your event management system</p>

      {error && <div className="alert alert-error">{error}</div>}

      <div className="dashboard-cards">
        <div className="dashboard-card card-events">
          <div className="card-icon">
            <svg width="48" height="48" viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg">
              <path d="M8 2V6M16 2V6M3 10H21M5 4H19C20.1046 4 21 4.89543 21 6V20C21 21.1046 20.1046 22 19 22H5C3.89543 22 3 21.1046 3 20V6C3 4.89543 3.89543 4 5 4Z" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round"/>
            </svg>
          </div>
          <div className="card-content">
            <h3>Total Events</h3>
            <p className="card-value">{overview.totalEvents}</p>
          </div>
        </div>

        <div className="dashboard-card card-users">
          <div className="card-icon">
            <svg width="48" height="48" viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg">
              <path d="M17 21V19C17 17.9391 16.5786 16.9217 15.8284 16.1716C15.0783 15.4214 14.0609 15 13 15H5C3.93913 15 2.92172 15.4214 2.17157 16.1716C1.42143 16.9217 1 17.9391 1 19V21M23 21V19C22.9993 18.1137 22.7044 17.2528 22.1614 16.5523C21.6184 15.8519 20.8581 15.3516 20 15.13M16 3.13C16.8604 3.35031 17.623 3.85071 18.1676 4.55232C18.7122 5.25392 19.0078 6.11683 19.0078 7.005C19.0078 7.89318 18.7122 8.75608 18.1676 9.45769C17.623 10.1593 16.8604 10.6597 16 10.88M13 7C13 9.20914 11.2091 11 9 11C6.79086 11 5 9.20914 5 7C5 4.79086 6.79086 3 9 3C11.2091 3 13 4.79086 13 7Z" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round"/>
            </svg>
          </div>
          <div className="card-content">
            <h3>Total Users</h3>
            <p className="card-value">{overview.totalUsers}</p>
          </div>
        </div>

        <div className="dashboard-card card-bookings">
          <div className="card-icon">
            <svg width="48" height="48" viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg">
              <path d="M9 5H7C5.89543 5 5 5.89543 5 7V19C5 20.1046 5.89543 21 7 21H17C18.1046 21 19 20.1046 19 19V7C19 5.89543 18.1046 5 17 5H15M9 5C9 6.10457 9.89543 7 11 7H13C14.1046 7 15 6.10457 15 5M9 5C9 3.89543 9.89543 3 11 3H13C14.1046 3 15 3.89543 15 5M12 12H15M12 16H15M9 12H9.01M9 16H9.01" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round"/>
            </svg>
          </div>
          <div className="card-content">
            <h3>Total Bookings</h3>
            <p className="card-value">{overview.totalBookings}</p>
          </div>
        </div>
      </div>
    </div>
  )
}

export default AdminDashboard

