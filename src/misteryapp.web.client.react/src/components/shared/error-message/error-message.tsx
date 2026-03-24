import styles from './error-message.module.css'

interface ErrorMessageProps {
  message: string
}

export function ErrorMessage({ message }: ErrorMessageProps) {
  return (
    <div className={styles.container} role="alert">
      <span className={styles.icon}>!</span>
      <p className={styles.text}>{message}</p>
    </div>
  )
}
