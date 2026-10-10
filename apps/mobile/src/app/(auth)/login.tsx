import { useState } from 'react';
import {
  Alert,
  KeyboardAvoidingView,
  Platform,
  ScrollView,
  StyleSheet,
  TouchableOpacity,
  View
} from 'react-native';
import { Link, router } from 'expo-router';
import { useSafeAreaInsets } from 'react-native-safe-area-context';
import { useTheme } from '@/theme/ThemeProvider';
import { Text } from '@/components/ui/Text';
import { Input } from '@/components/ui/Input';
import { Button } from '@/components/ui/Button';
import { authApi } from '@/api/auth';
import { session } from '@/auth/session';
import { useAuthStore } from '@/store/auth-store';

export default function LoginScreen() {
  const { colors } = useTheme();
  const insets = useSafeAreaInsets();
  const setUser = useAuthStore((s) => s.setUser);

  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [loading, setLoading] = useState(false);

  async function submit() {
    if (!email || !password) {
      Alert.alert('Missing fields', 'Please enter your email and password.');
      return;
    }
    setLoading(true);
    try {
      const res = await authApi.login({ email, password, deviceInfo: 'Android' });
      await session.set(res.accessToken, res.refreshToken);
      setUser(res.user);
      router.replace('/(tabs)');
    } catch (e) {
      Alert.alert('Sign in failed', e instanceof Error ? e.message : 'Please try again.');
    } finally {
      setLoading(false);
    }
  }

  return (
    <KeyboardAvoidingView
      behavior={Platform.OS === 'ios' ? 'padding' : undefined}
      style={{ flex: 1, backgroundColor: colors.background }}
    >
      <ScrollView
        contentContainerStyle={[
          styles.scroll,
          { paddingTop: insets.top + 40, paddingBottom: insets.bottom + 40 }
        ]}
        keyboardShouldPersistTaps="handled"
      >
        <View style={styles.header}>
          <Text variant="title">Welcome back</Text>
          <Text muted style={{ marginTop: 6 }}>
            Sign in to continue to Chima
          </Text>
        </View>

        <View style={{ marginTop: 32, gap: 12 }}>
          <View>
            <Text variant="label" style={{ marginBottom: 6 }}>
              Email
            </Text>
            <Input
              value={email}
              onChangeText={setEmail}
              placeholder="you@example.com"
              keyboardType="email-address"
              autoCapitalize="none"
              autoCorrect={false}
            />
          </View>

          <View>
            <Text variant="label" style={{ marginBottom: 6 }}>
              Password
            </Text>
            <Input
              value={password}
              onChangeText={setPassword}
              placeholder="Your password"
              secureTextEntry
            />
          </View>

          <TouchableOpacity
            onPress={() => router.push('/(auth)/forgot-password')}
            style={{ alignSelf: 'flex-end', marginTop: 4 }}
          >
            <Text variant="caption" muted>
              Forgot password?
            </Text>
          </TouchableOpacity>

          <Button onPress={submit} loading={loading} size="lg" fullWidth style={{ marginTop: 12 }}>
            Sign in
          </Button>
        </View>

        <View style={styles.footer}>
          <Text muted>New to Chima? </Text>
          <Link href="/(auth)/register">
            <Text style={{ color: colors.primary, fontWeight: '600' }}>Create account</Text>
          </Link>
        </View>
      </ScrollView>
    </KeyboardAvoidingView>
  );
}

const styles = StyleSheet.create({
  scroll: { paddingHorizontal: 24, flexGrow: 1 },
  header: { marginTop: 40 },
  footer: { flexDirection: 'row', justifyContent: 'center', marginTop: 32 }
});