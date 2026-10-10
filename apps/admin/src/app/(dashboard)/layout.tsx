'use client';

import { useEffect } from 'react';
import { useRouter, usePathname } from 'next/navigation';
import { Sidebar } from '@/components/layout/Sidebar';
import { Topbar } from '@/components/layout/Topbar';
import { authApi } from '@/lib/api/auth';
import { session } from '@/lib/auth/session';
import { useAuthStore } from '@/store/auth-store';

export default function DashboardLayout({ children }: { children: React.ReactNode }) {
  const router = useRouter();
  const pathname = usePathname();
  const { user, loading, setUser, setLoading, logout } = useAuthStore();

  useEffect(() => {
    let cancelled = false;

    async function boot() {
      if (!session.getAccess()) {
        router.replace('/login');
        return;
      }
      if (user) {
        setLoading(false);
        return;
      }
      try {
        const me = await authApi.me();
        if (cancelled) return;
        if (!me.isAdmin) {
          toast();
          logout();
          router.replace('/login');
          return;
        }
        setUser(me);
      } catch {
        if (!cancelled) {
          logout();
          router.replace('/login');
        }
      }
    }

    boot();
    return () => {
      cancelled = true;
    };
  }, [router, user, setUser, setLoading, logout, pathname]);

  if (loading) {
    return (
      <div className="min-h-screen flex items-center justify-center">
        <div className="animate-pulse text-sm text-muted-foreground">Loading consoleâ€¦</div>
      </div>
    );
  }

  return (
    <div className="flex min-h-screen bg-muted/20">
      <Sidebar />
      <div className="flex-1 flex flex-col min-w-0">
        <Topbar />
        <main className="flex-1 p-6">{children}</main>
      </div>
    </div>
  );
}

function toast() {
  // Lazy import to keep this file server-safe
  import('sonner').then(({ toast }) => toast.error('Admin access required'));
}