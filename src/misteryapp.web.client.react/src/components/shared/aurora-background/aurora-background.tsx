import { useEffect, useRef } from 'react'
import styles from './aurora-background.module.css'

interface AuroraBackgroundProps {
  intensity?: number // 0 to 1, controls vibrancy
}

export function AuroraBackground({ intensity = 0.5 }: AuroraBackgroundProps) {
  const ref = useRef<HTMLDivElement>(null)

  useEffect(() => {
    function handleMove(e: MouseEvent) {
      if (!ref.current) return
      const x = (e.clientX / window.innerWidth) * 100
      const y = (e.clientY / window.innerHeight) * 100
      ref.current.style.setProperty('--mouse-x', `${x}%`)
      ref.current.style.setProperty('--mouse-y', `${y}%`)
    }

    window.addEventListener('mousemove', handleMove)
    return () => window.removeEventListener('mousemove', handleMove)
  }, [])

  // Map intensity to opacity (0.3 to 0.8) and animation speed multiplier
  const opacity = 0.3 + intensity * 0.5
  const speed = 1 - intensity * 0.4 // faster when more intense

  return (
    <div
      ref={ref}
      className={styles.aurora}
      aria-hidden="true"
      style={{
        '--aurora-opacity': opacity,
        '--aurora-speed': speed,
      } as React.CSSProperties}
    >
      <div className={styles.blob1} />
      <div className={styles.blob2} />
      <div className={styles.blob3} />
    </div>
  )
}
