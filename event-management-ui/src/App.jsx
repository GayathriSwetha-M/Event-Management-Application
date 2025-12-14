import { Routes, Route, Navigate } from 'react-router-dom'
import { useAuth } from './contexts/AuthContext'

// User Pages
import Login from './pages/user/Login'
import Signup from './pages/user/Signup'
import Home from './pages/user/Home'
import EventDetails from './pages/user/EventDetails'
import BookingConfirmation from './pages/user/BookingConfirmation'
import MyBookings from './pages/user/MyBookings'

// Admin Pages
import AdminLogin from './pages/admin/AdminLogin'
import AdminDashboard from './pages/admin/AdminDashboard'
import AdminEvents from './pages/admin/AdminEvents'
import AdminUsers from './pages/admin/AdminUsers'
import AdminBookings from './pages/admin/AdminBookings'

// Layout Components
import UserLayout from './components/layouts/UserLayout'
import AdminLayout from './components/layouts/AdminLayout'

// Protected Route Component
const ProtectedRoute = ({ children, requireAdmin = false }) => {
  const { user, loading } = useAuth()

  if (loading) {
    return (
      <div style={{ display: 'flex', justifyContent: 'center', alignItems: 'center', height: '100vh' }}>
        <div className="spinner"></div>
      </div>
    )
  }

  if (!user) {
    return <Navigate to={requireAdmin ? '/admin/login' : '/login'} replace />
  }

  if (requireAdmin && user.role !== 'admin') {
    return <Navigate to="/" replace />
  }

  return children
}

function App() {
  return (
    <Routes>
      {/* Public Routes */}
      <Route path="/login" element={<Login />} />
      <Route path="/signup" element={<Signup />} />
      
      {/* User Routes */}
      <Route path="/" element={
        <ProtectedRoute>
          <UserLayout>
            <Home />
          </UserLayout>
        </ProtectedRoute>
      } />
      <Route path="/events/:id" element={
        <ProtectedRoute>
          <UserLayout>
            <EventDetails />
          </UserLayout>
        </ProtectedRoute>
      } />
      <Route path="/booking-confirmation/:bookingId" element={
        <ProtectedRoute>
          <UserLayout>
            <BookingConfirmation />
          </UserLayout>
        </ProtectedRoute>
      } />
      <Route path="/my-bookings" element={
        <ProtectedRoute>
          <UserLayout>
            <MyBookings />
          </UserLayout>
        </ProtectedRoute>
      } />

      {/* Admin Routes */}
      <Route path="/admin/login" element={<AdminLogin />} />
      <Route path="/admin" element={
        <ProtectedRoute requireAdmin={true}>
          <AdminLayout>
            <AdminDashboard />
          </AdminLayout>
        </ProtectedRoute>
      } />
      <Route path="/admin/events" element={
        <ProtectedRoute requireAdmin={true}>
          <AdminLayout>
            <AdminEvents />
          </AdminLayout>
        </ProtectedRoute>
      } />
      <Route path="/admin/users" element={
        <ProtectedRoute requireAdmin={true}>
          <AdminLayout>
            <AdminUsers />
          </AdminLayout>
        </ProtectedRoute>
      } />
      <Route path="/admin/bookings" element={
        <ProtectedRoute requireAdmin={true}>
          <AdminLayout>
            <AdminBookings />
          </AdminLayout>
        </ProtectedRoute>
      } />

      {/* Default redirect */}
      <Route path="*" element={<Navigate to="/" replace />} />
    </Routes>
  )
}

export default App

