import { ApplicationConfig, InjectionToken } from '@angular/core'
import { provideRouter } from '@angular/router'
import { provideHttpClient, withInterceptors } from '@angular/common/http'
import { routes } from './app.routes'

export const API_BASE_URL = new InjectionToken<string>('api.base.url')

export const appConfig: ApplicationConfig = {
  providers: [
    provideRouter(routes),
    provideHttpClient(withInterceptors([])),
    { provide: API_BASE_URL, useValue: '/api' },
  ],
}
