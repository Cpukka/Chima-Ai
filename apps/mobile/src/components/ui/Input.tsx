import { forwardRef } from 'react';
import { StyleSheet, TextInput, View, type TextInputProps } from 'react-native';
import { useTheme } from '@/theme/ThemeProvider';

interface Props extends TextInputProps {
  error?: string;
  leftIcon?: React.ReactNode;
}

export const Input = forwardRef<TextInput, Props>(({ error, leftIcon, style, ...rest }, ref) => {
  const { colors } = useTheme();
  return (
    <View
      style={[
        styles.wrap,
        {
          backgroundColor: colors.surface,
          borderColor: error ? colors.destructive : colors.border
        }
      ]}
    >
      {leftIcon ? <View style={styles.icon}>{leftIcon}</View> : null}
      <TextInput
        ref={ref}
        placeholderTextColor={colors.textMuted}
        style={[styles.input, { color: colors.text }, leftIcon ? { paddingLeft: 0 } : null, style]}
        {...rest}
      />
    </View>
  );
});
Input.displayName = 'Input';

const styles = StyleSheet.create({
  wrap: {
    flexDirection: 'row',
    alignItems: 'center',
    borderWidth: 1,
    borderRadius: 12,
    height: 48,
    paddingHorizontal: 14
  },
  icon: { marginRight: 8 },
  input: { flex: 1, fontSize: 15, paddingVertical: 0 }
});