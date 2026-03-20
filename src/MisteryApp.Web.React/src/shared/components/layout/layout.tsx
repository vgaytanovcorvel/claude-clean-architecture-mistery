import type { ReactNode } from 'react'
import { NavLink, useNavigate } from 'react-router-dom'
import { useAuth } from '../../../core/auth-context'
import styles from './layout.module.css'

export function Layout({ children }: { children: ReactNode }) {
  const { currentUser, signOut } = useAuth()
  const navigate = useNavigate()

  function handleSignOut() {
    signOut()
    navigate('/login')
  }

  return (
    <div className={styles.root}>
      <header className={styles.header}>
        <nav className={styles.nav}>
          <span className={styles.brand}>
            <span className={styles.brandDot} aria-hidden="true" />
            Mistery
          </span>
          <div className={styles.navLinks}>
            <NavLink
              to="/todos"
              className={({ isActive }) =>
                [styles.navLink, isActive ? styles.navLinkActive : ''].filter(Boolean).join(' ')
              }
            >
              Todos
            </NavLink>
            <NavLink
              to="/profile"
              className={({ isActive }) =>
                [styles.navLink, isActive ? styles.navLinkActive : ''].filter(Boolean).join(' ')
              }
            >
              Profile
            </NavLink>
          </div>
          <div className={styles.navUser}>
            <span className={styles.avatar} aria-hidden="true">
              {currentUser?.name.charAt(0).toUpperCase()}
            </span>
            <button className={styles.signOutBtn} onClick={handleSignOut} type="button">
              Sign out
            </button>
          </div>
        </nav>
      </header>
      <main className={styles.main}>{children}</main>
    </div>
  )
}
