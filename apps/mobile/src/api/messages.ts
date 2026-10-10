import { apiFetch, API_BASE } from './client';
import { session } from '@/auth/session';
import type { MessageDto, PagedResult } from './types';

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

  regenerate: (messageId: string, modelId?: string) =>
    apiFetch<MessageDto>(`/api/messages/${messageId}/regenerate`, {
      method: 'POST',
      body: JSON.stringify({ modelId })
    }),

  streamUrl: (conversationId: string) =>
    `${API_BASE}/api/conversations/${conversationId}/messages/stream`,

  getAccessToken: () => session.getAccess()
};