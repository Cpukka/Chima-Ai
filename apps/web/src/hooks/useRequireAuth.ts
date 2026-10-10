'use client';

import { useEffect } from 'react';
import { useRouter } from 'next/navigation';
import { authApi } from '@/lib/api/auth';
import { session } from '@/lib/auth/session';
import { useAuthStore } from '@/store/auth-store';

/**
 * Ensures a valid session, loads current user into the store,
 * and redirects to /login on failure.
 */
export function useRequireAuth() {
  const router = useRouter();
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
        if (!cancelled) setUser(me);
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
  }, [router, setUser, setLoading, logout, user]);

  return { user, loading };
}