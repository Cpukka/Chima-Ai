'use client';

import Link from 'next/link';
import { useQuery } from '@tanstack/react-query';
import { Check, Loader2 } from 'lucide-react';
import { Button } from '@/components/ui/button';
import { subscriptionsApi, type Plan } from '@/lib/api/subscriptions';
import { cn } from '@/lib/utils/cn';

export function PricingCards({ compact = false }: { compact?: boolean }) {
  const plans = useQuery({
    queryKey: ['subscription', 'plans'],
    queryFn: subscriptionsApi.plans
  });

  if (plans.isLoading) {
    return (
      <div className="flex items-center justify-center py-16">
        <Loader2 className="h-6 w-6 animate-spin text-muted-foreground" />
      </div>
    );
  }

  const list = plans.data ?? [];

  return (
    <div className={cn('grid gap-6', compact ? 'md:grid-cols-2' : 'md:grid-cols-2 lg:grid-cols-4')}>
      {list.map((plan) => (
        <PlanCard key={plan.id} plan={plan} />
      ))}
    </div>
  );
}

function PlanCard({ plan }: { plan: Plan }) {
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

      <div className="mb-6">
        <h3 className="text-lg font-semibold">{plan.displayName}</h3>
        {plan.description && (
          <p className="mt-1 text-sm text-muted-foreground">{plan.description}</p>
        )}
      </div>

      <div className="mb-6">
        <span className="text-4xl font-semibold">${plan.monthlyPrice}</span>
        <span className="ml-1 text-sm text-muted-foreground">/month</span>
      </div>

      <ul className="flex-1 space-y-3 mb-6">
        <Feature label={`${plan.limits.monthlyMessages.toLocaleString()} messages / month`} />
        <Feature label={`${plan.limits.monthlyFileUploads} file uploads / month`} />
        <Feature label={`${(plan.limits.monthlyTokens / 1000).toFixed(0)}K tokens / month`} />
        <Feature label={`${plan.limits.maxFileSizeMb} MB per file`} />
      </ul>

      <Button variant={highlighted ? 'default' : 'outline'} asChild className="w-full">
        <Link href={`/register?plan=${plan.code}`}>
          {plan.code === 'free' ? 'Get started' : `Choose ${plan.displayName}`}
        </Link>
      </Button>
    </div>
  );
}

function Feature({ label }: { label: string }) {
  return (
    <li className="flex items-start gap-2 text-sm">
      <Check className="h-4 w-4 text-primary mt-0.5 shrink-0" />
      <span className="text-muted-foreground">{label}</span>
    </li>
  );
}