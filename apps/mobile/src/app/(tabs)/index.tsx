import { useState } from 'react';
import { Alert, Pressable, ScrollView, StyleSheet, View } from 'react-native';
import { router } from 'expo-router';
import { useSafeAreaInsets } from 'react-native-safe-area-context';
import { Sparkles, BookOpen, PenLine, Code2, Lightbulb } from 'lucide-react-native';
import { useTheme } from '@/theme/ThemeProvider';
import { Text } from '@/components/ui/Text';
import { conversationsApi } from '@/api/conversations';
import { useAuthStore } from '@/store/auth-store';

const PROMPTS = [
  { icon: BookOpen, label: 'Explain something', prompt: 'Explain how X works in simple terms.' },
  { icon: PenLine, label: 'Help me write', prompt: 'Help me draft an email about X.' },
  { icon: Code2, label: 'Help me code', prompt: 'Write a function that does X in TypeScript.' },
  { icon: Lightbulb, label: 'Brainstorm ideas', prompt: 'Give me 10 creative ideas for X.' }
];

export default function ChatsTab() {
  const { colors } = useTheme();
  const insets = useSafeAreaInsets();
  const user = useAuthStore((s) => s.user);
  const [creating, setCreating] = useState(false);

  async function start(prompt?: string) {
    if (creating) return;
    setCreating(true);
    try {
      const conv = await conversationsApi.create({});
      const url = prompt
        ? `/chat/${conv.id}?prompt=${encodeURIComponent(prompt)}`
        : `/chat/${conv.id}`;
      router.push(url as never);
    } catch (e) {
      Alert.alert('Could not start chat', e instanceof Error ? e.message : 'Try again');
    } finally {
      setCreating(false);
    }
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
      <Text variant="title">
        {user?.displayName ? `Hi, ${user.displayName.split(' ')[0]}` : 'Welcome'}
      </Text>
      <Text muted style={{ marginTop: 4 }}>
        How can I help you today?
      </Text>

      <View style={styles.iconWrap}>
        <View style={[styles.iconCircle, { backgroundColor: colors.primary + '20' }]}>
          <Sparkles color={colors.primary} size={28} />
        </View>
      </View>

      <Pressable
        onPress={() => start()}
        style={({ pressed }) => [
          styles.newChat,
          { backgroundColor: colors.primary, opacity: pressed ? 0.9 : 1 }
        ]}
      >
        <Text style={{ color: '#ffffff', fontWeight: '600', fontSize: 15 }}>
          {creating ? 'Starting…' : 'Start a new chat'}
        </Text>
      </Pressable>

      <Text variant="label" muted style={{ marginTop: 28, marginBottom: 10 }}>
        Or try one of these
      </Text>

      <View style={{ gap: 10 }}>
        {PROMPTS.map((p) => {
          const Icon = p.icon;
          return (
            <Pressable
              key={p.label}
              onPress={() => start(p.prompt)}
              style={({ pressed }) => [
                styles.promptRow,
                {
                  backgroundColor: colors.surface,
                  borderColor: colors.border,
                  opacity: pressed ? 0.85 : 1
                }
              ]}
            >
              <View style={[styles.promptIcon, { backgroundColor: colors.primary + '15' }]}>
                <Icon color={colors.primary} size={18} />
              </View>
              <Text>{p.label}</Text>
            </Pressable>
          );
        })}
      </View>
    </ScrollView>
  );
}

const styles = StyleSheet.create({
  iconWrap: { alignItems: 'center', marginTop: 40, marginBottom: 24 },
  iconCircle: {
    width: 64,
    height: 64,
    borderRadius: 32,
    alignItems: 'center',
    justifyContent: 'center'
  },
  newChat: {
    height: 52,
    borderRadius: 12,
    alignItems: 'center',
    justifyContent: 'center'
  },
  promptRow: {
    flexDirection: 'row',
    alignItems: 'center',
    padding: 14,
    borderRadius: 12,
    borderWidth: 1
  },
  promptIcon: {
    width: 36,
    height: 36,
    borderRadius: 10,
    alignItems: 'center',
    justifyContent: 'center',
    marginRight: 12
  }
});