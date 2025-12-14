import axios from 'axios'

const api = axios.create({
  baseURL: 'https://localhost:7205', // Backend API URL
  headers: {
    'Content-Type': 'application/json'
  }
})

const token = localStorage.getItem('accessToken') || localStorage.getItem('token')
if (token) {
  api.defaults.headers.common['Authorization'] = `Bearer ${token}`
}

api.interceptors.request.use(
  (config) => {
    const token = localStorage.getItem('accessToken') || localStorage.getItem('token')
    if (token) {
      config.headers.Authorization = `Bearer ${token}`
    }
    return config
  },
  (error) => {
    return Promise.reject(error)
  }
)

let isRefreshing = false
let failedQueue = []

const processQueue = (error, token = null) => {
  failedQueue.forEach(prom => {
    if (error) {
      prom.reject(error)
    } else {
      prom.resolve(token)
    }
  })
  failedQueue = []
}

api.interceptors.response.use(
  (response) => response,
  async (error) => {
    const originalRequest = error.config

    if (error.response?.status === 401 && !originalRequest._retry) {
      if (isRefreshing) {
        return new Promise((resolve, reject) => {
          failedQueue.push({ resolve, reject })
        })
          .then(token => {
            originalRequest.headers.Authorization = `Bearer ${token}`
            return api(originalRequest)
          })
          .catch(err => {
            return Promise.reject(err)
          })
      }

      originalRequest._retry = true
      isRefreshing = true

      const refreshToken = localStorage.getItem('refreshToken')
      const accessToken = localStorage.getItem('accessToken') || localStorage.getItem('token')

      if (!refreshToken || !accessToken) {
        localStorage.removeItem('token')
        localStorage.removeItem('accessToken')
        localStorage.removeItem('refreshToken')
        localStorage.removeItem('user')
        delete api.defaults.headers.common['Authorization']
        processQueue(error, null)
        isRefreshing = false
        window.location.href = '/login'
        return Promise.reject(error)
      }

      try {
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
          localStorage.setItem('token', newAccessToken) // Keep for backward compatibility
          if (newRefreshToken) {
            localStorage.setItem('refreshToken', newRefreshToken)
          }
          
          api.defaults.headers.common['Authorization'] = `Bearer ${newAccessToken}`
          
          if (typeof window !== 'undefined' && window.startTokenRefreshTimer) {
            window.startTokenRefreshTimer()
          }
          
          originalRequest.headers.Authorization = `Bearer ${newAccessToken}`
          
          processQueue(null, newAccessToken)
          isRefreshing = false
          
          return api(originalRequest)
        } else {
          throw new Error('Token refresh failed')
        }
      } catch (refreshError) {
        localStorage.removeItem('token')
        localStorage.removeItem('accessToken')
        localStorage.removeItem('refreshToken')
        localStorage.removeItem('user')
        delete api.defaults.headers.common['Authorization']
        processQueue(refreshError, null)
        isRefreshing = false
        window.location.href = '/login'
        return Promise.reject(refreshError)
      }
    }

    return Promise.reject(error)
  }
)

export default api

