import { create } from 'zustand';
import type { UserDto } from '@/lib/api/types';
import { session } from '@/lib/auth/session';

interface AuthState {
  user: UserDto | null;
  loading: boolean;
  setUser: (u: UserDto | null) => void;
  setLoading: (v: boolean) => void;
  logout: () => void;
}

export const useAuthStore = create<AuthState>((set) => ({
  user: null,
  loading: true,
  setUser: (user) => set({ user, loading: false }),
  setLoading: (loading) => set({ loading }),
  logout: () => {
    session.clear();
    set({ user: null });
  }
}));