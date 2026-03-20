import type { IAuthRepository } from '../domain/interfaces/i-auth-repository'
import type { User } from '../domain/models/user'
import { NotFoundException } from '../domain/errors'

interface StoredUser {
  id: string
  email: string
  name: string
  bio: string
  password: string
  createdAt: string
}

const USERS_KEY   = 'mistery_users'
const SESSION_KEY = 'mistery_session'

function loadUsers(): StoredUser[] {
  try {
    return JSON.parse(localStorage.getItem(USERS_KEY) ?? '[]') as StoredUser[]
  } catch {
    return []
  }
}

function saveUsers(users: StoredUser[]): void {
  localStorage.setItem(USERS_KEY, JSON.stringify(users))
}

function toUser(stored: StoredUser): User {
  return { id: stored.id, email: stored.email, name: stored.name, bio: stored.bio, createdAt: stored.createdAt }
}

export class MockAuthRepository implements IAuthRepository {
  authGetCurrentUser(): User | null {
    const userId = localStorage.getItem(SESSION_KEY)
    if (!userId) return null
    const found = loadUsers().find(u => u.id === userId)
    return found ? toUser(found) : null
  }

  async authSignUp(email: string, name: string, password: string): Promise<User> {
    const users = loadUsers()
    const id = crypto.randomUUID()
    const newUser: StoredUser = {
      id,
      email: email.toLowerCase(),
      name,
      bio: '',
      password,
      createdAt: new Date().toISOString(),
    }
    saveUsers([...users, newUser])
    localStorage.setItem(SESSION_KEY, id)
    return toUser(newUser)
  }

  async authSignIn(email: string, password: string): Promise<User | null> {
    const found = loadUsers().find(
      u => u.email === email.toLowerCase() && u.password === password
    )
    if (!found) return null
    localStorage.setItem(SESSION_KEY, found.id)
    return toUser(found)
  }

  authSignOut(): void {
    localStorage.removeItem(SESSION_KEY)
  }
}

export class MockUserRepository {
  async userSingleById(id: string): Promise<User> {
    const found = loadUsers().find(u => u.id === id)
    if (!found) throw new NotFoundException(`User not found (UserId: ${id})`)
    return toUser(found)
  }

  async userSingleOrDefaultByEmail(email: string): Promise<User | null> {
    const found = loadUsers().find(u => u.email === email.toLowerCase())
    return found ? toUser(found) : null
  }

  async userUpdate(id: string, changes: Partial<Pick<User, 'name' | 'bio'>>): Promise<User> {
    const users = loadUsers()
    const idx = users.findIndex(u => u.id === id)
    if (idx === -1) throw new NotFoundException(`User not found (UserId: ${id})`)
    const updated = { ...users[idx], ...changes }
    const newUsers = [...users.slice(0, idx), updated, ...users.slice(idx + 1)]
    saveUsers(newUsers)
    return toUser(updated)
  }
}
