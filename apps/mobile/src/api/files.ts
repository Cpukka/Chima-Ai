import { API_BASE, apiFetch } from './client';
import { session } from '@/auth/session';

export interface UploadedFile {
  id: string;
  originalFileName: string;
  contentType: string;
  sizeBytes: number;
  processingStatus: 'pending' | 'extracting' | 'chunking' | 'embedding' | 'ready' | 'failed';
  processingError: string | null;
  pageCount: number | null;
  chunkCount: number | null;
  extractedCharCount: number | null;
  createdAt: string;
  processedAt: string | null;
}

export const filesApi = {
  list: () => apiFetch<UploadedFile[]>('/api/files'),

  get: (id: string) => apiFetch<UploadedFile>(`/api/files/${id}`),

  delete: (id: string) =>
    apiFetch<Record<string, never>>(`/api/files/${id}`, { method: 'DELETE' }),

  upload: async (uri: string, name: string, mimeType: string): Promise<UploadedFile> => {
    const token = session.getAccess();
    const form = new FormData();
    form.append('file', {
      uri,
      name,
      type: mimeType
    } as unknown as Blob);

    const res = await fetch(`${API_BASE}/api/files`, {
      method: 'POST',
      headers: token ? { Authorization: `Bearer ${token}` } : {},
      body: form
    });

    const json = await res.json();
    if (!res.ok || !json.success) throw new Error(json?.message ?? `Upload failed (${res.status})`);
    return json.data as UploadedFile;
  }
};

export function formatBytes(bytes: number): string {
  if (bytes < 1024) return `${bytes} B`;
  if (bytes < 1024 * 1024) return `${(bytes / 1024).toFixed(1)} KB`;
  return `${(bytes / (1024 * 1024)).toFixed(1)} MB`;
}