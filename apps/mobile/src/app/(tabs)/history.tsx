import { useCallback, useMemo, useState } from 'react';
import { ActivityIndicator, Alert, Pressable, ScrollView, StyleSheet, View } from 'react-native';
import { router, useFocusEffect } from 'expo-router';
import { useSafeAreaInsets } from 'react-native-safe-area-context';
import { Pin, Trash2 } from 'lucide-react-native';
import { useTheme } from '@/theme/ThemeProvider';
import { Text } from '@/components/ui/Text';
import { conversationsApi } from '@/api/conversations';
import { groupByDate, type DateGroup } from '@/utils/date';
import type { ConversationDto } from '@/api/types';

const GROUP_ORDER: DateGroup[] = ['Today', 'Yesterday', 'Previous 7 days', 'Older'];

export default function HistoryTab() {
  const { colors } = useTheme();
  const insets = useSafeAreaInsets();
  const [items, setItems] = useState<ConversationDto[]>([]);
  const [loading, setLoading] = useState(true);

  const load = useCallback(async () => {
    try {
      const res = await conversationsApi.list({ limit: 100 });
      setItems(res.items);
    } catch {
      setItems([]);
    } finally {
      setLoading(false);
    }
  }, []);

  useFocusEffect(
    useCallback(() => {
      load();
    }, [load])
  );

  const grouped = useMemo(() => {
    const map: Record<DateGroup, ConversationDto[]> = {
      Today: [],
      Yesterday: [],
      'Previous 7 days': [],
      Older: []
    };
    for (const c of items) map[groupByDate(c.updatedAt)].push(c);
    return map;
  }, [items]);

  async function deleteConv(id: string) {
    Alert.alert('Delete conversation', 'This cannot be undone.', [
      { text: 'Cancel', style: 'cancel' },
      {
        text: 'Delete',
        style: 'destructive',
        onPress: async () => {
          try {
            await conversationsApi.delete(id);
            setItems((prev) => prev.filter((c) => c.id !== id));
          } catch {
            Alert.alert('Delete failed', 'Please try again.');
          }
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
      <Text variant="title">History</Text>
      <Text muted style={{ marginTop: 4 }}>
        All your conversations
      </Text>

      {loading && (
        <View style={{ paddingVertical: 40, alignItems: 'center' }}>
          <ActivityIndicator color={colors.primary} />
        </View>
      )}

      {!loading && items.length === 0 && (
        <View style={{ paddingVertical: 40, alignItems: 'center' }}>
          <Text muted>No conversations yet</Text>
          <Text variant="caption" muted style={{ marginTop: 4 }}>
            Start a new chat to see it here
          </Text>
        </View>
      )}

      {!loading &&
        GROUP_ORDER.map((group) => {
          const list = grouped[group];
          if (!list.length) return null;
          return (
            <View key={group} style={{ marginTop: 20 }}>
              <Text
                variant="caption"
                muted
                style={{ marginBottom: 8, textTransform: 'uppercase' }}
              >
                {group}
              </Text>
              <View style={{ gap: 8 }}>
                {list.map((c) => (
                  <Pressable
                    key={c.id}
                    onPress={() => router.push(`/chat/${c.id}` as never)}
                    onLongPress={() => deleteConv(c.id)}
                    style={({ pressed }) => [
                      styles.row,
                      {
                        backgroundColor: colors.surface,
                        borderColor: colors.border,
                        opacity: pressed ? 0.85 : 1
                      }
                    ]}
                  >
                    {c.isPinned && (
                      <Pin size={12} color={colors.primary} style={{ marginRight: 6 }} />
                    )}
                    <View style={{ flex: 1 }}>
                      <Text style={{ fontWeight: '500' }} numberOfLines={1}>
                        {c.title}
                      </Text>
                      {c.lastMessagePreview ? (
                        <Text
                          variant="caption"
                          muted
                          numberOfLines={1}
                          style={{ marginTop: 2 }}
                        >
                          {c.lastMessagePreview}
                        </Text>
                      ) : null}
                    </View>
                    <Pressable onPress={() => deleteConv(c.id)} hitSlop={8}>
                      <Trash2 size={16} color={colors.textMuted} />
                    </Pressable>
                  </Pressable>
                ))}
              </View>
            </View>
          );
        })}
    </ScrollView>
  );
}

const styles = StyleSheet.create({
  row: {
    flexDirection: 'row',
    alignItems: 'center',
    padding: 14,
    borderRadius: 12,
    borderWidth: 1
  }
});