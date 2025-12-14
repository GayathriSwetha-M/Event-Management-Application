import api from '../services/api'

let refreshTimer = null

const refreshAccessToken = async () => {
  const refreshToken = localStorage.getItem('refreshToken')
  const accessToken = localStorage.getItem('accessToken') || localStorage.getItem('token')

  if (!refreshToken || !accessToken) {
    return
  }

  try {
    const axios = (await import('axios')).default
    const refreshApi = axios.create({
      baseURL: api.defaults.baseURL,
      headers: { 'Content-Type': 'application/json' }
    })

    const response = await refreshApi.post('/api/auth/refresh', {
      accessToken: accessToken,
      refreshToken: refreshToken
    })

    if (response.data.success) {
      const { accessToken: newAccessToken, refreshToken: newRefreshToken } = response.data.data

      localStorage.setItem('accessToken', newAccessToken)
      localStorage.setItem('token', newAccessToken)
      if (newRefreshToken) {
        localStorage.setItem('refreshToken', newRefreshToken)
      }

      api.defaults.headers.common['Authorization'] = `Bearer ${newAccessToken}`

      startTokenRefreshTimer()
    }
  } catch (error) {
    console.error('Token refresh failed:', error)
  }
}

export const startTokenRefreshTimer = () => {
  if (refreshTimer) {
    clearTimeout(refreshTimer)
  }

  const accessToken = localStorage.getItem('accessToken') || localStorage.getItem('token')
  if (!accessToken) {
    return
  }

  try {
    const payload = JSON.parse(atob(accessToken.split('.')[1]))
    const expirationTime = payload.exp * 1000 // Convert to milliseconds
    const currentTime = Date.now()
    const timeUntilExpiry = expirationTime - currentTime

    const refreshTime = timeUntilExpiry - (2 * 60 * 1000) // 2 minutes before expiry

    if (refreshTime > 0) {
      refreshTimer = setTimeout(() => {
        refreshAccessToken()
      }, refreshTime)
    } else {
      refreshAccessToken()
    }
  } catch (error) {
    console.error('Error parsing token:', error)
  }
}

export const stopTokenRefreshTimer = () => {
  if (refreshTimer) {
    clearTimeout(refreshTimer)
    refreshTimer = null
  }
}

if (typeof window !== 'undefined') {
  window.startTokenRefreshTimer = startTokenRefreshTimer
}

