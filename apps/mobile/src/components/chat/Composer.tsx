import { useRef, useState, useEffect } from 'react';
import {
  Alert,
  KeyboardAvoidingView,
  Platform,
  Pressable,
  ScrollView,
  StyleSheet,
  TextInput,
  View
} from 'react-native';
import * as Haptics from 'expo-haptics';
import * as DocumentPicker from 'expo-document-picker';
import { ArrowUp, Square, Paperclip, X, Loader2, Mic } from 'lucide-react-native';
import { useTheme } from '@/theme/ThemeProvider';
import { Text } from '@/components/ui/Text';
import { filesApi, formatBytes, type UploadedFile } from '@/api/files';

interface Props {
  onSend: (content: string, attachmentIds?: string[]) => void;
  onStop?: () => void;
  isStreaming?: boolean;
  disabled?: boolean;
  initialValue?: string;
}

const ALLOWED_MIME = [
  'text/plain',
  'text/markdown',
  'text/csv',
  'application/json',
  'application/pdf',
  'application/vnd.openxmlformats-officedocument.wordprocessingml.document'
];

export function Composer({ onSend, onStop, isStreaming, disabled, initialValue }: Props) {
  const { colors } = useTheme();
  const [value, setValue] = useState(initialValue ?? '');
  const [attachments, setAttachments] = useState<UploadedFile[]>([]);
  const [uploading, setUploading] = useState(false);
  const inputRef = useRef<TextInput>(null);

  useEffect(() => {
    if (initialValue !== undefined) setValue(initialValue);
  }, [initialValue]);

  async function pickAttachment() {
    try {
      const res = await DocumentPicker.getDocumentAsync({
        type: ALLOWED_MIME,
        copyToCacheDirectory: true,
        multiple: false
      });

      if (res.canceled || !res.assets?.[0]) return;
      const asset = res.assets[0];

      setUploading(true);
      await Haptics.impactAsync(Haptics.ImpactFeedbackStyle.Light);

      const uploaded = await filesApi.upload(
        asset.uri,
        asset.name,
        asset.mimeType ?? 'application/octet-stream'
      );

      setAttachments((prev) => [...prev, uploaded]);
      await Haptics.notificationAsync(Haptics.NotificationFeedbackType.Success);
    } catch (e) {
      Alert.alert('Upload failed', e instanceof Error ? e.message : 'Try again');
    } finally {
      setUploading(false);
    }
  }

  function removeAttachment(id: string) {
    setAttachments((prev) => prev.filter((a) => a.id !== id));
  }

  async function submit() {
    const v = value.trim();
    if ((!v && attachments.length === 0) || disabled || isStreaming) return;

    await Haptics.impactAsync(Haptics.ImpactFeedbackStyle.Medium);

    onSend(
      v || 'Please analyze the attached document.',
      attachments.map((a) => a.id)
    );

    setValue('');
    setAttachments([]);
    inputRef.current?.focus();
  }

  function voicePlaceholder() {
    Haptics.impactAsync(Haptics.ImpactFeedbackStyle.Light);
    Alert.alert(
      'Voice input coming soon',
      'Voice input requires a native dev build. It will be available in a future update.',
      [{ text: 'OK' }]
    );
  }

  const canSend = Boolean(value.trim() || attachments.length > 0);

  return (
    <KeyboardAvoidingView
      behavior={Platform.OS === 'ios' ? 'padding' : undefined}
      keyboardVerticalOffset={Platform.OS === 'ios' ? 90 : 0}
    >
      <View
        style={[
          styles.wrap,
          { backgroundColor: colors.background, borderTopColor: colors.border }
        ]}
      >
        {attachments.length > 0 && (
          <ScrollView
            horizontal
            showsHorizontalScrollIndicator={false}
            contentContainerStyle={styles.chipsRow}
            style={{ marginBottom: 8 }}
          >
            {attachments.map((a) => (
              <View
                key={a.id}
                style={[
                  styles.chip,
                  {
                    backgroundColor: colors.surface,
                    borderColor: colors.border
                  }
                ]}
              >
                <Paperclip size={12} color={colors.textMuted} />
                <Text
                  variant="caption"
                  numberOfLines={1}
                  style={{ marginHorizontal: 6, maxWidth: 120 }}
                >
                  {a.originalFileName}
                </Text>
                <Text variant="caption" muted style={{ marginRight: 6 }}>
                  {formatBytes(a.sizeBytes)}
                </Text>
                <Pressable onPress={() => removeAttachment(a.id)} hitSlop={6}>
                  <X size={12} color={colors.textMuted} />
                </Pressable>
              </View>
            ))}
          </ScrollView>
        )}

        <View
          style={[
            styles.inputRow,
            { backgroundColor: colors.surface, borderColor: colors.border }
          ]}
        >
          {/* Attachment button */}
          <Pressable
            onPress={pickAttachment}
            disabled={uploading || disabled || isStreaming}
            style={styles.iconBtn}
            hitSlop={6}
          >
            {uploading ? (
              <Loader2 size={18} color={colors.textMuted} />
            ) : (
              <Paperclip size={18} color={colors.textMuted} />
            )}
          </Pressable>

          <TextInput
            ref={inputRef}
            value={value}
            onChangeText={setValue}
            placeholder="Ask Chima anything…"
            placeholderTextColor={colors.textMuted}
            multiline
            style={[styles.input, { color: colors.text }]}
            editable={!disabled}
          />

          {/* Voice placeholder button — hidden while typing so it doesn't compete with Send */}
          {!value.trim() && attachments.length === 0 && !isStreaming && (
            <Pressable onPress={voicePlaceholder} style={styles.iconBtn} hitSlop={6}>
              <Mic size={18} color={colors.textMuted} />
            </Pressable>
          )}

          {/* Send / Stop */}
          {isStreaming ? (
            <Pressable
              onPress={onStop}
              style={[styles.sendBtn, { backgroundColor: colors.textMuted }]}
              hitSlop={8}
            >
              <Square size={14} color="#ffffff" fill="#ffffff" />
            </Pressable>
          ) : (
            <Pressable
              onPress={submit}
              disabled={!canSend || disabled}
              style={[
                styles.sendBtn,
                { backgroundColor: canSend ? colors.primary : colors.border }
              ]}
              hitSlop={8}
            >
              <ArrowUp size={16} color={canSend ? '#ffffff' : colors.textMuted} />
            </Pressable>
          )}
        </View>
      </View>
    </KeyboardAvoidingView>
  );
}

const styles = StyleSheet.create({
  wrap: {
    paddingHorizontal: 12,
    paddingTop: 8,
    paddingBottom: 12,
    borderTopWidth: StyleSheet.hairlineWidth
  },
  chipsRow: {
    gap: 6,
    paddingHorizontal: 2
  },
  chip: {
    flexDirection: 'row',
    alignItems: 'center',
    paddingHorizontal: 8,
    paddingVertical: 6,
    borderRadius: 8,
    borderWidth: 1,
    marginRight: 6
  },
  inputRow: {
    flexDirection: 'row',
    alignItems: 'flex-end',
    borderWidth: 1,
    borderRadius: 22,
    paddingLeft: 6,
    paddingRight: 6,
    paddingVertical: 6,
    minHeight: 44
  },
  iconBtn: {
    width: 32,
    height: 32,
    alignItems: 'center',
    justifyContent: 'center',
    borderRadius: 16
  },
  input: {
    flex: 1,
    fontSize: 15,
    maxHeight: 120,
    paddingVertical: Platform.OS === 'ios' ? 8 : 4,
    paddingHorizontal: 8
  },
  sendBtn: {
    width: 32,
    height: 32,
    borderRadius: 16,
    alignItems: 'center',
    justifyContent: 'center'
  }
});