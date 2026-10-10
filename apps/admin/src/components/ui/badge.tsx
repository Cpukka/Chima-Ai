import { cn } from '@/lib/utils/cn';

const VARIANTS = {
  default: 'bg-primary/10 text-primary',
  success: 'bg-emerald-500/10 text-emerald-600',
  warning: 'bg-amber-500/10 text-amber-600',
  danger: 'bg-destructive/10 text-destructive',
  muted: 'bg-muted text-muted-foreground'
};

export type BadgeVariant = keyof typeof VARIANTS;

export function Badge({
  children,
  variant = 'default',
  className
}: {
  children: React.ReactNode;
  variant?: BadgeVariant;
  className?: string;
}) {
  return (
    <span
      className={cn(
        'inline-flex items-center px-2 py-0.5 rounded-full text-xs font-medium',
        VARIANTS[variant],
        className
      )}
    >
      {children}
    </span>
  );
}