import { useCallback, useEffect, useMemo, useRef, useState } from 'react';
import {
  ActivityIndicator,
  Alert,
  FlatList,
  KeyboardAvoidingView,
  Platform,
  Pressable,
  StyleSheet,
  View
} from 'react-native';
import { router, useLocalSearchParams } from 'expo-router';
import { useSafeAreaInsets } from 'react-native-safe-area-context';
import { ArrowLeft } from 'lucide-react-native';
import { useTheme } from '@/theme/ThemeProvider';
import { Text } from '@/components/ui/Text';
import { MessageBubble } from '@/components/chat/MessageBubble';
import { Composer } from '@/components/chat/Composer';
import { conversationsApi } from '@/api/conversations';
import { messagesApi } from '@/api/messages';
import { session } from '@/auth/session';
import { streamMessage } from '@/sse/stream';
import { useAuthStore } from '@/store/auth-store';
import type { ConversationDto, MessageDto } from '@/api/types';

export default function ChatScreen() {
  const { id, prompt } = useLocalSearchParams<{ id: string; prompt?: string }>();
  const { colors } = useTheme();
  const insets = useSafeAreaInsets();
  const user = useAuthStore((s) => s.user);

  const [conversation, setConversation] = useState<ConversationDto | null>(null);
  const [messages, setMessages] = useState<MessageDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [streaming, setStreaming] = useState(false);
  const [liveContent, setLiveContent] = useState('');

  const listRef = useRef<FlatList<MessageDto>>(null);
  const abortRef = useRef<AbortController | null>(null);
  const sentInitialPrompt = useRef(false);

  // ---- Initial load ----
  useEffect(() => {
    let cancelled = false;
    (async () => {
      try {
        const [c, msgs] = await Promise.all([
          conversationsApi.get(id),
          messagesApi.list(id, { limit: 100, ascending: true })
        ]);
        if (!cancelled) {
          setConversation(c);
          setMessages(msgs.items);
        }
      } catch (e) {
        if (!cancelled) Alert.alert('Load failed', e instanceof Error ? e.message : 'Try again');
      } finally {
        if (!cancelled) setLoading(false);
      }
    })();
    return () => {
      cancelled = true;
    };
  }, [id]);

  // ---- Send initial prompt from URL (?prompt=...) ----
  useEffect(() => {
    if (!prompt || sentInitialPrompt.current || loading) return;
    sentInitialPrompt.current = true;
    // Clear the param to avoid resending on hot reload
    router.setParams({ prompt: undefined } as never);
    void handleSend(prompt);
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [prompt, loading]);

  const scrollToEnd = useCallback(() => {
    requestAnimationFrame(() => {
      listRef.current?.scrollToEnd({ animated: true });
    });
  }, []);

  const handleSend = useCallback(
    async (content: string) => {
      if (!content.trim() || streaming) return;
      const token = session.getAccess();
      if (!token) {
        router.replace('/(auth)/login');
        return;
      }

      setStreaming(true);
      setLiveContent('');

      // Optimistic user message
      const optimistic: MessageDto = {
        id: `optimistic-${Date.now()}`,
        conversationId: id,
        role: 'user',
        status: 'completed',
        content,
        parentMessageId: null,
        branchRootId: null,
        modelId: null,
        inputTokens: null,
        outputTokens: null,
        estimatedCost: null,
        durationMs: null,
        errorCode: null,
        errorMessage: null,
        createdAt: new Date().toISOString(),
        updatedAt: new Date().toISOString()
      };
      setMessages((prev) => [...prev, optimistic]);
      setTimeout(scrollToEnd, 50);

      const controller = streamMessage(
        messagesApi.streamUrl(id),
        { content },
        token,
        {
          onMeta: () => {},
          onDelta: (d) => {
            setLiveContent((v) => v + d.content);
            scrollToEnd();
          },
          onDone: async () => {
            setStreaming(false);
            setLiveContent('');
            // Refresh from server to get real messages with IDs
            try {
              const msgs = await messagesApi.list(id, { limit: 100, ascending: true });
              setMessages(msgs.items);
            } catch {
              /* ignore */
            }
          },
          onError: async (e) => {
            setStreaming(false);
            setLiveContent('');
            if (e.code !== 'cancelled') {
              Alert.alert('AI error', e.message || 'The AI request failed.');
            }
            try {
              const msgs = await messagesApi.list(id, { limit: 100, ascending: true });
              setMessages(msgs.items);
            } catch {
              /* ignore */
            }
          }
        }
      );
      abortRef.current = controller;
    },
    [id, streaming, scrollToEnd]
  );

  function handleStop() {
    abortRef.current?.abort();
    setStreaming(false);
    setLiveContent('');
  }

  async function handleRegenerate(messageId: string) {
    try {
      await messagesApi.regenerate(messageId);
      const msgs = await messagesApi.list(id, { limit: 100, ascending: true });
      setMessages(msgs.items);
    } catch (e) {
      Alert.alert('Regenerate failed', e instanceof Error ? e.message : 'Try again');
    }
  }

  // Displayed list = messages + live streaming bubble
  const displayed: MessageDto[] = useMemo(() => {
    if (!streaming || !liveContent) return messages;
    return [
      ...messages,
      {
        id: 'live-assistant',
        conversationId: id,
        role: 'assistant',
        status: 'streaming',
        content: liveContent,
        parentMessageId: null,
        branchRootId: null,
        modelId: null,
        inputTokens: null,
        outputTokens: null,
        estimatedCost: null,
        durationMs: null,
        errorCode: null,
        errorMessage: null,
        createdAt: new Date().toISOString(),
        updatedAt: new Date().toISOString()
      }
    ];
  }, [messages, streaming, liveContent, id]);

  return (
    <KeyboardAvoidingView
      behavior={Platform.OS === 'ios' ? 'padding' : undefined}
      keyboardVerticalOffset={0}
      style={{ flex: 1, backgroundColor: colors.background }}
    >
      {/* Header */}
      <View
        style={[
          styles.header,
          {
            paddingTop: insets.top + 8,
            borderBottomColor: colors.border,
            backgroundColor: colors.background
          }
        ]}
      >
        <Pressable onPress={() => router.back()} style={styles.backBtn} hitSlop={8}>
          <ArrowLeft size={22} color={colors.text} />
        </Pressable>
        <View style={{ flex: 1, minWidth: 0 }}>
          <Text style={{ fontWeight: '600' }} numberOfLines={1}>
            {conversation?.title ?? 'Chat'}
          </Text>
        </View>
      </View>

      {/* Messages */}
      {loading ? (
        <View style={styles.center}>
          <ActivityIndicator color={colors.primary} />
        </View>
      ) : (
        <FlatList
          ref={listRef}
          data={displayed}
          keyExtractor={(m) => m.id}
          renderItem={({ item }) => (
            <MessageBubble message={item} user={user} onRegenerate={handleRegenerate} />
          )}
          contentContainerStyle={{ paddingVertical: 12, paddingBottom: 20 }}
          onContentSizeChange={scrollToEnd}
          keyboardShouldPersistTaps="handled"
        />
      )}

      {/* Composer */}
      <View style={{ paddingBottom: insets.bottom }}>
        <Composer
          onSend={handleSend}
          onStop={handleStop}
          isStreaming={streaming}
          disabled={loading}
        />
      </View>
    </KeyboardAvoidingView>
  );
}

const styles = StyleSheet.create({
  header: {
    flexDirection: 'row',
    alignItems: 'center',
    paddingHorizontal: 12,
    paddingBottom: 12,
    borderBottomWidth: StyleSheet.hairlineWidth,
    gap: 8
  },
  backBtn: { width: 32, height: 32, alignItems: 'center', justifyContent: 'center' },
  center: { flex: 1, alignItems: 'center', justifyContent: 'center' }
});