import { useState, useEffect } from 'react'
import api from '../../services/api'
import './MyBookings.css'

const MyBookings = () => {
  const [bookings, setBookings] = useState([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState('')

  useEffect(() => {
    fetchBookings()
  }, [])

  const fetchBookings = async () => {
    try {
      setLoading(true)
      const response = await api.get('/api/bookings/my-bookings')
      
      if (response.data.success) {
        setBookings(response.data.data || [])
      } else {
        setError(response.data.message || 'Failed to load bookings')
      }
    } catch (error) {
      setError(error.response?.data?.message || 'Failed to load bookings. Please try again.')
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
      return 'status-cancelled'
    }
    
    if (event < today) {
      return 'status-completed'
    }
    
    return 'status-upcoming'
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

  if (loading) {
    return (
      <div className="container">
        <div className="spinner"></div>
      </div>
    )
  }

  return (
    <div className="container">
      <div className="my-bookings-header">
        <h1>My Bookings</h1>
        <p>View all your event bookings</p>
      </div>

      {error && <div className="alert alert-error">{error}</div>}

      {bookings.length === 0 && !loading && (
        <div className="no-bookings">
          <p>You haven't booked any events yet.</p>
        </div>
      )}

      <div className="bookings-list">
        {bookings.map((booking) => (
          <div key={booking.id} className="booking-card">
            <div className="booking-card-header">
              <h3>{booking.eventTitle}</h3>
              <span className={`status-badge ${getStatusClass(booking.status, booking.eventDate)}`}>
                {getStatusText(booking.status, booking.eventDate)}
              </span>
            </div>
            <div className="booking-card-body">
              <div className="booking-info">
                <div className="booking-info-item">
                  <span className="booking-label">Event Date:</span>
                  <span className="booking-value">{formatDate(booking.eventDate)}</span>
                </div>
                <div className="booking-info-item">
                  <span className="booking-label">Event Time:</span>
                  <span className="booking-value">{formatTime(booking.eventTime)}</span>
                </div>
                <div className="booking-info-item">
                  <span className="booking-label">Venue:</span>
                  <span className="booking-value">{booking.venue}</span>
                </div>
                <div className="booking-info-item">
                  <span className="booking-label">Number of Seats:</span>
                  <span className="booking-value">{booking.numberOfSeats || 1}</span>
                </div>
                <div className="booking-info-item">
                  <span className="booking-label">Booking ID:</span>
                  <span className="booking-value">{booking.id}</span>
                </div>
                <div className="booking-info-item">
                  <span className="booking-label">Booked On:</span>
                  <span className="booking-value">
                    {new Date(booking.createdAt).toLocaleDateString('en-US', {
                      year: 'numeric',
                      month: 'long',
                      day: 'numeric',
                      hour: '2-digit',
                      minute: '2-digit'
                    })}
                  </span>
                </div>
              </div>
            </div>
          </div>
        ))}
      </div>
    </div>
  )
}

export default MyBookings

