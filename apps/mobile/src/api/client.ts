import { Platform } from 'react-native';
import Constants from 'expo-constants';
import { session } from '@/auth/session';
import type { ApiResponse } from './types';

function resolveBase(): string {
  const env = process.env.EXPO_PUBLIC_API_BASE_URL;
  if (env && env.length > 0) return env;

  const hostUri = Constants.expoConfig?.hostUri;

  if (typeof hostUri === 'string' && hostUri.length > 0) {
    const host = hostUri.split(':')[0];
    return `http://${host}:5080`;
  }

  return Platform.OS === 'android' ? 'http://10.0.2.2:5080' : 'http://localhost:5080';
}

export const API_BASE = resolveBase();

export class ApiError extends Error {
  status: number;
  code: string;
  constructor(status: number, code: string, message: string) {
    super(message);
    this.status = status;
    this.code = code;
  }
}

async function tryRefresh(): Promise<boolean> {
  const refresh = session.getRefresh();
  if (!refresh) return false;
  try {
    const res = await fetch(`${API_BASE}/api/auth/refresh`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ refreshToken: refresh })
    });
    if (!res.ok) return false;
    const json = (await res.json()) as ApiResponse<{ accessToken: string; refreshToken: string }>;
    if (!json.success || !json.data) return false;
    await session.set(json.data.accessToken, json.data.refreshToken);
    return true;
  } catch {
    return false;
  }
}

export async function apiFetch<T>(
  path: string,
  options: RequestInit & { skipAuth?: boolean; _retry?: boolean } = {}
): Promise<T> {
  const { skipAuth, _retry, ...rest } = options;
  const headers: Record<string, string> = {
    'Content-Type': 'application/json',
    ...((rest.headers as Record<string, string>) ?? {})
  };

  if (!skipAuth) {
    const token = session.getAccess();
    if (token) headers.Authorization = `Bearer ${token}`;
  }

  const res = await fetch(`${API_BASE}${path}`, { ...rest, headers });

  if (res.status === 401 && !skipAuth && !_retry) {
    const ok = await tryRefresh();
    if (ok) return apiFetch<T>(path, { ...options, _retry: true });
    await session.clear();
  }

  let json: ApiResponse<T> | null = null;
  try {
    json = (await res.json()) as ApiResponse<T>;
  } catch {
    /* not JSON */
  }

  if (!res.ok || !json || !json.success) {
    const msg = json?.message ?? `Request failed (${res.status})`;
    const code = json?.errors?.[0]?.code ?? 'request_failed';
    throw new ApiError(res.status, code, msg);
  }

  return json.data as T;
}