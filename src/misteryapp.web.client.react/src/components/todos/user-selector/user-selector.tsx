import { motion } from 'framer-motion'
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
      {users.map(user => {
        const isActive = user.id === selectedUserId
        return (
          <button
            key={user.id}
            className={`${styles.chip} ${isActive ? styles.chipActive : ''}`}
            onClick={() => onSelect(user.id)}
            aria-pressed={isActive}
          >
            {isActive && (
              <motion.span
                className={styles.pill}
                layoutId="user-pill"
                transition={{ type: 'spring', stiffness: 400, damping: 30 }}
              />
            )}
            {user.avatarUrl && <img className={styles.avatar} src={user.avatarUrl} alt="" />}
            <span className={styles.name}>{user.name}</span>
          </button>
        )
      })}
    </div>
  )
}
