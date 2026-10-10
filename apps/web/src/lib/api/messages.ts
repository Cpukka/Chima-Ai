import { apiFetch } from './client';
import { session } from '@/lib/auth/session';
import type { MessageDto, PagedResult } from './types';

const BASE = process.env.NEXT_PUBLIC_API_BASE_URL || 'http://localhost:5080';

export const messagesApi = {
  list: (conversationId: string, params: { cursor?: string; limit?: number; ascending?: boolean } = {}) => {
    const q = new URLSearchParams();
    if (params.cursor) q.set('cursor', params.cursor);
    if (params.limit) q.set('limit', String(params.limit));
    if (params.ascending !== undefined) q.set('ascending', String(params.ascending));
    return apiFetch<PagedResult<MessageDto>>(`/api/conversations/${conversationId}/messages?${q}`);
  },

  send: (conversationId: string, content: string, modelId?: string) =>
    apiFetch<{ userMessage: MessageDto; assistantPlaceholder: MessageDto }>(
      `/api/conversations/${conversationId}/messages`,
      { method: 'POST', body: JSON.stringify({ content, modelId }) }
    ),

  edit: (messageId: string, content: string) =>
    apiFetch<MessageDto>(`/api/messages/${messageId}`, {
      method: 'PATCH',
      body: JSON.stringify({ content })
    }),

  delete: (messageId: string) =>
    apiFetch<Record<string, never>>(`/api/messages/${messageId}`, { method: 'DELETE' }),

  regenerate: (messageId: string, modelId?: string) =>
    apiFetch<MessageDto>(`/api/messages/${messageId}/regenerate`, {
      method: 'POST',
      body: JSON.stringify({ modelId })
    }),

  // ---- SSE streaming ----
  streamUrl: (conversationId: string) =>
    `${BASE}/api/conversations/${conversationId}/messages/stream`,

  getAccessToken: () => session.getAccess()
};