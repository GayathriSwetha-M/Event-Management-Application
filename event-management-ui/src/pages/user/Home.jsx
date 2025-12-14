import { useState, useEffect } from 'react'
import { Link } from 'react-router-dom'
import api from '../../services/api'
import './Home.css'

const Home = () => {
  const [events, setEvents] = useState([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState('')

  useEffect(() => {
    fetchEvents()
  }, [])

  const fetchEvents = async () => {
    try {
      setLoading(true)
      const response = await api.get('/api/events')
      
      if (response.data.success) {
        setEvents(response.data.data || [])
      } else {
        setError(response.data.message || 'Failed to load events')
      }
    } catch (error) {
      setError(error.response?.data?.message || 'Failed to load events. Please try again.')
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
      <div className="home-header">
        <h1>Upcoming Events</h1>
        <p>Discover and book your favorite events</p>
      </div>

      {error && <div className="alert alert-error">{error}</div>}

      {events.length === 0 && !loading && (
        <div className="no-events">
          <p>No upcoming events available at the moment.</p>
        </div>
      )}

      <div className="events-grid">
        {events.map((event) => (
          <div key={event.id} className="event-card">
            <div className="event-card-header">
              <h3>{event.title}</h3>
            </div>
            <div className="event-card-body">
              <div className="event-info">
                <div className="event-info-item">
                  <span className="event-label">Date:</span>
                  <span className="event-value">{formatDate(event.eventDate)}</span>
                </div>
                <div className="event-info-item">
                  <span className="event-label">Time:</span>
                  <span className="event-value">{formatTime(event.eventTime)}</span>
                </div>
                <div className="event-info-item">
                  <span className="event-label">Venue:</span>
                  <span className="event-value">{event.venue}</span>
                </div>
                <div className="event-info-item">
                  <span className="event-label">Available:</span>
                  <span className="event-value">{event.availableSlots} / {event.capacity} slots</span>
                </div>
              </div>
              <p className="event-description">
                {event.description?.substring(0, 100)}
                {event.description?.length > 100 && '...'}
              </p>
            </div>
            <div className="event-card-footer">
              <Link to={`/events/${event.id}`} className="btn btn-primary">
                View Details
              </Link>
            </div>
          </div>
        ))}
      </div>
    </div>
  )
}

export default Home

