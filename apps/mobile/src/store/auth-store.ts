import { create } from 'zustand';
import type { UserDto } from '@/api/types';
import { session } from '@/auth/session';

interface AuthState {
  user: UserDto | null;
  loading: boolean;
  setUser: (u: UserDto | null) => void;
  setLoading: (v: boolean) => void;
  logout: () => Promise<void>;
}

export const useAuthStore = create<AuthState>((set) => ({
  user: null,
  loading: true,
  setUser: (user) => set({ user, loading: false }),
  setLoading: (loading) => set({ loading }),
  logout: async () => {
    await session.clear();
    set({ user: null });
  }
}));