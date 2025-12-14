import { useState, useEffect } from 'react'
import { useParams, useNavigate } from 'react-router-dom'
import api from '../../services/api'
import './BookingConfirmation.css'

const BookingConfirmation = () => {
  const { bookingId } = useParams()
  const navigate = useNavigate()
  const [booking, setBooking] = useState(null)
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState('')

  useEffect(() => {
    setLoading(false)
  }, [bookingId])

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

  if (loading) {
    return (
      <div className="container">
        <div className="spinner"></div>
      </div>
    )
  }

  return (
    <div className="container">
      <div className="confirmation-container">
        <div className="confirmation-card">
          <div className="confirmation-icon">
            <svg width="80" height="80" viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg">
              <circle cx="12" cy="12" r="10" stroke="#27ae60" strokeWidth="2" fill="none"/>
              <path d="M8 12l2 2 4-4" stroke="#27ae60" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round"/>
            </svg>
          </div>
          
          <h1>Booking Successful!</h1>
          <p className="confirmation-message">
            Your event has been successfully booked. We've sent a confirmation to your email.
          </p>

          <div className="booking-info-card">
            <h2>Booking Details</h2>
            <div className="booking-info-item">
              <span className="booking-label">Booking ID:</span>
              <span className="booking-value">{bookingId}</span>
            </div>
            <div className="booking-info-item">
              <span className="booking-label">Status:</span>
              <span className="booking-value status-booked">Confirmed</span>
            </div>
          </div>

          <div className="confirmation-actions">
            <button 
              onClick={() => navigate('/my-bookings')} 
              className="btn btn-primary"
            >
              View My Bookings
            </button>
            <button 
              onClick={() => navigate('/')} 
              className="btn btn-secondary"
            >
              Browse More Events
            </button>
          </div>
        </div>
      </div>
    </div>
  )
}

export default BookingConfirmation

