import styles from './spinner.module.css'

export function Spinner() {
  return (
    <div className={styles.wrapper} role="status">
      <div className={styles.ring} />
      <span className="u-sr-only">Loading...</span>
    </div>
  )
}
