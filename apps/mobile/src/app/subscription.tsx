import { useCallback, useState } from 'react';
import {
  ActivityIndicator,
  Alert,
  Pressable,
  ScrollView,
  StyleSheet,
  View
} from 'react-native';
import { router, useFocusEffect } from 'expo-router';
import { useSafeAreaInsets } from 'react-native-safe-area-context';
import { ArrowLeft, Check, Sparkles } from 'lucide-react-native';
import { useTheme } from '@/theme/ThemeProvider';
import { Text } from '@/components/ui/Text';
import { UsageBar } from '@/components/ui/UsageBar';
import {
  subscriptionsApi,
  type Plan,
  type SubscriptionStatus
} from '@/api/subscriptions';

export default function SubscriptionScreen() {
  const { colors } = useTheme();
  const insets = useSafeAreaInsets();
  const [status, setStatus] = useState<SubscriptionStatus | null>(null);
  const [plans, setPlans] = useState<Plan[]>([]);
  const [loading, setLoading] = useState(true);
  const [busy, setBusy] = useState(false);
  const [yearly, setYearly] = useState(false);

  const load = useCallback(async () => {
    try {
      const [s, p] = await Promise.all([
        subscriptionsApi.current(),
        subscriptionsApi.plans()
      ]);
      setStatus(s);
      setPlans(p);
    } catch (e) {
      Alert.alert('Load failed', e instanceof Error ? e.message : 'Try again');
    } finally {
      setLoading(false);
    }
  }, []);

  useFocusEffect(
    useCallback(() => {
      load();
    }, [load])
  );

  async function switchPlan(planCode: string) {
    if (busy) return;
    setBusy(true);
    try {
      const s = await subscriptionsApi.devChangePlan(planCode);
      setStatus(s);
    } catch (e) {
      Alert.alert('Switch failed', e instanceof Error ? e.message : 'Try again');
    } finally {
      setBusy(false);
    }
  }

  async function cancel() {
    Alert.alert(
      'Cancel auto-renewal',
      'You will keep access until the end of the current period.',
      [
        { text: 'Keep', style: 'cancel' },
        {
          text: 'Cancel auto-renew',
          style: 'destructive',
          onPress: async () => {
            setBusy(true);
            try {
              const s = await subscriptionsApi.cancel();
              setStatus(s);
            } catch (e) {
              Alert.alert('Failed', e instanceof Error ? e.message : 'Try again');
            } finally {
              setBusy(false);
            }
          }
        }
      ]
    );
  }

  async function restore() {
    setBusy(true);
    try {
      const s = await subscriptionsApi.restore();
      setStatus(s);
    } catch (e) {
      Alert.alert('Failed', e instanceof Error ? e.message : 'Try again');
    } finally {
      setBusy(false);
    }
  }

  if (loading || !status) {
    return (
      <View style={{ flex: 1, backgroundColor: colors.background, alignItems: 'center', justifyContent: 'center' }}>
        <ActivityIndicator color={colors.primary} />
      </View>
    );
  }

  return (
    <View style={{ flex: 1, backgroundColor: colors.background }}>
      {/* Header */}
      <View
        style={[
          styles.header,
          { paddingTop: insets.top + 8, borderBottomColor: colors.border }
        ]}
      >
        <Pressable onPress={() => router.back()} style={styles.backBtn} hitSlop={8}>
          <ArrowLeft size={22} color={colors.text} />
        </Pressable>
        <Text style={{ fontWeight: '600' }}>Subscription</Text>
      </View>

      <ScrollView
        contentContainerStyle={{
          padding: 20,
          paddingBottom: insets.bottom + 40
        }}
      >
        {/* Current plan */}
        <View
          style={[
            styles.card,
            { backgroundColor: colors.surface, borderColor: colors.border }
          ]}
        >
          <View style={styles.currentRow}>
            <View style={{ flexDirection: 'row', alignItems: 'center', gap: 8 }}>
              <Sparkles size={20} color={colors.primary} />
              <Text variant="subtitle">{status.planName}</Text>
            </View>

            {status.autoRenew ? (
              <View style={[styles.badge, { backgroundColor: colors.primary + '20' }]}>
                <Text variant="caption" style={{ color: colors.primary, fontWeight: '600' }}>
                  Auto-renew
                </Text>
              </View>
            ) : (
              <View style={[styles.badge, { backgroundColor: colors.warning + '20' }]}>
                <Text variant="caption" style={{ color: colors.warning, fontWeight: '600' }}>
                  Cancels soon
                </Text>
              </View>
            )}
          </View>

          {status.currentPeriodEnd && (
            <Text variant="caption" muted style={{ marginTop: 4 }}>
              {status.autoRenew ? 'Renews' : 'Ends'}{' '}
              {new Date(status.currentPeriodEnd).toLocaleDateString()}
            </Text>
          )}

          <View style={{ marginTop: 20 }}>
            <UsageBar
              used={status.usage.messagesUsed}
              limit={status.usage.messagesLimit}
              label="AI messages"
            />
            <UsageBar
              used={status.usage.fileUploadsUsed}
              limit={status.usage.fileUploadsLimit}
              label="File uploads"
            />
            <UsageBar
              used={status.usage.tokensUsed}
              limit={status.usage.tokensLimit}
              label="Tokens"
            />
          </View>

          {status.planCode !== 'free' && (
            <View style={{ marginTop: 8 }}>
              {status.autoRenew ? (
                <Pressable
                  onPress={cancel}
                  disabled={busy}
                  style={[
                    styles.secondaryBtn,
                    { borderColor: colors.border }
                  ]}
                >
                  <Text style={{ color: colors.text }}>Cancel auto-renew</Text>
                </Pressable>
              ) : (
                <Pressable
                  onPress={restore}
                  disabled={busy}
                  style={[
                    styles.secondaryBtn,
                    { borderColor: colors.primary }
                  ]}
                >
                  <Text style={{ color: colors.primary }}>Restore auto-renew</Text>
                </Pressable>
              )}
            </View>
          )}
        </View>

        {/* Billing toggle */}
        <View style={styles.toggleRow}>
          <Text variant="subtitle">Plans</Text>
          <View style={[styles.toggle, { borderColor: colors.border }]}>
            <Pressable
              onPress={() => setYearly(false)}
              style={[
                styles.toggleBtn,
                !yearly && { backgroundColor: colors.primary }
              ]}
            >
              <Text
                variant="caption"
                style={{
                  color: !yearly ? '#fff' : colors.textMuted,
                  fontWeight: '600'
                }}
              >
                Monthly
              </Text>
            </Pressable>
            <Pressable
              onPress={() => setYearly(true)}
              style={[
                styles.toggleBtn,
                yearly && { backgroundColor: colors.primary }
              ]}
            >
              <Text
                variant="caption"
                style={{
                  color: yearly ? '#fff' : colors.textMuted,
                  fontWeight: '600'
                }}
              >
                Yearly
              </Text>
            </Pressable>
          </View>
        </View>

        {/* Plans */}
        <View style={{ gap: 12 }}>
          {plans.map((plan) => (
            <PlanCard
              key={plan.id}
              plan={plan}
              yearly={yearly}
              isCurrent={plan.code === status.planCode}
              busy={busy}
              onSwitch={() => switchPlan(plan.code)}
            />
          ))}
        </View>

        <Text variant="caption" muted style={{ marginTop: 20, textAlign: 'center' }}>
          Plan changes are simulated in this build. Google Play Billing and Stripe will be wired
          when billing accounts are ready.
        </Text>
      </ScrollView>
    </View>
  );
}

function PlanCard({
  plan,
  yearly,
  isCurrent,
  busy,
  onSwitch
}: {
  plan: Plan;
  yearly: boolean;
  isCurrent: boolean;
  busy: boolean;
  onSwitch: () => void;
}) {
  const { colors } = useTheme();
  const price = yearly ? plan.yearlyPrice : plan.monthlyPrice;
  const period = yearly ? 'per year' : 'per month';
  const highlighted = plan.code === 'pro';

  return (
    <View
      style={[
        styles.planCard,
        {
          backgroundColor: colors.surface,
          borderColor: highlighted ? colors.primary : colors.border,
          borderWidth: highlighted ? 2 : 1
        }
      ]}
    >
      {highlighted && (
        <View style={[styles.popularBadge, { backgroundColor: colors.primary }]}>
          <Text variant="caption" style={{ color: '#fff', fontWeight: '600' }}>
            Most popular
          </Text>
        </View>
      )}

      <View style={styles.planHeader}>
        <View>
          <Text variant="subtitle">{plan.displayName}</Text>
          {plan.description && (
            <Text variant="caption" muted style={{ marginTop: 2, maxWidth: 220 }}>
              {plan.description}
            </Text>
          )}
        </View>
        <View style={{ alignItems: 'flex-end' }}>
          <Text variant="title">${price}</Text>
          <Text variant="caption" muted>
            /{period}
          </Text>
        </View>
      </View>

      <View style={{ marginTop: 16, gap: 6 }}>
        <Feature text={`${plan.limits.monthlyMessages.toLocaleString()} messages / month`} />
        <Feature text={`${plan.limits.monthlyFileUploads} file uploads / month`} />
        <Feature text={`${(plan.limits.monthlyTokens / 1000).toFixed(0)}K tokens / month`} />
        <Feature text={`${plan.limits.maxFileSizeMb} MB per file`} />
      </View>

      {isCurrent ? (
        <View
          style={[
            styles.currentBadge,
            { borderColor: colors.border, marginTop: 16 }
          ]}
        >
          <Text variant="caption" muted style={{ fontWeight: '600' }}>
            Current plan
          </Text>
        </View>
      ) : (
        <Pressable
          onPress={onSwitch}
          disabled={busy}
          style={({ pressed }) => [
            styles.switchBtn,
            {
              backgroundColor: highlighted ? colors.primary : 'transparent',
              borderColor: highlighted ? colors.primary : colors.border,
              opacity: busy ? 0.5 : pressed ? 0.85 : 1,
              marginTop: 16
            }
          ]}
        >
          <Text
            style={{
              color: highlighted ? '#fff' : colors.text,
              fontWeight: '600',
              textAlign: 'center'
            }}
          >
            Switch to {plan.displayName}
          </Text>
        </Pressable>
      )}
    </View>
  );
}

function Feature({ text }: { text: string }) {
  const { colors } = useTheme();
  return (
    <View style={{ flexDirection: 'row', alignItems: 'center', gap: 8 }}>
      <Check size={14} color={colors.primary} />
      <Text variant="caption" muted>
        {text}
      </Text>
    </View>
  );
}

const styles = StyleSheet.create({
  header: {
    flexDirection: 'row',
    alignItems: 'center',
    gap: 8,
    paddingHorizontal: 12,
    paddingBottom: 12,
    borderBottomWidth: StyleSheet.hairlineWidth
  },
  backBtn: { width: 32, height: 32, alignItems: 'center', justifyContent: 'center' },
  card: { borderWidth: 1, borderRadius: 16, padding: 18, marginBottom: 24 },
  currentRow: {
    flexDirection: 'row',
    alignItems: 'center',
    justifyContent: 'space-between'
  },
  badge: { paddingHorizontal: 10, paddingVertical: 4, borderRadius: 999 },
  secondaryBtn: {
    height: 44,
    borderRadius: 12,
    borderWidth: 1,
    alignItems: 'center',
    justifyContent: 'center',
    marginTop: 8
  },
  toggleRow: {
    flexDirection: 'row',
    alignItems: 'center',
    justifyContent: 'space-between',
    marginBottom: 12
  },
  toggle: { flexDirection: 'row', borderWidth: 1, borderRadius: 10, padding: 2 },
  toggleBtn: { paddingHorizontal: 12, paddingVertical: 6, borderRadius: 8 },
  planCard: { borderRadius: 16, padding: 18, position: 'relative' },
  popularBadge: {
    position: 'absolute',
    top: -10,
    left: 18,
    paddingHorizontal: 10,
    paddingVertical: 3,
    borderRadius: 999
  },
  planHeader: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    alignItems: 'flex-start',
    marginTop: 4
  },
  switchBtn: {
    height: 44,
    borderRadius: 12,
    borderWidth: 1,
    alignItems: 'center',
    justifyContent: 'center'
  },
  currentBadge: {
    height: 44,
    borderRadius: 12,
    borderWidth: 1,
    alignItems: 'center',
    justifyContent: 'center'
  }
});