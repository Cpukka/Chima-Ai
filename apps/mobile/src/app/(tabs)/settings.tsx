import { Alert, Pressable, ScrollView, StyleSheet, View } from 'react-native';
import { router } from 'expo-router';
import { useSafeAreaInsets } from 'react-native-safe-area-context';
import { LogOut, Moon, Sun, Monitor, CreditCard } from 'lucide-react-native';
import { useTheme } from '@/theme/ThemeProvider';
import { Text } from '@/components/ui/Text';
import { useAuthStore } from '@/store/auth-store';
import { authApi } from '@/api/auth';
import { session } from '@/auth/session';

export default function SettingsTab() {
  const { colors, mode, setMode } = useTheme();
  const insets = useSafeAreaInsets();
  const { user, logout } = useAuthStore();

  async function signOut() {
    Alert.alert('Sign out', 'Are you sure?', [
      { text: 'Cancel', style: 'cancel' },
      {
        text: 'Sign out',
        style: 'destructive',
        onPress: async () => {
          const rt = session.getRefresh();
          if (rt) {
            try {
              await authApi.logout(rt);
            } catch {
              /* ignore */
            }
          }
          await logout();
          router.replace('/(auth)/login');
        }
      }
    ]);
  }

  return (
    <ScrollView
      style={{ flex: 1, backgroundColor: colors.background }}
      contentContainerStyle={{
        paddingTop: insets.top + 24,
        paddingBottom: insets.bottom + 24,
        paddingHorizontal: 20
      }}
    >
      <Text variant="title">Settings</Text>

      <View style={styles.section}>
        <Text variant="caption" muted style={{ marginBottom: 8, textTransform: 'uppercase' }}>
          Account
        </Text>
        <View
          style={[
            styles.card,
            { backgroundColor: colors.surface, borderColor: colors.border }
          ]}
        >
          <View style={styles.row}>
            <Text muted>Name</Text>
            <Text>{user?.displayName ?? 'â€”'}</Text>
          </View>
          <View style={[styles.divider, { backgroundColor: colors.border }]} />
          <View style={styles.row}>
            <Text muted>Email</Text>
            <Text numberOfLines={1} style={{ maxWidth: 200 }}>
              {user?.email}
            </Text>
          </View>
          <View style={[styles.divider, { backgroundColor: colors.border }]} />
          <View style={styles.row}>
            <Text muted>Email verified</Text>
            <Text>{user?.emailVerified ? 'Yes' : 'No'}</Text>
          </View>
        </View>
      </View>

      
      <View style={styles.section}>
        <Text variant="caption" muted style={{ marginBottom: 8, textTransform: 'uppercase' }}>
          Billing
        </Text>
        <Pressable
          onPress={() => router.push('/subscription' as never)}
          style={[
            styles.card,
            { backgroundColor: colors.surface, borderColor: colors.border, padding: 14 }
          ]}
        >
          <View style={{ flexDirection: 'row', alignItems: 'center', justifyContent: 'space-between' }}>
            <View style={{ flexDirection: 'row', alignItems: 'center', gap: 10 }}>
              <CreditCard size={18} color={colors.primary} />
              <Text>Subscription & plans</Text>
            </View>
            <Text variant="caption" muted>Manage</Text>
          </View>
        </Pressable>
      </View>
<View style={styles.section}>
        <Text variant="caption" muted style={{ marginBottom: 8, textTransform: 'uppercase' }}>
          Appearance
        </Text>
        <View
          style={[
            styles.card,
            { backgroundColor: colors.surface, borderColor: colors.border }
          ]}
        >
          {(
            [
              { key: 'light', label: 'Light', icon: Sun },
              { key: 'dark', label: 'Dark', icon: Moon },
              { key: 'system', label: 'System', icon: Monitor }
            ] as const
          ).map((opt, i) => {
            const Icon = opt.icon;
            const selected = mode === opt.key;
            return (
              <View key={opt.key}>
                {i > 0 && <View style={[styles.divider, { backgroundColor: colors.border }]} />}
                <Pressable onPress={() => setMode(opt.key)} style={styles.row}>
                  <View style={{ flexDirection: 'row', alignItems: 'center', gap: 10 }}>
                    <Icon size={18} color={selected ? colors.primary : colors.textMuted} />
                    <Text style={{ color: selected ? colors.primary : colors.text }}>
                      {opt.label}
                    </Text>
                  </View>
                  {selected && <View style={[styles.dot, { backgroundColor: colors.primary }]} />}
                </Pressable>
              </View>
            );
          })}
        </View>
      </View>

      <Pressable
        onPress={signOut}
        style={({ pressed }) => [
          styles.signOut,
          {
            borderColor: colors.destructive,
            opacity: pressed ? 0.85 : 1,
            marginTop: 24
          }
        ]}
      >
        <LogOut size={16} color={colors.destructive} />
        <Text style={{ color: colors.destructive, fontWeight: '600', marginLeft: 8 }}>
          Sign out
        </Text>
      </Pressable>
    </ScrollView>
  );
}

const styles = StyleSheet.create({
  section: { marginTop: 24 },
  card: { borderRadius: 12, borderWidth: 1, overflow: 'hidden' },
  row: {
    flexDirection: 'row',
    alignItems: 'center',
    justifyContent: 'space-between',
    padding: 14
  },
  divider: { height: 1 },
  dot: { width: 8, height: 8, borderRadius: 4 },
  signOut: {
    height: 48,
    borderRadius: 12,
    borderWidth: 1,
    flexDirection: 'row',
    alignItems: 'center',
    justifyContent: 'center'
  }
});