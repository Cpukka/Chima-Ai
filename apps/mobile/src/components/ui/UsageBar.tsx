import { StyleSheet, View } from 'react-native';
import { useTheme } from '@/theme/ThemeProvider';
import { Text } from './Text';

interface Props {
  used: number;
  limit: number;
  label: string;
}

export function UsageBar({ used, limit, label }: Props) {
  const { colors } = useTheme();
  const unlimited = limit < 0;
  const pct = unlimited ? 0 : Math.min(100, (used / Math.max(1, limit)) * 100);
  const danger = !unlimited && pct >= 90;
  const warn = !unlimited && pct >= 70 && pct < 90;

  const barColor = danger ? colors.destructive : warn ? colors.warning : colors.primary;

  return (
    <View style={styles.wrap}>
      <View style={styles.labelRow}>
        <Text variant="caption" muted>
          {label}
        </Text>
        <Text variant="caption" style={{ fontWeight: '600' }}>
          {used.toLocaleString()}
          <Text variant="caption" muted>
            {' '}/ {unlimited ? '∞' : limit.toLocaleString()}
          </Text>
        </Text>
      </View>
      <View style={[styles.track, { backgroundColor: colors.border }]}>
        <View
          style={[
            styles.fill,
            {
              backgroundColor: barColor,
              width: `${unlimited ? 5 : Math.max(2, pct)}%`
            }
          ]}
        />
      </View>
    </View>
  );
}

const styles = StyleSheet.create({
  wrap: { marginBottom: 12 },
  labelRow: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    marginBottom: 6
  },
  track: { height: 6, borderRadius: 3, overflow: 'hidden' },
  fill: { height: '100%', borderRadius: 3 }
});