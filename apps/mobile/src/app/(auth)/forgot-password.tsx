import { useState } from 'react';
import {
  Alert,
  KeyboardAvoidingView,
  Platform,
  ScrollView,
  StyleSheet,
  View
} from 'react-native';
import { router } from 'expo-router';
import { useSafeAreaInsets } from 'react-native-safe-area-context';
import { useTheme } from '@/theme/ThemeProvider';
import { Text } from '@/components/ui/Text';
import { Input } from '@/components/ui/Input';
import { Button } from '@/components/ui/Button';
import { authApi } from '@/api/auth';

export default function ForgotPasswordScreen() {
  const { colors } = useTheme();
  const insets = useSafeAreaInsets();
  const [email, setEmail] = useState('');
  const [sent, setSent] = useState(false);
  const [loading, setLoading] = useState(false);

  async function submit() {
    if (!email) {
      Alert.alert('Missing email', 'Please enter your email address.');
      return;
    }
    setLoading(true);
    try {
      await authApi.forgotPassword(email);
    } catch {
      // swallow to avoid enumeration
    } finally {
      setSent(true);
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
      >
        <View style={{ marginTop: 40 }}>
          <Text variant="title">Forgot password</Text>
          <Text muted style={{ marginTop: 6 }}>
            {sent
              ? `If an account exists for ${email}, we sent a reset link.`
              : 'Enter your email and we will send you a reset link.'}
          </Text>
        </View>

        {!sent && (
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
            <Button onPress={submit} loading={loading} size="lg" fullWidth style={{ marginTop: 12 }}>
              Send reset link
            </Button>
          </View>
        )}

        <View style={{ marginTop: 32 }}>
          <Button variant="outline" onPress={() => router.replace('/(auth)/login')}>
            Back to sign in
          </Button>
        </View>
      </ScrollView>
    </KeyboardAvoidingView>
  );
}

const styles = StyleSheet.create({
  scroll: { paddingHorizontal: 24, flexGrow: 1 }
});