export interface AuthSession {
  userId: string;
  email: string;
  name: string;
  avatarColor: string;
}

export interface LoginRequest {
  email: string;
  password: string;
}

export interface SignupRequest {
  email: string;
  name: string;
  password: string;
}
