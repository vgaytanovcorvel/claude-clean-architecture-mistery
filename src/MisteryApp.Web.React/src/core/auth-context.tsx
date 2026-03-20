import { createContext, useContext, useState, useEffect, type ReactNode } from 'react'
import type { User } from '../domain/models/user'
import type { Result } from '../domain/result'
import { useServices } from './services-context'

interface AuthContextValue {
  currentUser: User | null
  isLoading: boolean
  signIn:  (email: string, password: string) => Promise<Result<User>>
  signUp:  (email: string, name: string, password: string) => Promise<Result<User>>
  signOut: () => void
  refreshUser: (user: User) => void
}

const AuthContext = createContext<AuthContextValue | null>(null)

export function AuthProvider({ children }: { children: ReactNode }) {
  const { authService } = useServices()
  const [currentUser, setCurrentUser] = useState<User | null>(null)
  const [isLoading, setIsLoading]     = useState(true)

  useEffect(() => {
    setCurrentUser(authService.getCurrentUser())
    setIsLoading(false)
  }, [authService])

  async function signIn(email: string, password: string): Promise<Result<User>> {
    const result = await authService.signIn(email, password)
    if (result.success) setCurrentUser(result.value)
    return result
  }

  async function signUp(email: string, name: string, password: string): Promise<Result<User>> {
    const result = await authService.signUp(email, name, password)
    if (result.success) setCurrentUser(result.value)
    return result
  }

  function signOut() {
    authService.signOut()
    setCurrentUser(null)
  }

  function refreshUser(user: User) {
    setCurrentUser(user)
  }

  return (
    <AuthContext.Provider value={{ currentUser, isLoading, signIn, signUp, signOut, refreshUser }}>
      {children}
    </AuthContext.Provider>
  )
}

export function useAuth(): AuthContextValue {
  const ctx = useContext(AuthContext)
  if (!ctx) throw new Error('useAuth must be used inside AuthProvider')
  return ctx
}
