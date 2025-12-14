import { useState, useEffect, useMemo } from 'react'
import api from '../../services/api'
import './AdminBookings.css'

const AdminBookings = () => {
  const [bookings, setBookings] = useState([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState('')
  const [groupBy, setGroupBy] = useState('event') // 'event' or 'user'

  useEffect(() => {
    fetchBookings()
  }, [])

  const fetchBookings = async () => {
    try {
      setLoading(true)
      const response = await api.get('/api/admin/bookings')
      
      if (response.data.success) {
        setBookings(response.data.data || [])
      } else {
        setError(response.data.message || 'Failed to load bookings')
      }
    } catch (error) {
      setError(error.response?.data?.message || 'Failed to load bookings')
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

  const formatTime = (timeString) => {
    if (!timeString) return ''
    return timeString.substring(0, 5)
  }

  const getStatusClass = (status, eventDate) => {
    const today = new Date()
    const event = new Date(eventDate)
    
    if (status === 'cancelled') {
      return 'cancelled'
    }
    
    if (event < today) {
      return 'completed'
    }
    
    return 'upcoming'
  }

  const getStatusText = (status, eventDate) => {
    const today = new Date()
    const event = new Date(eventDate)
    
    if (status === 'cancelled') {
      return 'Cancelled'
    }
    
    if (event < today) {
      return 'Completed'
    }
    
    return 'Upcoming'
  }

  // Group bookings by event or user
  const groupedBookings = useMemo(() => {
    if (!bookings || bookings.length === 0) return {}

    if (groupBy === 'event') {
      // Group by event
      return bookings.reduce((acc, booking) => {
        const eventKey = booking.eventId || booking.eventTitle
        if (!acc[eventKey]) {
          acc[eventKey] = {
            eventId: booking.eventId,
            eventTitle: booking.eventTitle,
            eventDate: booking.eventDate,
            eventTime: booking.eventTime,
            venue: booking.venue,
            bookings: []
          }
        }
        acc[eventKey].bookings.push(booking)
        return acc
      }, {})
    } else {
      // Group by user
      return bookings.reduce((acc, booking) => {
        const userKey = booking.userId || booking.userName
        if (!acc[userKey]) {
          acc[userKey] = {
            userId: booking.userId,
            userName: booking.userName,
            userEmail: booking.userEmail,
            bookings: []
          }
        }
        acc[userKey].bookings.push(booking)
        return acc
      }, {})
    }
  }, [bookings, groupBy])

  if (loading) {
    return (
      <div className="admin-bookings">
        <div className="spinner"></div>
      </div>
    )
  }

  return (
    <div className="admin-bookings">
      <div className="admin-bookings-header">
        <div>
          <h1>Bookings</h1>
          <p className="page-subtitle">View all event bookings</p>
        </div>
        <div className="grouping-selector">
          <label>Group by:</label>
          <div className="grouping-buttons">
            <button
              className={`grouping-btn ${groupBy === 'event' ? 'active' : ''}`}
              onClick={() => setGroupBy('event')}
            >
              By Event
            </button>
            <button
              className={`grouping-btn ${groupBy === 'user' ? 'active' : ''}`}
              onClick={() => setGroupBy('user')}
            >
              By User
            </button>
          </div>
        </div>
      </div>

      {error && <div className="alert alert-error">{error}</div>}

      {Object.keys(groupedBookings).length === 0 ? (
        <div className="no-bookings">
          <p>No bookings found.</p>
        </div>
      ) : (
        <div className="grouped-bookings">
          {Object.values(groupedBookings).map((group, groupIndex) => (
            <div key={groupIndex} className="booking-group">
              {groupBy === 'event' ? (
                <div className="group-header event-header">
                  <div className="group-header-content">
                    <h3>{group.eventTitle}</h3>
                    <div className="group-header-details">
                      <span>📅 {formatDate(group.eventDate)}</span>
                      <span>🕐 {formatTime(group.eventTime)}</span>
                      <span>📍 {group.venue}</span>
                      <span className="booking-count">{group.bookings.length} booking{group.bookings.length !== 1 ? 's' : ''}</span>
                    </div>
                  </div>
                </div>
              ) : (
                <div className="group-header user-header">
                  <div className="group-header-content">
                    <h3>{group.userName}</h3>
                    <div className="group-header-details">
                      <span>📧 {group.userEmail}</span>
                      <span className="booking-count">{group.bookings.length} booking{group.bookings.length !== 1 ? 's' : ''}</span>
                    </div>
                  </div>
                </div>
              )}

              <div className="group-bookings-table">
                <table className="table">
                  <thead>
                    <tr>
                      {groupBy === 'event' ? (
                        <>
                          <th>User</th>
                          <th>Seats</th>
                          <th>Booking Date</th>
                          <th>Status</th>
                        </>
                      ) : (
                        <>
                          <th>Event</th>
                          <th>Date</th>
                          <th>Time</th>
                          <th>Venue</th>
                          <th>Seats</th>
                          <th>Booking Date</th>
                          <th>Status</th>
                        </>
                      )}
                    </tr>
                  </thead>
                  <tbody>
                    {group.bookings.map((booking) => (
                      <tr key={booking.id}>
                        {groupBy === 'event' ? (
                          <>
                            <td>
                              <div>
                                <strong>{booking.userName}</strong>
                                <br />
                                <small style={{ color: '#666' }}>{booking.userEmail}</small>
                              </div>
                            </td>
                            <td>{booking.numberOfSeats || 1}</td>
                            <td>{formatDate(booking.createdAt)}</td>
                            <td>
                              <span className={`status-badge ${getStatusClass(booking.status, booking.eventDate)}`}>
                                {getStatusText(booking.status, booking.eventDate)}
                              </span>
                            </td>
                          </>
                        ) : (
                          <>
                            <td>
                              <strong>{booking.eventTitle}</strong>
                            </td>
                            <td>{formatDate(booking.eventDate)}</td>
                            <td>{formatTime(booking.eventTime)}</td>
                            <td>{booking.venue}</td>
                            <td>{booking.numberOfSeats || 1}</td>
                            <td>{formatDate(booking.createdAt)}</td>
                            <td>
                              <span className={`status-badge ${getStatusClass(booking.status, booking.eventDate)}`}>
                                {getStatusText(booking.status, booking.eventDate)}
                              </span>
                            </td>
                          </>
                        )}
                      </tr>
                    ))}
                  </tbody>
                </table>
              </div>
            </div>
          ))}
        </div>
      )}
    </div>
  )
}

export default AdminBookings

