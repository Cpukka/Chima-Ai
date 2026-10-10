import * as SecureStore from 'expo-secure-store';

const ACCESS_KEY = 'chima_access';
const REFRESH_KEY = 'chima_refresh';

let memoryAccess: string | null = null;
let memoryRefresh: string | null = null;

export const session = {
  async load(): Promise<void> {
    try {
      memoryAccess = await SecureStore.getItemAsync(ACCESS_KEY);
      memoryRefresh = await SecureStore.getItemAsync(REFRESH_KEY);
    } catch {
      memoryAccess = null;
      memoryRefresh = null;
    }
  },

  getAccess(): string | null {
    return memoryAccess;
  },

  getRefresh(): string | null {
    return memoryRefresh;
  },

  async set(access: string, refresh: string): Promise<void> {
    memoryAccess = access;
    memoryRefresh = refresh;
    try {
      await SecureStore.setItemAsync(ACCESS_KEY, access);
      await SecureStore.setItemAsync(REFRESH_KEY, refresh);
    } catch {
      // ignore
    }
  },

  async clear(): Promise<void> {
    memoryAccess = null;
    memoryRefresh = null;
    try {
      await SecureStore.deleteItemAsync(ACCESS_KEY);
      await SecureStore.deleteItemAsync(REFRESH_KEY);
    } catch {
      // ignore
    }
  }
};