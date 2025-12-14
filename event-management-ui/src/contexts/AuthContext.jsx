import { createContext, useContext, useState, useEffect } from 'react'
import api from '../services/api'
import { startTokenRefreshTimer, stopTokenRefreshTimer } from '../utils/tokenManager'

const AuthContext = createContext()

export const useAuth = () => {
  const context = useContext(AuthContext)
  if (!context) {
    throw new Error('useAuth must be used within an AuthProvider')
  }
  return context
}

export const AuthProvider = ({ children }) => {
  const [user, setUser] = useState(null)
  const [loading, setLoading] = useState(true)

  useEffect(() => {
    const accessToken = localStorage.getItem('accessToken') || localStorage.getItem('token')
    const refreshToken = localStorage.getItem('refreshToken')
    const userData = localStorage.getItem('user')
    
    if (accessToken && userData) {
      try {
        const parsedUser = JSON.parse(userData)
        setUser(parsedUser)
        api.defaults.headers.common['Authorization'] = `Bearer ${accessToken}`
        
        if (refreshToken) {
          localStorage.setItem('refreshToken', refreshToken)
        }
        
        startTokenRefreshTimer()
      } catch (error) {
        console.error('Error parsing user data:', error)
        clearAuthData()
      }
    }
    setLoading(false)
  }, [])

  const clearAuthData = () => {
    localStorage.removeItem('token')
    localStorage.removeItem('accessToken')
    localStorage.removeItem('refreshToken')
    localStorage.removeItem('user')
    delete api.defaults.headers.common['Authorization']
  }

  const login = async (emailOrPhone, password) => {
    try {
      const response = await api.post('/api/auth/login', {
        emailOrPhone,
        password
      })

      if (response.data.success) {
        const { accessToken, refreshToken, token, ...userData } = response.data.data
        const finalAccessToken = accessToken || token
        const finalRefreshToken = refreshToken
        
        localStorage.setItem('accessToken', finalAccessToken)
        localStorage.setItem('token', finalAccessToken) // Keep for backward compatibility
        if (finalRefreshToken) {
          localStorage.setItem('refreshToken', finalRefreshToken)
        }
        localStorage.setItem('user', JSON.stringify(userData))
        api.defaults.headers.common['Authorization'] = `Bearer ${finalAccessToken}`
        setUser(userData)
        
        startTokenRefreshTimer()
        
        return { success: true }
      } else {
        const errors = response.data.errors || []
        const errorMessage = errors.length > 0 
          ? errors.join(', ') 
          : response.data.message || 'Login failed'
        return { success: false, message: errorMessage }
      }
    } catch (error) {
      if (error.response?.data?.errors && error.response.data.errors.length > 0) {
        return {
          success: false,
          message: error.response.data.errors.join(', ')
        }
      }
      
      const errorMessage = error.response?.data?.message || 'Login failed. Please try again.'
      return {
        success: false,
        message: errorMessage
      }
    }
  }

  const signup = async (name, emailOrPhone, password) => {
    try {
      const response = await api.post('/api/auth/signup', {
        name,
        emailOrPhone,
        password
      })

      if (response.data.success) {
        return { success: true, message: response.data.message }
      } else {
        return { success: false, message: response.data.message }
      }
    } catch (error) {
      return {
        success: false,
        message: error.response?.data?.message || 'Signup failed. Please try again.'
      }
    }
  }

  const logout = () => {
    stopTokenRefreshTimer()
    clearAuthData()
    setUser(null)
  }

  const value = {
    user,
    loading,
    login,
    signup,
    logout
  }

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>
}

