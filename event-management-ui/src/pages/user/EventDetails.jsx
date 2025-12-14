import { useState, useEffect } from 'react'
import { useParams, useNavigate } from 'react-router-dom'
import api from '../../services/api'
import './EventDetails.css'

const EventDetails = () => {
  const { id } = useParams()
  const navigate = useNavigate()
  const [event, setEvent] = useState(null)
  const [loading, setLoading] = useState(true)
  const [booking, setBooking] = useState(false)
  const [error, setError] = useState('')
  const [numberOfSeats, setNumberOfSeats] = useState(1)

  useEffect(() => {
    fetchEventDetails()
  }, [id])

  const fetchEventDetails = async () => {
    try {
      setLoading(true)
      const response = await api.get(`/api/events/${id}`)
      
      if (response.data.success) {
        setEvent(response.data.data)
      } else {
        setError(response.data.message || 'Event not found')
      }
    } catch (error) {
      setError(error.response?.data?.message || 'Failed to load event details')
    } finally {
      setLoading(false)
    }
  }

  const handleBookEvent = async () => {
    if (!event || event.availableSlots === 0) {
      setError('No available slots for this event')
      return
    }

    if (numberOfSeats < 1) {
      setError('Number of seats must be at least 1')
      return
    }

    if (numberOfSeats > event.availableSlots) {
      setError(`Only ${event.availableSlots} seat(s) available. Please select fewer seats.`)
      return
    }

    try {
      setBooking(true)
      setError('')
      const response = await api.post(`/api/events/${id}/book`, {
        numberOfSeats: numberOfSeats
      })
      
      if (response.data.success) {
        const bookingId = response.data.data?.id || response.data.data?.BookingId
        navigate(`/booking-confirmation/${bookingId}`)
      } else {
        const errors = response.data.errors || []
        const errorMessage = errors.length > 0 
          ? errors.join(', ') 
          : response.data.message || 'Failed to book event'
        setError(errorMessage)
      }
    } catch (error) {
      if (error.response?.data?.errors && error.response.data.errors.length > 0) {
        setError(error.response.data.errors.join(', '))
      } else {
        setError(error.response?.data?.message || 'Failed to book event. Please try again.')
      }
    } finally {
      setBooking(false)
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
    return timeString.substring(0, 5)
  }

  if (loading) {
    return (
      <div className="container">
        <div className="spinner"></div>
      </div>
    )
  }

  if (!event) {
    return (
      <div className="container">
        <div className="alert alert-error">
          {error || 'Event not found'}
        </div>
        <button onClick={() => navigate('/')} className="btn btn-primary">
          Back to Events
        </button>
      </div>
    )
  }

  return (
    <div className="container">
      <button onClick={() => navigate('/')} className="btn btn-secondary back-btn">
        ← Back to Events
      </button>

      <div className="event-details">
        <div className="event-details-header">
          <h1>{event.title}</h1>
        </div>

        {error && <div className="alert alert-error">{error}</div>}

        <div className="event-details-content">
          <div className="event-details-main">
            <div className="event-section">
              <h2>Description</h2>
              <p>{event.description || 'No description available.'}</p>
            </div>

            <div className="event-section">
              <h2>Event Information</h2>
              <div className="info-grid">
                <div className="info-item">
                  <span className="info-label">Date</span>
                  <span className="info-value">{formatDate(event.eventDate)}</span>
                </div>
                <div className="info-item">
                  <span className="info-label">Time</span>
                  <span className="info-value">{formatTime(event.eventTime)}</span>
                </div>
                <div className="info-item">
                  <span className="info-label">Venue</span>
                  <span className="info-value">{event.venue}</span>
                </div>
                <div className="info-item">
                  <span className="info-label">Capacity</span>
                  <span className="info-value">{event.capacity} attendees</span>
                </div>
                <div className="info-item">
                  <span className="info-label">Booked</span>
                  <span className="info-value">{event.bookedCount} / {event.capacity}</span>
                </div>
                <div className="info-item">
                  <span className="info-label">Available Slots</span>
                  <span className={`info-value ${event.availableSlots === 0 ? 'text-danger' : 'text-success'}`}>
                    {event.availableSlots}
                  </span>
                </div>
              </div>
            </div>
          </div>

          <div className="event-details-sidebar">
            <div className="booking-card">
              <h3>Book This Event</h3>
              <div className="booking-info">
                <p><strong>Available Slots:</strong> {event.availableSlots}</p>
                {event.availableSlots === 0 && (
                  <p className="text-danger">Event is fully booked</p>
                )}
              </div>
              
              {event.availableSlots > 0 && (
                <div className="form-group" style={{ marginTop: '20px' }}>
                  <label htmlFor="numberOfSeats">Number of Seats *</label>
                  <input
                    type="number"
                    id="numberOfSeats"
                    min="1"
                    max={event.availableSlots}
                    value={numberOfSeats}
                    onChange={(e) => {
                      const value = parseInt(e.target.value, 10) || 1
                      const maxSeats = Math.min(value, event.availableSlots)
                      setNumberOfSeats(Math.max(1, maxSeats))
                    }}
                    disabled={booking}
                    className="form-control"
                    style={{ width: '100%', padding: '10px', fontSize: '16px', border: '2px solid #ddd', borderRadius: '6px' }}
                  />
                  <small style={{ color: '#666', fontSize: '14px', marginTop: '5px', display: 'block' }}>
                    Maximum {event.availableSlots} seat(s) available
                  </small>
                </div>
              )}
              
              <button
                onClick={handleBookEvent}
                className="btn btn-primary btn-block"
                disabled={booking || event.availableSlots === 0}
                style={{ marginTop: '20px' }}
              >
                {booking ? 'Booking...' : event.availableSlots === 0 ? 'Fully Booked' : `Book ${numberOfSeats} Seat${numberOfSeats > 1 ? 's' : ''}`}
              </button>
            </div>
          </div>
        </div>
      </div>
    </div>
  )
}

export default EventDetails

