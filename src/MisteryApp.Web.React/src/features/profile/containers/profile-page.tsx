import { useState, type FormEvent } from 'react'
import { useAuth } from '../../../core/auth-context'
import { useServices } from '../../../core/services-context'
import { Layout } from '../../../shared/components/layout/layout'
import { GlassCard } from '../../../shared/components/glass-card/glass-card'
import { Input } from '../../../shared/components/input/input'
import { Button } from '../../../shared/components/button/button'
import styles from './profile-page.module.css'

export function ProfilePage() {
  const { currentUser, refreshUser } = useAuth()
  const { profileService }           = useServices()

  const [name,     setName]     = useState(currentUser?.name     ?? '')
  const [bio,      setBio]      = useState(currentUser?.bio      ?? '')
  const [saving,   setSaving]   = useState(false)
  const [success,  setSuccess]  = useState(false)
  const [error,    setError]    = useState('')

  if (!currentUser) return null

  async function handleSubmit(e: FormEvent) {
    e.preventDefault()
    setError('')
    setSuccess(false)
    setSaving(true)
    const result = await profileService.updateProfile(currentUser!.id, { name, bio })
    setSaving(false)
    if (result.success) {
      refreshUser(result.value)
      setSuccess(true)
      setTimeout(() => setSuccess(false), 3000)
    } else {
      setError(result.error)
    }
  }

  return (
    <Layout>
      <div className={styles.page}>
        <header className={styles.pageHeader}>
          <h1 className={styles.pageTitle}>Profile</h1>
        </header>

        <div className={styles.grid}>
          <GlassCard className={styles.avatarCard}>
            <div className={styles.avatarRing}>
              <span className={styles.avatarLetter} aria-hidden="true">
                {currentUser.name.charAt(0).toUpperCase()}
              </span>
            </div>
            <p className={styles.avatarName}>{currentUser.name}</p>
            <p className={styles.avatarEmail}>{currentUser.email}</p>
            <div className={styles.metaRow}>
              <span className={styles.metaLabel}>Member since</span>
              <span className={styles.metaValue}>
                {new Date(currentUser.createdAt).toLocaleDateString('en-US', {
                  month: 'long',
                  year:  'numeric',
                })}
              </span>
            </div>
          </GlassCard>

          <GlassCard className={styles.formCard}>
            <h2 className={styles.sectionTitle}>Edit profile</h2>
            <form onSubmit={handleSubmit} className={styles.form} noValidate>
              {error   && <p className={styles.errorMsg}  role="alert">{error}</p>}
              {success && <p className={styles.successMsg} role="status">Profile updated!</p>}

              <Input
                label="Email"
                type="email"
                value={currentUser.email}
                readOnly
                hint="Email cannot be changed"
              />
              <Input
                label="Name"
                type="text"
                value={name}
                onChange={e => setName(e.target.value)}
                placeholder="Your name"
                maxLength={100}
                required
              />
              <div className={styles.textareaGroup}>
                <label className={styles.textareaLabel} htmlFor="bio">Bio</label>
                <textarea
                  id="bio"
                  className={styles.textarea}
                  value={bio}
                  onChange={e => setBio(e.target.value)}
                  placeholder="Tell us a little about yourself..."
                  rows={4}
                  maxLength={500}
                />
                <span className={styles.charCount}>{bio.length}/500</span>
              </div>
              <Button type="submit" loading={saving}>
                Save changes
              </Button>
            </form>
          </GlassCard>
        </div>
      </div>
    </Layout>
  )
}
