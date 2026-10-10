import { apiFetch } from './client';
import type { AuthResponse, UserDto } from './types';

export const authApi = {
  login: (email: string, password: string) =>
    apiFetch<AuthResponse>('/api/auth/login', {
      method: 'POST',
      body: JSON.stringify({ email, password, deviceInfo: 'Admin Web' }),
      skipAuth: true
    }),

  me: () => apiFetch<UserDto>('/api/auth/me'),

  logout: (refreshToken: string) =>
    apiFetch<Record<string, never>>('/api/auth/logout', {
      method: 'POST',
      body: JSON.stringify({ refreshToken })
    })
};