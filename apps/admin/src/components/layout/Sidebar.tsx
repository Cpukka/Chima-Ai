'use client';

import Link from 'next/link';
import { usePathname } from 'next/navigation';
import {
  LayoutDashboard,
  Users,
  CreditCard,
  Receipt,
  Cpu,
  BarChart3,
  ToggleLeft,
  Settings,
  ScrollText
} from 'lucide-react';
import { cn } from '@/lib/utils/cn';

const NAV = [
  { href: '/', label: 'Dashboard', icon: LayoutDashboard },
  { href: '/users', label: 'Users', icon: Users },
  { href: '/subscriptions', label: 'Subscriptions', icon: CreditCard },
  { href: '/payments', label: 'Payments', icon: Receipt },
  { href: '/ai-models', label: 'AI Models', icon: Cpu },
  { href: '/usage', label: 'Usage & Costs', icon: BarChart3 },
  { href: '/feature-flags', label: 'Feature Flags', icon: ToggleLeft },
  { href: '/settings', label: 'Settings', icon: Settings },
  { href: '/audit-logs', label: 'Audit Logs', icon: ScrollText }
];

export function Sidebar() {
  const pathname = usePathname();
  return (
    <aside className="hidden md:flex w-60 shrink-0 flex-col border-r border-border bg-background">
      <div className="h-16 flex items-center px-5 border-b border-border">
        <Link href="/dashboard" className="flex items-center gap-2 font-semibold">
          <div className="h-7 w-7 rounded-lg bg-gradient-to-br from-primary to-indigo-500" />
          <span>Chima Admin</span>
        </Link>
      </div>

      <nav className="flex-1 p-3 space-y-1 overflow-y-auto">
        {NAV.map((n) => {
          const Icon = n.icon;
          const active = n.href === '/' ? pathname === '/' : pathname === n.href || pathname.startsWith(n.href + '/');
          return (
            <Link
              key={n.href}
              href={n.href}
              className={cn(
                'flex items-center gap-3 rounded-lg px-3 py-2 text-sm transition-colors',
                active
                  ? 'bg-primary/10 text-primary font-medium'
                  : 'text-muted-foreground hover:bg-accent hover:text-foreground'
              )}
            >
              <Icon className="h-4 w-4 shrink-0" />
              <span>{n.label}</span>
            </Link>
          );
        })}
      </nav>

      <div className="p-3 border-t border-border text-xs text-muted-foreground">
        v0.1.0
      </div>
    </aside>
  );
}