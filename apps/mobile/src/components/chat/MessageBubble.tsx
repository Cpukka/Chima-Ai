import { useState } from 'react';
import { Alert, Pressable, StyleSheet, View } from 'react-native';
import * as Clipboard from 'expo-clipboard';
import * as Haptics from 'expo-haptics';
import * as Speech from 'expo-speech';
import {
  Check,
  Copy,
  RefreshCw,
  AlertTriangle,
  Volume2,
  VolumeX
} from 'lucide-react-native';
import { useTheme } from '@/theme/ThemeProvider';
import { Text } from '@/components/ui/Text';
import { MarkdownRenderer } from './MarkdownRenderer';
import type { MessageDto, UserDto } from '@/api/types';

interface Props {
  message: MessageDto;
  user: UserDto | null;
  onRegenerate?: (messageId: string) => void;
}

export function MessageBubble({ message, user, onRegenerate }: Props) {
  const { colors } = useTheme();
  const [copied, setCopied] = useState(false);
  const [speaking, setSpeaking] = useState(false);
  const isUser = message.role === 'user';
  const isStreaming = message.status === 'streaming' || message.status === 'pending';
  const isFailed = message.status === 'failed';

  async function copyMessage() {
    await Clipboard.setStringAsync(message.content);
    await Haptics.impactAsync(Haptics.ImpactFeedbackStyle.Light);
    setCopied(true);
    setTimeout(() => setCopied(false), 1500);
  }

  async function toggleSpeak() {
    await Haptics.impactAsync(Haptics.ImpactFeedbackStyle.Light);

    if (speaking) {
      Speech.stop();
      setSpeaking(false);
      return;
    }

    // Stop any existing speech, then read this message
    Speech.stop();
    setSpeaking(true);

    // Strip markdown-ish markers for a cleaner read
    const clean = message.content
      .replace(/```[\s\S]*?```/g, ' code block ')
      .replace(/`([^`]+)`/g, '$1')
      .replace(/\*\*([^*]+)\*\*/g, '$1')
      .replace(/\*([^*]+)\*/g, '$1')
      .replace(/\[([^\]]+)\]\([^)]+\)/g, '$1')
      .replace(/^#+\s+/gm, '')
      .replace(/\s+/g, ' ')
      .trim();

    Speech.speak(clean, {
      onDone: () => setSpeaking(false),
      onStopped: () => setSpeaking(false),
      onError: () => {
        setSpeaking(false);
        Alert.alert('Speech failed', 'Could not read the message aloud.');
      }
    });
  }

  return (
    <View
      style={[
        styles.wrap,
        { backgroundColor: isUser ? colors.background : colors.surface }
      ]}
    >
      <View style={styles.inner}>
        <View
          style={[
            styles.avatar,
            { backgroundColor: isUser ? colors.primary + '20' : colors.primary }
          ]}
        >
          <Text
            style={{
              color: isUser ? colors.primary : '#ffffff',
              fontSize: 12,
              fontWeight: '700'
            }}
          >
            {isUser ? (user?.displayName?.slice(0, 1).toUpperCase() || 'U') : 'C'}
          </Text>
        </View>

        <View style={{ flex: 1, minWidth: 0 }}>
          <View style={styles.header}>
            <Text variant="label" style={{ fontWeight: '600' }}>
              {isUser ? 'You' : 'Chima'}
            </Text>
            {isStreaming && (
              <Text variant="caption" muted style={{ marginLeft: 8 }}>
                {message.status === 'pending' ? 'Thinking…' : 'Writing…'}
              </Text>
            )}
          </View>

          <Pressable onLongPress={copyMessage} delayLongPress={400}>
            {isUser ? (
              <Text style={{ marginTop: 4 }}>{message.content}</Text>
            ) : (
              <View style={{ marginTop: 4 }}>
                {message.content ? (
                  <MarkdownRenderer content={message.content} />
                ) : isStreaming ? (
                  <Text muted>…</Text>
                ) : null}
              </View>
            )}
          </Pressable>

          {isFailed && (
            <View
              style={[
                styles.failBox,
                {
                  borderColor: colors.destructive + '60',
                  backgroundColor: colors.destructive + '10'
                }
              ]}
            >
              <View style={{ flexDirection: 'row', alignItems: 'flex-start', gap: 8 }}>
                <AlertTriangle size={16} color={colors.destructive} />
                <View style={{ flex: 1 }}>
                  <Text
                    style={{ color: colors.destructive, fontWeight: '600', fontSize: 13 }}
                  >
                    Response failed
                  </Text>
                  <Text variant="caption" muted style={{ marginTop: 2 }}>
                    {message.errorMessage ?? 'Something went wrong.'}
                  </Text>
                </View>
              </View>
              {onRegenerate && (
                <Pressable
                  onPress={() => onRegenerate(message.id)}
                  style={[
                    styles.retryBtn,
                    { borderColor: colors.destructive, marginTop: 10 }
                  ]}
                >
                  <Text
                    style={{
                      color: colors.destructive,
                      fontSize: 13,
                      fontWeight: '600'
                    }}
                  >
                    Retry
                  </Text>
                </Pressable>
              )}
            </View>
          )}

          {!isStreaming && !isFailed && message.content ? (
            <View style={styles.actions}>
              <Pressable onPress={copyMessage} style={styles.actionBtn} hitSlop={6}>
                {copied ? (
                  <Check size={12} color={colors.textMuted} />
                ) : (
                  <Copy size={12} color={colors.textMuted} />
                )}
                <Text variant="caption" muted style={{ marginLeft: 4 }}>
                  {copied ? 'Copied' : 'Copy'}
                </Text>
              </Pressable>

              {!isUser && (
                <Pressable onPress={toggleSpeak} style={styles.actionBtn} hitSlop={6}>
                  {speaking ? (
                    <VolumeX size={12} color={colors.primary} />
                  ) : (
                    <Volume2 size={12} color={colors.textMuted} />
                  )}
                  <Text
                    variant="caption"
                    muted={!speaking}
                    style={{ marginLeft: 4, color: speaking ? colors.primary : undefined }}
                  >
                    {speaking ? 'Stop' : 'Read'}
                  </Text>
                </Pressable>
              )}

              {!isUser && onRegenerate && (
                <Pressable
                  onPress={() => {
                    onRegenerate(message.id);
                    Haptics.impactAsync(Haptics.ImpactFeedbackStyle.Light);
                  }}
                  style={styles.actionBtn}
                  hitSlop={6}
                >
                  <RefreshCw size={12} color={colors.textMuted} />
                  <Text variant="caption" muted style={{ marginLeft: 4 }}>
                    Regenerate
                  </Text>
                </Pressable>
              )}
            </View>
          ) : null}
        </View>
      </View>
    </View>
  );
}

const styles = StyleSheet.create({
  wrap: { paddingHorizontal: 16, paddingVertical: 14 },
  inner: { flexDirection: 'row', gap: 12 },
  avatar: {
    width: 32,
    height: 32,
    borderRadius: 16,
    alignItems: 'center',
    justifyContent: 'center',
    marginTop: 2
  },
  header: { flexDirection: 'row', alignItems: 'center' },
  actions: { flexDirection: 'row', gap: 12, marginTop: 10, flexWrap: 'wrap' },
  actionBtn: { flexDirection: 'row', alignItems: 'center' },
  failBox: { marginTop: 8, padding: 12, borderRadius: 10, borderWidth: 1 },
  retryBtn: {
    height: 34,
    borderRadius: 8,
    borderWidth: 1,
    alignItems: 'center',
    justifyContent: 'center'
  }
});