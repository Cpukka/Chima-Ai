import { cn } from '@/lib/utils/cn';

interface Props {
  used: number;
  limit: number;
  label: string;
  className?: string;
}

export function UsageBar({ used, limit, label, className }: Props) {
  const unlimited = limit < 0;
  const pct = unlimited ? 0 : Math.min(100, (used / Math.max(1, limit)) * 100);
  const danger = !unlimited && pct >= 90;
  const warn = !unlimited && pct >= 70 && pct < 90;

  return (
    <div className={cn('space-y-1.5', className)}>
      <div className="flex items-baseline justify-between text-sm">
        <span className="text-muted-foreground">{label}</span>
        <span className="tabular-nums">
          {used}
          <span className="text-muted-foreground"> / {unlimited ? '∞' : limit}</span>
        </span>
      </div>
      <div className="h-2 rounded-full bg-muted overflow-hidden">
        <div
          className={cn(
            'h-full transition-all',
            danger ? 'bg-destructive' : warn ? 'bg-amber-500' : 'bg-primary'
          )}
          style={{ width: `${unlimited ? 5 : Math.max(2, pct)}%` }}
        />
      </div>
    </div>
  );
}