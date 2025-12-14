import { useState, useEffect } from 'react'
import api from '../../services/api'
import './AdminUsers.css'

const AdminUsers = () => {
  const [users, setUsers] = useState([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState('')

  useEffect(() => {
    fetchUsers()
  }, [])

  const fetchUsers = async () => {
    try {
      setLoading(true)
      const response = await api.get('/api/admin/users')
      
      if (response.data.success) {
        setUsers(response.data.data || [])
      } else {
        setError(response.data.message || 'Failed to load users')
      }
    } catch (error) {
      setError(error.response?.data?.message || 'Failed to load users')
    } finally {
      setLoading(false)
    }
  }

  const formatDate = (dateString) => {
    const date = new Date(dateString)
    return date.toLocaleDateString('en-US', {
      year: 'numeric',
      month: 'long',
      day: 'numeric'
    })
  }

  if (loading) {
    return (
      <div className="admin-users">
        <div className="spinner"></div>
      </div>
    )
  }

  return (
    <div className="admin-users">
      <h1>Users</h1>
      <p className="page-subtitle">Manage all registered users</p>

      {error && <div className="alert alert-error">{error}</div>}

      <div className="table-container">
        <table className="table">
          <thead>
            <tr>
              <th>Name</th>
              <th>Email/Phone</th>
              <th>Role</th>
              <th>Status</th>
              <th>Created At</th>
            </tr>
          </thead>
          <tbody>
            {users.length === 0 ? (
              <tr>
                <td colSpan="5" style={{ textAlign: 'center', padding: '40px' }}>
                  No users found.
                </td>
              </tr>
            ) : (
              users.map((user) => (
                <tr key={user.id}>
                  <td>{user.name}</td>
                  <td>{user.emailOrPhone}</td>
                  <td>
                    <span className={`role-badge ${user.role}`}>
                      {user.role}
                    </span>
                  </td>
                  <td>
                    <span className="status-badge active">
                      Active
                    </span>
                  </td>
                  <td>{formatDate(user.createdAt)}</td>
                </tr>
              ))
            )}
          </tbody>
        </table>
      </div>
    </div>
  )
}

export default AdminUsers

