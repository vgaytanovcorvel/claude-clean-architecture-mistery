import { createContext, useContext, type ReactNode } from 'react'
import { MockAuthRepository, MockUserRepository } from '../repositories/mock-auth-repository'
import { MockTodoRepository } from '../repositories/mock-todo-repository'
import { AuthService } from '../services/auth-service'
import { TodoService } from '../services/todo-service'
import { ProfileService } from '../services/profile-service'

interface Services {
  authService: AuthService
  todoService: TodoService
  profileService: ProfileService
}

const authRepository  = new MockAuthRepository()
const userRepository  = new MockUserRepository()
const todoRepository  = new MockTodoRepository()

const defaultServices: Services = {
  authService:    new AuthService(authRepository, userRepository),
  todoService:    new TodoService(todoRepository),
  profileService: new ProfileService(userRepository),
}

const ServicesContext = createContext<Services>(defaultServices)

export function ServicesProvider({ children, services = defaultServices }: {
  children: ReactNode
  services?: Services
}) {
  return <ServicesContext.Provider value={services}>{children}</ServicesContext.Provider>
}

export function useServices(): Services {
  return useContext(ServicesContext)
}
