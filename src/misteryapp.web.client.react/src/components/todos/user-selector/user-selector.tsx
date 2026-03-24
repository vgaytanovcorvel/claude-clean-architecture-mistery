import styles from './user-selector.module.css'
import type { User } from '../../../domain/models/user'

interface UserSelectorProps {
  users: readonly User[]
  selectedUserId: number | null
  onSelect: (id: number) => void
}

export function UserSelector({ users, selectedUserId, onSelect }: UserSelectorProps) {
  return (
    <div className={styles.bar}>
      {users.map(user => (
        <button
          key={user.id}
          className={`${styles.chip} ${user.id === selectedUserId ? styles.chipActive : ''}`}
          onClick={() => onSelect(user.id)}
          aria-pressed={user.id === selectedUserId}
        >
          {user.avatarUrl && <img className={styles.avatar} src={user.avatarUrl} alt="" />}
          <span className={styles.name}>{user.name}</span>
        </button>
      ))}
    </div>
  )
}
