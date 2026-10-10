'use client';

import { useRouter } from 'next/navigation';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { useState } from 'react';
import { toast } from 'sonner';
import { Check, Loader2, Sparkles, AlertTriangle } from 'lucide-react';
import { Button } from '@/components/ui/button';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { UsageBar } from '@/components/ui/usage-bar';
import { subscriptionsApi, type Plan, type SubscriptionStatus } from '@/lib/api/subscriptions';
import { cn } from '@/lib/utils/cn';

export default function SubscriptionPage() {
  const qc = useQueryClient();
  const [yearly, setYearly] = useState(false);

  const current = useQuery({
    queryKey: ['subscription', 'current'],
    queryFn: subscriptionsApi.current
  });

  const plans = useQuery({
    queryKey: ['subscription', 'plans'],
    queryFn: subscriptionsApi.plans
  });

  const cancel = useMutation({
    mutationFn: subscriptionsApi.cancel,
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: ['subscription'] });
      toast.success('Auto-renewal cancelled. You keep access until the end of the period.');
    }
  });

  const restore = useMutation({
    mutationFn: subscriptionsApi.restore,
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: ['subscription'] });
      toast.success('Subscription restored.');
    }
  });

  if (current.isLoading || plans.isLoading) {
    return (
      <div className="flex-1 flex items-center justify-center">
        <Loader2 className="h-6 w-6 animate-spin text-muted-foreground" />
      </div>
    );
  }

  const status = current.data!;
  const planList = plans.data ?? [];

  return (
    <div className="flex flex-col h-full pt-14 md:pt-0 overflow-y-auto">
      <header className="h-14 border-b border-border/40 flex items-center px-6 shrink-0">
        <h1 className="text-sm font-medium">Subscription</h1>
      </header>

      <div className="max-w-5xl mx-auto w-full px-6 py-8 space-y-10">
        {/* Current plan */}
        <section>
          <h2 className="text-2xl font-semibold tracking-tight mb-4">Current plan</h2>
          <Card>
            <CardContent className="p-6 space-y-6">
              <div className="flex items-start justify-between gap-4">
                <div>
                  <div className="flex items-center gap-2">
                    <Sparkles className="h-5 w-5 text-primary" />
                    <span className="text-xl font-semibold">{status.planName}</span>
                    {status.autoRenew ? (
                      <span className="text-xs px-2 py-0.5 rounded-full bg-primary/10 text-primary">
                        Auto-renew on
                      </span>
                    ) : (
                      <span className="text-xs px-2 py-0.5 rounded-full bg-amber-500/10 text-amber-600">
                        Cancels at period end
                      </span>
                    )}
                  </div>
                  {status.currentPeriodEnd && (
                    <p className="text-sm text-muted-foreground mt-1">
                      Renews {new Date(status.currentPeriodEnd).toLocaleDateString()}
                    </p>
                  )}
                </div>

                {status.planCode === 'free' ? (
                  <Button asChild>
                    <a href="#plans">Upgrade</a>
                  </Button>
                ) : status.autoRenew ? (
                  <Button
                    variant="outline"
                    onClick={() => cancel.mutate()}
                    disabled={cancel.isPending}
                  >
                    {cancel.isPending ? 'Cancelling…' : 'Cancel auto-renew'}
                  </Button>
                ) : (
                  <Button
                    onClick={() => restore.mutate()}
                    disabled={restore.isPending}
                  >
                    {restore.isPending ? 'Restoring…' : 'Restore auto-renew'}
                  </Button>
                )}
              </div>

              <div className="grid gap-4 sm:grid-cols-3">
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
                  label="Tokens used"
                />
              </div>

              {status.usage.messagesLimit > 0 &&
                status.usage.messagesUsed >= status.usage.messagesLimit && (
                  <div className="flex items-start gap-2 rounded-lg border border-destructive/40 bg-destructive/5 p-3 text-sm">
                    <AlertTriangle className="h-4 w-4 text-destructive shrink-0 mt-0.5" />
                    <div>
                      <div className="font-medium text-destructive">You&apos;ve hit your monthly limit</div>
                      <div className="text-muted-foreground text-xs mt-0.5">
                        Upgrade to keep using Chima. Your limit resets on{' '}
                        {new Date(status.usage.periodEnd).toLocaleDateString()}.
                      </div>
                    </div>
                  </div>
                )}
            </CardContent>
          </Card>
        </section>

        {/* Plans */}
        <section id="plans">
          <div className="flex items-center justify-between mb-4">
            <h2 className="text-2xl font-semibold tracking-tight">Plans</h2>
            <div className="inline-flex rounded-lg border border-border p-1 text-xs">
              <button
                onClick={() => setYearly(false)}
                className={cn(
                  'px-3 py-1 rounded-md',
                  !yearly ? 'bg-primary text-primary-foreground' : 'text-muted-foreground'
                )}
              >
                Monthly
              </button>
              <button
                onClick={() => setYearly(true)}
                className={cn(
                  'px-3 py-1 rounded-md',
                  yearly ? 'bg-primary text-primary-foreground' : 'text-muted-foreground'
                )}
              >
                Yearly (save ~17%)
              </button>
            </div>
          </div>

          <div className="grid gap-6 md:grid-cols-2 lg:grid-cols-4">
            {planList.map((plan) => (
              <PlanCard
                key={plan.id}
                plan={plan}
                yearly={yearly}
                current={status}
                onSwitch={async (code) => {
                  try {
                    await subscriptionsApi.devChangePlan(code);
                    qc.invalidateQueries({ queryKey: ['subscription'] });
                    toast.success(`Switched to ${plan.displayName}.`);
                  } catch (e) {
                    toast.error(e instanceof Error ? e.message : 'Switch failed');
                  }
                }}
              />
            ))}
          </div>

          <p className="text-xs text-muted-foreground mt-6 text-center max-w-2xl mx-auto">
            In this development build, plan changes are simulated and no payment is taken. Stripe
            Checkout (web) and Google Play Billing (Android) will be wired in a later phase.
          </p>
        </section>
      </div>
    </div>
  );
}

function PlanCard({
  plan,
  yearly,
  current,
  onSwitch
}: {
  plan: Plan;
  yearly: boolean;
  current: SubscriptionStatus;
  onSwitch: (code: string) => void;
}) {
  const isCurrent = current.planCode === plan.code;
  const price = yearly ? plan.yearlyPrice : plan.monthlyPrice;
  const period = yearly ? 'per year' : 'per month';
  const highlighted = plan.code === 'pro';

  return (
    <div
      className={cn(
        'relative flex flex-col rounded-2xl border p-6 bg-card',
        highlighted ? 'border-primary shadow-lg' : 'border-border/60'
      )}
    >
      {highlighted && (
        <div className="absolute -top-3 left-1/2 -translate-x-1/2 rounded-full bg-primary text-primary-foreground px-3 py-1 text-xs font-medium">
          Most popular
        </div>
      )}

      <div className="mb-4">
        <h3 className="text-lg font-semibold">{plan.displayName}</h3>
        {plan.description && (
          <p className="mt-1 text-xs text-muted-foreground">{plan.description}</p>
        )}
      </div>

      <div className="mb-5">
        <span className="text-3xl font-semibold">${price}</span>
        <span className="ml-1 text-xs text-muted-foreground">/{period}</span>
      </div>

      <ul className="flex-1 space-y-2.5 mb-5 text-sm">
        <Feature label={`${plan.limits.monthlyMessages.toLocaleString()} messages / month`} />
        <Feature label={`${plan.limits.monthlyFileUploads} file uploads / month`} />
        <Feature label={`${(plan.limits.monthlyTokens / 1000).toFixed(0)}K tokens / month`} />
        <Feature label={`${plan.limits.maxFileSizeMb} MB per file`} />
        {plan.limits.maxConversations < 0 ? (
          <Feature label="Unlimited conversations" />
        ) : (
          <Feature label={`Up to ${plan.limits.maxConversations} conversations`} />
        )}
      </ul>

      {isCurrent ? (
        <Button variant="outline" disabled className="w-full">
          Current plan
        </Button>
      ) : (
        <Button
          variant={highlighted ? 'default' : 'outline'}
          className="w-full"
          onClick={() => onSwitch(plan.code)}
        >
          Switch to {plan.displayName}
        </Button>
      )}
    </div>
  );
}

function Feature({ label }: { label: string }) {
  return (
    <li className="flex items-start gap-2">
      <Check className="h-4 w-4 text-primary mt-0.5 shrink-0" />
      <span className="text-muted-foreground">{label}</span>
    </li>
  );
}