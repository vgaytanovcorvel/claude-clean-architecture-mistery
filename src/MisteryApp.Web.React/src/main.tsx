import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'
import { BrowserRouter } from 'react-router-dom'
import { ServicesProvider } from './core/services-context'
import { AuthProvider } from './core/auth-context'
import App from './App'
import './styles/main.css'

const rootEl = document.getElementById('root')
if (!rootEl) throw new Error('Root element not found')

createRoot(rootEl).render(
  <StrictMode>
    <BrowserRouter>
      <ServicesProvider>
        <AuthProvider>
          <App />
        </AuthProvider>
      </ServicesProvider>
    </BrowserRouter>
  </StrictMode>,
)
