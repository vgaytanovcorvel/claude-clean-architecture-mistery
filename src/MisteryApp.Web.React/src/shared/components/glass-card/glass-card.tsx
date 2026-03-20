import type { ReactNode, HTMLAttributes } from 'react'
import styles from './glass-card.module.css'

interface GlassCardProps extends HTMLAttributes<HTMLDivElement> {
  children: ReactNode
  variant?: 'default' | 'elevated'
}

export function GlassCard({ children, variant = 'default', className, ...rest }: GlassCardProps) {
  const cls = [styles.card, variant === 'elevated' ? styles.elevated : '', className ?? '']
    .filter(Boolean).join(' ')
  return <div className={cls} {...rest}>{children}</div>
}
