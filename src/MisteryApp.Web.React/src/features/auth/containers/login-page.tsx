import { useState, type FormEvent } from 'react'
import { Link, useNavigate } from 'react-router-dom'
import { useAuth } from '../../../core/auth-context'
import { GlassCard } from '../../../shared/components/glass-card/glass-card'
import { Input } from '../../../shared/components/input/input'
import { Button } from '../../../shared/components/button/button'
import styles from './auth-page.module.css'

export function LoginPage() {
  const { signIn } = useAuth()
  const navigate   = useNavigate()

  const [email,    setEmail]    = useState('')
  const [password, setPassword] = useState('')
  const [error,    setError]    = useState('')
  const [loading,  setLoading]  = useState(false)

  async function handleSubmit(e: FormEvent) {
    e.preventDefault()
    setError('')
    setLoading(true)
    const result = await signIn(email, password)
    setLoading(false)
    if (result.success) {
      navigate('/todos', { replace: true })
    } else {
      setError(result.error)
    }
  }

  return (
    <div className={styles.page}>
      <div className={styles.glow} aria-hidden="true" />
      <GlassCard variant="elevated" className={styles.card}>
        <div className={styles.header}>
          <div className={styles.logo} aria-hidden="true" />
          <h1 className={styles.title}>Welcome back</h1>
          <p className={styles.subtitle}>Sign in to your account</p>
        </div>
        <form onSubmit={handleSubmit} className={styles.form} noValidate>
          {error && <p className={styles.formError} role="alert">{error}</p>}
          <Input
            label="Email"
            type="email"
            value={email}
            onChange={e => setEmail(e.target.value)}
            placeholder="you@example.com"
            autoComplete="email"
            required
          />
          <Input
            label="Password"
            type="password"
            value={password}
            onChange={e => setPassword(e.target.value)}
            placeholder="••••••••"
            autoComplete="current-password"
            required
          />
          <Button type="submit" loading={loading} className={styles.submitBtn}>
            Sign in
          </Button>
        </form>
        <p className={styles.footer}>
          No account?{' '}
          <Link to="/signup">Create one</Link>
        </p>
      </GlassCard>
    </div>
  )
}
