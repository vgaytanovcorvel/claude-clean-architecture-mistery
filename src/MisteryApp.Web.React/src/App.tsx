import { Routes, Route, Navigate } from 'react-router-dom'
import type { ReactNode } from 'react'
import { useAuth } from './core/auth-context'
import { LoginPage, SignupPage } from './features/auth'
import { TodosPage } from './features/todos'
import { ProfilePage } from './features/profile'

function ProtectedRoute({ children }: { children: ReactNode }) {
  const { currentUser, isLoading } = useAuth()
  if (isLoading)     return null
  if (!currentUser)  return <Navigate to="/login" replace />
  return <>{children}</>
}

function PublicRoute({ children }: { children: ReactNode }) {
  const { currentUser, isLoading } = useAuth()
  if (isLoading)    return null
  if (currentUser)  return <Navigate to="/todos" replace />
  return <>{children}</>
}

export default function App() {
  return (
    <Routes>
      <Route path="/" element={<Navigate to="/todos" replace />} />
      <Route path="/login"   element={<PublicRoute><LoginPage  /></PublicRoute>} />
      <Route path="/signup"  element={<PublicRoute><SignupPage /></PublicRoute>} />
      <Route path="/todos"   element={<ProtectedRoute><TodosPage   /></ProtectedRoute>} />
      <Route path="/profile" element={<ProtectedRoute><ProfilePage /></ProtectedRoute>} />
    </Routes>
  )
}
