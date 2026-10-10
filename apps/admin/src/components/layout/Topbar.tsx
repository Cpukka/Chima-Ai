'use client';

import { useRouter } from 'next/navigation';
import { LogOut } from 'lucide-react';
import { Button } from '@/components/ui/button';
import { ThemeToggle } from './ThemeToggle';
import { useAuthStore } from '@/store/auth-store';
import { authApi } from '@/lib/api/auth';
import { session } from '@/lib/auth/session';
import { toast } from 'sonner';

export function Topbar() {
  const router = useRouter();
  const { user, logout } = useAuthStore();

  async function signOut() {
    const rt = session.getRefresh();
    if (rt) {
      try { await authApi.logout(rt); } catch { /* ignore */ }
    }
    logout();
    toast.success('Signed out');
    router.push('/login');
  }

  return (
    <header className="h-16 border-b border-border bg-background flex items-center justify-between px-6 shrink-0">
      <div className="text-sm text-muted-foreground">
        {new Date().toLocaleDateString(undefined, { weekday: 'long', month: 'long', day: 'numeric' })}
      </div>

      <div className="flex items-center gap-2">
        <ThemeToggle />

        <div className="text-right px-2">
          <div className="text-sm font-medium">{user?.email}</div>
          <div className="text-xs text-muted-foreground">Administrator</div>
        </div>

        <Button variant="ghost" size="icon" onClick={signOut} aria-label="Sign out">
          <LogOut className="h-4 w-4" />
        </Button>
      </div>
    </header>
  );
}