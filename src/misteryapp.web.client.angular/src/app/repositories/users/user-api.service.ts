import { Injectable, inject } from '@angular/core'
import { HttpClient } from '@angular/common/http'
import { firstValueFrom } from 'rxjs'
import { map } from 'rxjs/operators'
import { API_BASE_URL } from '../../app.config'
import { User } from '../../domain/models/user'
import { ApiResponse } from '../../domain/models/api-response'
import { LoginRequest } from '../../domain/models/requests'

@Injectable({ providedIn: 'root' })
export class UserApiService {
  private readonly http = inject(HttpClient)
  private readonly baseUrl = inject(API_BASE_URL)

  login(request: LoginRequest): Promise<User> {
    return firstValueFrom(
      this.http.post<ApiResponse<User>>(`${this.baseUrl}/users/login`, request).pipe(
        map(res => res.data!)
      )
    )
  }

  getUserByUsername(username: string): Promise<User | null> {
    return firstValueFrom(
      this.http.get<ApiResponse<User>>(`${this.baseUrl}/users/${username}`).pipe(
        map(res => res.data)
      )
    )
  }
}
