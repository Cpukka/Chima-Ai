import { MarkdownStream } from 'react-native-markdown-stream';
import { View } from 'react-native';
import { useTheme } from '@/theme/ThemeProvider';

interface Props {
  content: string;
}

export function MarkdownRenderer({ content }: Props) {
  const { isDark } = useTheme();

  return (
    <View>
      <MarkdownStream
        content={content}
        theme={isDark ? 'dark' : 'light'}
      />
    </View>
  );
}