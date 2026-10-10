import { apiFetch } from './client';

export interface PlanLimits {
  monthlyMessages: number;
  monthlyFileUploads: number;
  monthlyTokens: number;
  maxFileSizeMb: number;
  maxConversations: number;
  monthlyCreditAllowance: number;
}

export interface Plan {
  id: string;
  code: string;
  displayName: string;
  description: string | null;
  monthlyPrice: number;
  yearlyPrice: number;
  currency: string;
  sortOrder: number;
  isDefault: boolean;
  limits: PlanLimits;
}

export interface SubscriptionUsage {
  messagesUsed: number;
  messagesLimit: number;
  fileUploadsUsed: number;
  fileUploadsLimit: number;
  tokensUsed: number;
  tokensLimit: number;
  periodStart: string;
  periodEnd: string;
}

export interface SubscriptionStatus {
  planCode: string;
  planName: string;
  status: string;
  currentPeriodStart: string | null;
  currentPeriodEnd: string | null;
  autoRenew: boolean;
  provider: string | null;
  externalSubscriptionId: string | null;
  usage: SubscriptionUsage;
  limits: PlanLimits;
}

export const subscriptionsApi = {
  plans: () => apiFetch<Plan[]>('/api/subscriptions/plans', { skipAuth: true }),
  current: () => apiFetch<SubscriptionStatus>('/api/subscriptions/current'),
  cancel: () =>
    apiFetch<SubscriptionStatus>('/api/subscriptions/cancel', { method: 'POST' }),
  restore: () =>
    apiFetch<SubscriptionStatus>('/api/subscriptions/restore', { method: 'POST' }),
  devChangePlan: (planCode: string) =>
    apiFetch<SubscriptionStatus>('/api/subscriptions/dev/change-plan', {
      method: 'POST',
      body: JSON.stringify({ planCode, provider: 'manual' })
    })
};