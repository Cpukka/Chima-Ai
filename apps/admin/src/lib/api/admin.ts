import { apiFetch } from './client';
import type { DashboardMetrics, TimeSeriesPoint } from './types';

export interface PagedAdminResult<T> {
  items: T[];
  page: number;
  pageSize: number;
  total: number;
  totalPages: number;
}

export interface AdminUserListItem {
  id: string;
  email: string;
  displayName: string | null;
  emailVerified: boolean;
  status: string;
  planCode: string;
  planName: string;
  createdAt: string;
  lastLoginAt: string | null;
  isAdmin: boolean;
  totalMessages: number;
  totalSpent: number;
}

export interface AdminUserDetail {
  id: string;
  email: string;
  displayName: string | null;
  avatarUrl: string | null;
  emailVerified: boolean;
  status: string;
  isAdmin: boolean;
  adminRole: string | null;
  createdAt: string;
  lastLoginAt: string | null;
  planCode: string;
  planName: string;
  subscriptionStatus: string;
  currentPeriodEnd: string | null;
  autoRenew: boolean;
  messagesUsedThisMonth: number;
  messagesLimit: number;
  fileUploadsUsedThisMonth: number;
  fileUploadsLimit: number;
  tokensUsedThisMonth: number;
  totalSpent: number;
  conversationCount: number;
  messageCount: number;
  fileCount: number;
  recentPayments: AdminPayment[];
}

export interface AdminPayment {
  id: string;
  amount: number;
  currency: string;
  status: string;
  provider: string;
  externalPaymentId: string | null;
  createdAt: string;
  paidAt: string | null;
}

export interface RevenueMetrics {
  totalRevenue: number;
  revenueInPeriod: number;
  mrr: number;
  arr: number;
  activeSubscriptions: number;
  cancelledInPeriod: number;
  newSubscriptionsInPeriod: number;
  churnPercent: number;
  byPlan: { planCode: string; planName: string; subscribers: number; mrr: number }[];
}

export interface AiCostMetrics {
  requestsInPeriod: number;
  inputTokensInPeriod: number;
  outputTokensInPeriod: number;
  estimatedCostInPeriod: number;
  revenueInPeriod: number;
  grossMarginInPeriod: number;
  grossMarginPercent: number;
  byModel: {
    modelId: string;
    modelName: string;
    providerName: string;
    requests: number;
    inputTokens: number;
    outputTokens: number;
    estimatedCost: number;
  }[];
  dailyCost: TimeSeriesPoint[];
}

export const adminApi = {
  dashboard: (days = 30) => apiFetch<DashboardMetrics>(`/api/admin/dashboard?days=${days}`),
  revenue: (days = 30) => apiFetch<RevenueMetrics>(`/api/admin/revenue?days=${days}`),
  aiCosts: (days = 30) => apiFetch<AiCostMetrics>(`/api/admin/ai-costs?days=${days}`),

  users: (params: { page?: number; pageSize?: number; search?: string; status?: string; plan?: string } = {}) => {
    const q = new URLSearchParams();
    if (params.page) q.set('page', String(params.page));
    if (params.pageSize) q.set('pageSize', String(params.pageSize));
    if (params.search) q.set('search', params.search);
    if (params.status) q.set('status', params.status);
    if (params.plan) q.set('plan', params.plan);
    return apiFetch<PagedAdminResult<AdminUserListItem>>(`/api/admin/users?${q}`);
  },

  user: (id: string) => apiFetch<AdminUserDetail>(`/api/admin/users/${id}`),

  suspendUser: (id: string, reason: string) =>
    apiFetch<Record<string, never>>(`/api/admin/users/${id}/suspend`, {
      method: 'POST',
      body: JSON.stringify({ reason })
    }),

  unsuspendUser: (id: string) =>
    apiFetch<Record<string, never>>(`/api/admin/users/${id}/unsuspend`, { method: 'POST' }),

  changeUserPlan: (id: string, planCode: string, reason: string) =>
    apiFetch<Record<string, never>>(`/api/admin/users/${id}/change-plan`, {
      method: 'POST',
      body: JSON.stringify({ planCode, reason })
    })
};
// ---------- Payments ----------

export interface AdminPaymentListItem {
  id: string;
  userId: string;
  userEmail: string;
  userDisplayName: string | null;
  amount: number;
  currency: string;
  status: string;
  provider: string;
  externalPaymentId: string | null;
  subscriptionPlanCode: string | null;
  createdAt: string;
  paidAt: string | null;
  refundedAt: string | null;
}

// ---------- Subscriptions ----------

export interface AdminSubscriptionListItem {
  id: string;
  userId: string;
  userEmail: string;
  userDisplayName: string | null;
  planCode: string;
  planName: string;
  monthlyPrice: number;
  status: string;
  provider: string;
  externalSubscriptionId: string | null;
  autoRenew: boolean;
  currentPeriodStart: string;
  currentPeriodEnd: string;
  cancelledAt: string | null;
  createdAt: string;
}

export interface AdminPlanOption {
  id: string;
  code: string;
  displayName: string;
  monthlyPrice: number;
  yearlyPrice: number;
  currency: string;
  isActive: boolean;
  isDefault: boolean;
  sortOrder: number;
}

export interface AdminListQuery {
  page?: number;
  pageSize?: number;
  search?: string;
  status?: string;
  provider?: string;
  planCode?: string;
  from?: string;
  to?: string;
}

export interface AdminPagedResult<T> {
  items: T[];
  page: number;
  pageSize: number;
  total: number;
  totalPages: number;
}

export const adminPaymentsApi = {
  list: (params: AdminListQuery = {}) => {
    const q = new URLSearchParams();
    if (params.page) q.set('page', String(params.page));
    if (params.pageSize) q.set('pageSize', String(params.pageSize));
    if (params.search) q.set('search', params.search);
    if (params.status) q.set('status', params.status);
    if (params.provider) q.set('provider', params.provider);
    if (params.from) q.set('from', params.from);
    if (params.to) q.set('to', params.to);
    return apiFetch<AdminPagedResult<AdminPaymentListItem>>(`/api/admin/payments?${q}`);
  }
};

export const adminSubscriptionsApi = {
  list: (params: AdminListQuery = {}) => {
    const q = new URLSearchParams();
    if (params.page) q.set('page', String(params.page));
    if (params.pageSize) q.set('pageSize', String(params.pageSize));
    if (params.search) q.set('search', params.search);
    if (params.status) q.set('status', params.status);
    if (params.planCode) q.set('planCode', params.planCode);
    return apiFetch<AdminPagedResult<AdminSubscriptionListItem>>(`/api/admin/subscriptions?${q}`);
  }
};

export const adminPlansApi = {
  list: () => apiFetch<AdminPlanOption[]>('/api/admin/plans')
};
// ---------- AI ----------

export interface AdminAiProvider {
  id: string;
  name: string;
  displayName: string;
  baseUrl: string;
  isActive: boolean;
  apiKeySecretName: string;
  modelCount: number;
}

export interface AdminAiModel {
  id: string;
  providerId: string;
  providerName: string;
  name: string;
  displayName: string;
  modelIdentifier: string;
  inputPricePer1M: number;
  outputPricePer1M: number;
  contextWindow: number;
  maxOutputTokens: number;
  isActive: boolean;
  isDefault: boolean;
  isPremium: boolean;
  capabilitiesCsv: string;
}

export const adminAiApi = {
  listProviders: () => apiFetch<AdminAiProvider[]>('/api/admin/ai/providers'),
  createProvider: (body: {
    name: string;
    displayName: string;
    baseUrl: string;
    apiKeySecretName: string;
    isActive?: boolean;
  }) =>
    apiFetch<AdminAiProvider>('/api/admin/ai/providers', {
      method: 'POST',
      body: JSON.stringify(body)
    }),
  updateProvider: (id: string, body: { displayName?: string; baseUrl?: string; isActive?: boolean }) =>
    apiFetch<AdminAiProvider>(`/api/admin/ai/providers/${id}`, {
      method: 'PATCH',
      body: JSON.stringify(body)
    }),

  listModels: () => apiFetch<AdminAiModel[]>('/api/admin/ai/models'),
  createModel: (body: {
    providerId: string;
    name: string;
    displayName: string;
    modelIdentifier: string;
    inputPricePer1M: number;
    outputPricePer1M: number;
    contextWindow: number;
    maxOutputTokens?: number;
    isActive?: boolean;
    isPremium?: boolean;
    capabilitiesCsv?: string;
  }) =>
    apiFetch<AdminAiModel>('/api/admin/ai/models', {
      method: 'POST',
      body: JSON.stringify(body)
    }),
  updateModel: (id: string, body: {
    displayName?: string;
    inputPricePer1M?: number;
    outputPricePer1M?: number;
    contextWindow?: number;
    maxOutputTokens?: number;
    isActive?: boolean;
    isPremium?: boolean;
    capabilitiesCsv?: string;
  }) =>
    apiFetch<AdminAiModel>(`/api/admin/ai/models/${id}`, {
      method: 'PATCH',
      body: JSON.stringify(body)
    }),
  setDefaultModel: (id: string) =>
    apiFetch<Record<string, never>>(`/api/admin/ai/models/${id}/set-default`, { method: 'POST' })
};

// ---------- Feature flags ----------

export interface AdminFeatureFlag {
  id: string;
  key: string;
  displayName: string;
  description: string | null;
  isEnabled: boolean;
  enabledForPlansCsv: string | null;
  rolloutPercent: number;
  updatedAt: string;
}

export const adminFeatureFlagsApi = {
  list: () => apiFetch<AdminFeatureFlag[]>('/api/admin/feature-flags'),
  update: (key: string, body: { isEnabled?: boolean; enabledForPlansCsv?: string; rolloutPercent?: number }) =>
    apiFetch<AdminFeatureFlag>(`/api/admin/feature-flags/${key}`, {
      method: 'PATCH',
      body: JSON.stringify(body)
    })
};

// ---------- System settings ----------

export interface AdminSystemSetting {
  id: string;
  key: string;
  value: string;
  valueType: string;
  description: string | null;
  isSecret: boolean;
  updatedAt: string;
}

export const adminSettingsApi = {
  list: () => apiFetch<AdminSystemSetting[]>('/api/admin/system-settings'),
  update: (key: string, value: string) =>
    apiFetch<AdminSystemSetting>(`/api/admin/system-settings/${key}`, {
      method: 'PATCH',
      body: JSON.stringify({ value })
    })
};

// ---------- Audit ----------

export interface AdminAuditLog {
  id: string;
  actorUserId: string | null;
  actorEmail: string | null;
  action: string;
  entityType: string | null;
  entityId: string | null;
  metadataJson: string | null;
  ipAddress: string | null;
  userAgent: string | null;
  createdAt: string;
}

export const adminAuditApi = {
  list: (params: { page?: number; pageSize?: number; action?: string; actorId?: string } = {}) => {
    const q = new URLSearchParams();
    if (params.page) q.set('page', String(params.page));
    if (params.pageSize) q.set('pageSize', String(params.pageSize));
    if (params.action) q.set('action', params.action);
    if (params.actorId) q.set('actorId', params.actorId);
    return apiFetch<{ items: AdminAuditLog[]; total: number; page: number; pageSize: number }>(
      `/api/admin/audit-logs?${q}`
    );
  }
};