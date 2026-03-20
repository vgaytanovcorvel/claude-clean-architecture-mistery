import { useState, type FormEvent } from 'react'
import { Link, useNavigate } from 'react-router-dom'
import { useAuth } from '../../../core/auth-context'
import { GlassCard } from '../../../shared/components/glass-card/glass-card'
import { Input } from '../../../shared/components/input/input'
import { Button } from '../../../shared/components/button/button'
import styles from './auth-page.module.css'

export function SignupPage() {
  const { signUp } = useAuth()
  const navigate   = useNavigate()

  const [name,     setName]     = useState('')
  const [email,    setEmail]    = useState('')
  const [password, setPassword] = useState('')
  const [error,    setError]    = useState('')
  const [loading,  setLoading]  = useState(false)

  async function handleSubmit(e: FormEvent) {
    e.preventDefault()
    setError('')
    setLoading(true)
    const result = await signUp(email, name, password)
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
          <h1 className={styles.title}>Create account</h1>
          <p className={styles.subtitle}>Start organizing your life</p>
        </div>
        <form onSubmit={handleSubmit} className={styles.form} noValidate>
          {error && <p className={styles.formError} role="alert">{error}</p>}
          <Input
            label="Name"
            type="text"
            value={name}
            onChange={e => setName(e.target.value)}
            placeholder="Your name"
            autoComplete="name"
            required
          />
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
            autoComplete="new-password"
            hint="At least 6 characters"
            required
          />
          <Button type="submit" loading={loading} className={styles.submitBtn}>
            Create account
          </Button>
        </form>
        <p className={styles.footer}>
          Already have an account?{' '}
          <Link to="/login">Sign in</Link>
        </p>
      </GlassCard>
    </div>
  )
}
