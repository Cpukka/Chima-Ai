import { useEffect } from 'react';
import { View, ActivityIndicator } from 'react-native';
import { router } from 'expo-router';
import { useAuthStore } from '@/store/auth-store';
import { authApi } from '@/api/auth';
import { session } from '@/auth/session';
import { useTheme } from '@/theme/ThemeProvider';

export default function Boot() {
  const { colors } = useTheme();
  const setUser = useAuthStore((s) => s.setUser);

  useEffect(() => {
    (async () => {
      if (!session.getAccess()) {
        router.replace('/(auth)/login');
        return;
      }
      try {
        const me = await authApi.me();
        setUser(me);
        router.replace('/(tabs)');
      } catch {
        await session.clear();
        router.replace('/(auth)/login');
      }
    })();
  }, [setUser]);

  return (
    <View
      style={{
        flex: 1,
        alignItems: 'center',
        justifyContent: 'center',
        backgroundColor: colors.background
      }}
    >
      <ActivityIndicator color={colors.primary} />
    </View>
  );
}