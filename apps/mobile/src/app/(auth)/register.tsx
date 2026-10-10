import { useState } from 'react';
import {
  Alert,
  KeyboardAvoidingView,
  Platform,
  ScrollView,
  StyleSheet,
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

export default function RegisterScreen() {
  const { colors } = useTheme();
  const insets = useSafeAreaInsets();
  const setUser = useAuthStore((s) => s.setUser);

  const [displayName, setDisplayName] = useState('');
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
      const res = await authApi.register({ email, password, displayName });
      await session.set(res.accessToken, res.refreshToken);
      setUser(res.user);
      router.replace('/(tabs)');
    } catch (e) {
      Alert.alert('Registration failed', e instanceof Error ? e.message : 'Please try again.');
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
          <Text variant="title">Create your account</Text>
          <Text muted style={{ marginTop: 6 }}>
            Free to start. No credit card required.
          </Text>
        </View>

        <View style={{ marginTop: 32, gap: 12 }}>
          <View>
            <Text variant="label" style={{ marginBottom: 6 }}>
              Name
            </Text>
            <Input value={displayName} onChangeText={setDisplayName} placeholder="Your name" />
          </View>

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
              placeholder="At least 8 characters"
              secureTextEntry
            />
            <Text variant="caption" muted style={{ marginTop: 6 }}>
              Use uppercase, lowercase, a number, and a symbol.
            </Text>
          </View>

          <Button onPress={submit} loading={loading} size="lg" fullWidth style={{ marginTop: 12 }}>
            Create account
          </Button>
        </View>

        <View style={styles.footer}>
          <Text muted>Already have an account? </Text>
          <Link href="/(auth)/login">
            <Text style={{ color: colors.primary, fontWeight: '600' }}>Sign in</Text>
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