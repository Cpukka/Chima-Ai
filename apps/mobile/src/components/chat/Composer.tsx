import { useRef, useState, useEffect } from 'react';
import { KeyboardAvoidingView, Platform, Pressable, StyleSheet, TextInput, View } from 'react-native';
import * as Haptics from 'expo-haptics';
import { ArrowUp, Square } from 'lucide-react-native';
import { useTheme } from '@/theme/ThemeProvider';

interface Props {
  onSend: (content: string) => void;
  onStop?: () => void;
  isStreaming?: boolean;
  disabled?: boolean;
  initialValue?: string;
}

export function Composer({ onSend, onStop, isStreaming, disabled, initialValue }: Props) {
  const { colors } = useTheme();
  const [value, setValue] = useState(initialValue ?? '');
  const inputRef = useRef<TextInput>(null);

  useEffect(() => {
    if (initialValue !== undefined) setValue(initialValue);
  }, [initialValue]);

  async function submit() {
    const v = value.trim();
    if (!v || disabled || isStreaming) return;
    await Haptics.impactAsync(Haptics.ImpactFeedbackStyle.Light);
    onSend(v);
    setValue('');
    inputRef.current?.focus();
  }

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
        <View
          style={[
            styles.inputRow,
            { backgroundColor: colors.surface, borderColor: colors.border }
          ]}
        >
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
              disabled={!value.trim() || disabled}
              style={[
                styles.sendBtn,
                {
                  backgroundColor: value.trim() ? colors.primary : colors.border
                }
              ]}
              hitSlop={8}
            >
              <ArrowUp size={16} color={value.trim() ? '#ffffff' : colors.textMuted} />
            </Pressable>
          )}
        </View>
      </View>
    </KeyboardAvoidingView>
  );
}

const styles = StyleSheet.create({
  wrap: { paddingHorizontal: 12, paddingTop: 8, paddingBottom: 12, borderTopWidth: StyleSheet.hairlineWidth },
  inputRow: {
    flexDirection: 'row',
    alignItems: 'flex-end',
    borderWidth: 1,
    borderRadius: 22,
    paddingLeft: 16,
    paddingRight: 6,
    paddingVertical: 6,
    minHeight: 44
  },
  input: {
    flex: 1,
    fontSize: 15,
    maxHeight: 120,
    paddingVertical: Platform.OS === 'ios' ? 8 : 4,
    paddingRight: 8
  },
  sendBtn: {
    width: 32,
    height: 32,
    borderRadius: 16,
    alignItems: 'center',
    justifyContent: 'center'
  }
});