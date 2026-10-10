import { Text as RNText, type TextProps as RNTextProps } from 'react-native';
import { useTheme } from '@/theme/ThemeProvider';

interface Props extends RNTextProps {
  variant?: 'body' | 'title' | 'subtitle' | 'caption' | 'label';
  muted?: boolean;
}

export function Text({ variant = 'body', muted, style, ...rest }: Props) {
  const { colors } = useTheme();

  const size =
    variant === 'title' ? 28 :
    variant === 'subtitle' ? 20 :
    variant === 'caption' ? 12 :
    variant === 'label' ? 13 :
    15;

  const weight =
    variant === 'title' ? '700' :
    variant === 'subtitle' ? '600' :
    variant === 'label' ? '500' :
    '400';

  return (
    <RNText
      style={[
        {
          fontSize: size,
          fontWeight: weight as '400' | '500' | '600' | '700',
          color: muted ? colors.textMuted : colors.text
        },
        style
      ]}
      {...rest}
    />
  );
}