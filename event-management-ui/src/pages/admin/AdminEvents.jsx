import { useState, useEffect } from 'react'
import { useNavigate } from 'react-router-dom'
import api from '../../services/api'
import './AdminEvents.css'

const AdminEvents = () => {
  const [events, setEvents] = useState([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState('')
  const [showForm, setShowForm] = useState(false)
  const [editingEvent, setEditingEvent] = useState(null)
  const [formData, setFormData] = useState({
    title: '',
    description: '',
    eventDate: '',
    eventTime: '',
    venue: '',
    capacity: ''
  })
  const [submitting, setSubmitting] = useState(false)

  const navigate = useNavigate()

  useEffect(() => {
    fetchEvents()
  }, [])

  const fetchEvents = async () => {
    try {
      setLoading(true)
      const response = await api.get('/api/admin/events')
      
      if (response.data.success) {
        setEvents(response.data.data || [])
      } else {
        setError(response.data.message || 'Failed to load events')
      }
    } catch (error) {
      setError(error.response?.data?.message || 'Failed to load events')
    } finally {
      setLoading(false)
    }
  }

  const handleInputChange = (e) => {
    const { name, value } = e.target
    setFormData(prev => ({
      ...prev,
      [name]: value
    }))
  }

  const handleCreateEvent = () => {
    setEditingEvent(null)
    setFormData({
      title: '',
      description: '',
      eventDate: '',
      eventTime: '',
      venue: '',
      capacity: ''
    })
    setShowForm(true)
  }

  const handleEditEvent = (event) => {
    setEditingEvent(event)
    setFormData({
      title: event.title,
      description: event.description || '',
      eventDate: event.eventDate,
      eventTime: event.eventTime,
      venue: event.venue,
      capacity: event.capacity.toString()
    })
    setShowForm(true)
  }

  const handleSubmit = async (e) => {
    e.preventDefault()
    setSubmitting(true)
    setError('')

    try {
      if (!formData.title || !formData.eventDate || !formData.eventTime || !formData.venue || !formData.capacity) {
        setError('Please fill in all required fields')
        setSubmitting(false)
        return
      }

      const capacity = parseInt(formData.capacity, 10)
      if (isNaN(capacity) || capacity < 1) {
        setError('Capacity must be a number greater than 0')
        setSubmitting(false)
        return
      }

      const selectedDate = new Date(formData.eventDate)
      const today = new Date()
      today.setHours(0, 0, 0, 0)
      if (selectedDate < today) {
        setError('Event date must be today or in the future')
        setSubmitting(false)
        return
      }

      const timeValue = formData.eventTime
      const timeFormatted = timeValue.length === 5 ? `${timeValue}:00` : timeValue
      
      const requestData = {
        title: formData.title.trim(),
        description: formData.description?.trim() || null,
        eventDate: formData.eventDate, // DateOnly format: YYYY-MM-DD
        eventTime: timeFormatted, // TimeOnly format: HH:mm:ss
        venue: formData.venue.trim(),
        capacity: capacity // Integer
      }

      if (editingEvent) {
        const response = await api.put(`/api/admin/events/${editingEvent.id}`, requestData)
        if (response.data.success) {
          setShowForm(false)
          fetchEvents()
        } else {
          const errors = response.data.errors || []
          const errorMessage = errors.length > 0 
            ? errors.join(', ') 
            : response.data.message || 'Failed to update event'
          setError(errorMessage)
        }
      } else {
        const response = await api.post('/api/admin/events', requestData)
        if (response.data.success) {
          setShowForm(false)
          fetchEvents()
        } else {
          // Handle validation errors
          const errors = response.data.errors || []
          const errorMessage = errors.length > 0 
            ? errors.join(', ') 
            : response.data.message || 'Failed to create event'
          setError(errorMessage)
        }
      }
    } catch (error) {
      if (error.response?.data?.errors && error.response.data.errors.length > 0) {
        setError(error.response.data.errors.join(', '))
      } else {
        setError(error.response?.data?.message || 'Failed to save event')
      }
    } finally {
      setSubmitting(false)
    }
  }

  const handleDeleteEvent = async (id) => {
    if (!window.confirm('Are you sure you want to delete this event?')) {
      return
    }

    try {
      const response = await api.delete(`/api/admin/events/${id}`)
      if (response.data.success) {
        fetchEvents()
      } else {
        setError(response.data.message || 'Failed to delete event')
      }
    } catch (error) {
      setError(error.response?.data?.message || 'Failed to delete event')
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

  const getEventStatus = (eventDate) => {
    const today = new Date()
    const event = new Date(eventDate)
    return event < today ? 'Completed' : 'Upcoming'
  }

  if (loading) {
    return (
      <div className="admin-events">
        <div className="spinner"></div>
      </div>
    )
  }

  return (
    <div className="admin-events">
      <div className="admin-events-header">
        <h1>Events Management</h1>
        <button onClick={handleCreateEvent} className="btn btn-primary">
          Create New Event
        </button>
      </div>

      {error && <div className="alert alert-error">{error}</div>}

      {showForm && (
        <div className="modal-overlay" onClick={() => setShowForm(false)}>
          <div className="modal-content" onClick={(e) => e.stopPropagation()}>
            <h2>{editingEvent ? 'Edit Event' : 'Create New Event'}</h2>
            <form onSubmit={handleSubmit}>
              <div className="form-group">
                <label htmlFor="title">Title *</label>
                <input
                  type="text"
                  id="title"
                  name="title"
                  value={formData.title}
                  onChange={handleInputChange}
                  required
                  disabled={submitting}
                />
              </div>

              <div className="form-group">
                <label htmlFor="description">Description</label>
                <textarea
                  id="description"
                  name="description"
                  value={formData.description}
                  onChange={handleInputChange}
                  rows="4"
                  disabled={submitting}
                />
              </div>

              <div className="form-row">
                <div className="form-group">
                  <label htmlFor="eventDate">Date *</label>
                  <input
                    type="date"
                    id="eventDate"
                    name="eventDate"
                    value={formData.eventDate}
                    onChange={handleInputChange}
                    required
                    disabled={submitting}
                  />
                </div>

                <div className="form-group">
                  <label htmlFor="eventTime">Time *</label>
                  <input
                    type="time"
                    id="eventTime"
                    name="eventTime"
                    value={formData.eventTime}
                    onChange={handleInputChange}
                    required
                    disabled={submitting}
                  />
                </div>
              </div>

              <div className="form-group">
                <label htmlFor="venue">Venue *</label>
                <input
                  type="text"
                  id="venue"
                  name="venue"
                  value={formData.venue}
                  onChange={handleInputChange}
                  required
                  disabled={submitting}
                />
              </div>

              <div className="form-group">
                <label htmlFor="capacity">Capacity *</label>
                <input
                  type="number"
                  id="capacity"
                  name="capacity"
                  value={formData.capacity}
                  onChange={handleInputChange}
                  min="1"
                  required
                  disabled={submitting}
                />
              </div>

              <div className="form-actions">
                <button
                  type="button"
                  onClick={() => setShowForm(false)}
                  className="btn btn-secondary"
                  disabled={submitting}
                >
                  Cancel
                </button>
                <button
                  type="submit"
                  className="btn btn-primary"
                  disabled={submitting}
                >
                  {submitting ? 'Saving...' : editingEvent ? 'Update Event' : 'Create Event'}
                </button>
              </div>
            </form>
          </div>
        </div>
      )}

      <div className="table-container">
        <table className="table">
          <thead>
            <tr>
              <th>Event Name</th>
              <th>Date</th>
              <th>Time</th>
              <th>Venue</th>
              <th>Capacity</th>
              <th>Status</th>
              <th>Actions</th>
            </tr>
          </thead>
          <tbody>
            {events.length === 0 ? (
              <tr>
                <td colSpan="7" style={{ textAlign: 'center', padding: '40px' }}>
                  No events found. Create your first event!
                </td>
              </tr>
            ) : (
              events.map((event) => (
                <tr key={event.id}>
                  <td>{event.title}</td>
                  <td>{formatDate(event.eventDate)}</td>
                  <td>{formatTime(event.eventTime)}</td>
                  <td>{event.venue}</td>
                  <td>{event.capacity}</td>
                  <td>
                    <span className={`status-badge ${getEventStatus(event.eventDate).toLowerCase()}`}>
                      {getEventStatus(event.eventDate)}
                    </span>
                  </td>
                  <td>
                    <div className="action-buttons">
                      <button
                        onClick={() => handleEditEvent(event)}
                        className="btn btn-secondary btn-sm"
                      >
                        Edit
                      </button>
                      <button
                        onClick={() => handleDeleteEvent(event.id)}
                        className="btn btn-danger btn-sm"
                      >
                        Delete
                      </button>
                    </div>
                  </td>
                </tr>
              ))
            )}
          </tbody>
        </table>
      </div>
    </div>
  )
}

export default AdminEvents

