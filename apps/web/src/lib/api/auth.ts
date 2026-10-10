import { apiFetch } from './client';
import type { AuthResponse, UserDto } from './types';

export const authApi = {
  register: (body: { email: string; password: string; displayName?: string }) =>
    apiFetch<AuthResponse>('/api/auth/register', {
      method: 'POST',
      body: JSON.stringify(body),
      skipAuth: true
    }),

  login: (body: { email: string; password: string; deviceInfo?: string }) =>
    apiFetch<AuthResponse>('/api/auth/login', {
      method: 'POST',
      body: JSON.stringify(body),
      skipAuth: true
    }),

  logout: (refreshToken: string) =>
    apiFetch<Record<string, never>>('/api/auth/logout', {
      method: 'POST',
      body: JSON.stringify({ refreshToken })
    }),

  me: () => apiFetch<UserDto>('/api/auth/me'),

  forgotPassword: (email: string) =>
    apiFetch<Record<string, never>>('/api/auth/forgot-password', {
      method: 'POST',
      body: JSON.stringify({ email }),
      skipAuth: true
    }),

  resetPassword: (token: string, newPassword: string) =>
    apiFetch<Record<string, never>>('/api/auth/reset-password', {
      method: 'POST',
      body: JSON.stringify({ token, newPassword }),
      skipAuth: true
    }),

  googleStartUrl: async (): Promise<string> => {
    const res = await apiFetch<{ url: string; state: string }>('/api/auth/google/start', {
      method: 'GET',
      skipAuth: true
    });
    return res.url;
  }
};