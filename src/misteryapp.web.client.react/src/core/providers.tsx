import { createContext, useContext, type ReactNode } from 'react'
import { QueryClient, QueryClientProvider } from '@tanstack/react-query'
import { HttpExampleRepository } from '../repositories/http-example-repository'
import { ExampleService } from '../services/example-service'
import { ApiClient } from './api-client'
import type { IExampleService } from '../domain/interfaces/i-example-service'

export interface Services {
  exampleService: IExampleService
}

const apiClient = new ApiClient(import.meta.env.VITE_API_BASE_URL ?? '')
const exampleRepository = new HttpExampleRepository(apiClient)

const defaultServices: Services = {
  exampleService: new ExampleService(exampleRepository),
}

const ServicesContext = createContext<Services>(defaultServices)

const queryClient = new QueryClient({
  defaultOptions: {
    queries: {
      staleTime: 5 * 60 * 1000,
      retry: 1,
    },
  },
})

export function AppProviders({
  children,
  services = defaultServices,
}: {
  children: ReactNode
  services?: Services
}) {
  return (
    <ServicesContext.Provider value={services}>
      <QueryClientProvider client={queryClient}>{children}</QueryClientProvider>
    </ServicesContext.Provider>
  )
}

export const useServices = () => useContext(ServicesContext)
