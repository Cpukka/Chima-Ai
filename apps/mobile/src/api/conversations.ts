import { apiFetch } from './client';
import type { ConversationDto, PagedResult } from './types';

export const conversationsApi = {
  list: (params: { cursor?: string; limit?: number; includeArchived?: boolean } = {}) => {
    const q = new URLSearchParams();
    if (params.cursor) q.set('cursor', params.cursor);
    if (params.limit) q.set('limit', String(params.limit));
    if (params.includeArchived) q.set('includeArchived', 'true');
    return apiFetch<PagedResult<ConversationDto>>(`/api/conversations?${q}`);
  },

  get: (id: string) => apiFetch<ConversationDto>(`/api/conversations/${id}`),

  create: (body: { title?: string } = {}) =>
    apiFetch<ConversationDto>('/api/conversations', {
      method: 'POST',
      body: JSON.stringify(body)
    }),

  update: (id: string, body: { title?: string; isPinned?: boolean; isArchived?: boolean }) =>
    apiFetch<ConversationDto>(`/api/conversations/${id}`, {
      method: 'PATCH',
      body: JSON.stringify(body)
    }),

  delete: (id: string) =>
    apiFetch<Record<string, never>>(`/api/conversations/${id}`, { method: 'DELETE' })
};