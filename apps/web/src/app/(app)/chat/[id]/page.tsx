'use client';

import { useParams, useSearchParams, useRouter } from 'next/navigation';
import { useCallback, useEffect, useMemo, useRef, useState } from 'react';
import { useQuery, useQueryClient } from '@tanstack/react-query';
import { toast } from 'sonner';
import { conversationsApi } from '@/lib/api/conversations';
import { messagesApi } from '@/lib/api/messages';
import { session } from '@/lib/auth/session';
import { streamMessage } from '@/lib/sse/stream';
import { useAuthStore } from '@/store/auth-store';
import { MessageBubble } from '@/components/chat/MessageBubble';
import { Composer } from '@/components/chat/Composer';
import { EmptyState } from '@/components/chat/EmptyState';
import { MoreHorizontal, Pin, Pencil, Trash2 } from 'lucide-react';
import {
  DropdownMenu,
  DropdownMenuTrigger,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuSeparator
} from '@/components/ui/dropdown-menu';
import type { MessageDto } from '@/lib/api/types';

export default function ConversationPage() {
  const { id } = useParams<{ id: string }>();
  const router = useRouter();
  const search = useSearchParams();
  const qc = useQueryClient();
  const { user } = useAuthStore();
  const [streaming, setStreaming] = useState(false);
  const [liveAssistantId, setLiveAssistantId] = useState<string | null>(null);
  const [liveContent, setLiveContent] = useState('');
  const abortRef = useRef<AbortController | null>(null);
  const bottomRef = useRef<HTMLDivElement>(null);
  const sentInitialPrompt = useRef(false);

  const conversation = useQuery({
    queryKey: ['conversation', id],
    queryFn: () => conversationsApi.get(id),
    enabled: Boolean(id)
  });

  const messages = useQuery({
    queryKey: ['messages', id],
    queryFn: () => messagesApi.list(id, { limit: 100, ascending: true }),
    enabled: Boolean(id)
  });

  // Auto-scroll as content arrives
  useEffect(() => {
    bottomRef.current?.scrollIntoView({ behavior: 'smooth', block: 'end' });
  }, [messages.data, liveContent]);

  // Handle ?prompt= URL param — send on first load
  const initialPrompt = search.get('prompt');
  useEffect(() => {
    if (!initialPrompt || sentInitialPrompt.current) return;
    if (messages.isLoading) return;
    sentInitialPrompt.current = true;
    router.replace(`/chat/${id}`);
    void send(initialPrompt);
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [initialPrompt, messages.isLoading, id]);

  const send = useCallback(
    async (content: string) => {
      if (!content.trim() || streaming) return;
      const token = session.getAccess();
      if (!token) {
        router.replace('/login');
        return;
      }

      setStreaming(true);
      setLiveContent('');
      setLiveAssistantId(null);

      // Optimistic user message
      const optimisticUser: MessageDto = {
        id: `optimistic-${Date.now()}`,
        conversationId: id,
        role: 'user',
        status: 'completed',
        content,
        parentMessageId: null,
        branchRootId: null,
        modelId: null,
        inputTokens: null,
        outputTokens: null,
        estimatedCost: null,
        durationMs: null,
        errorCode: null,
        errorMessage: null,
        createdAt: new Date().toISOString(),
        updatedAt: new Date().toISOString()
      };
      qc.setQueryData<{ items: MessageDto[]; nextCursor: string | null; hasMore: boolean }>(
        ['messages', id],
        (prev) => {
          if (!prev) return { items: [optimisticUser], nextCursor: null, hasMore: false };
          return { ...prev, items: [...prev.items, optimisticUser] };
        }
      );

      const controller = streamMessage(
        messagesApi.streamUrl(id),
        { content },
        token,
        {
          onMeta: (m) => setLiveAssistantId(m.assistantMessageId),
          onDelta: (d) => setLiveContent((v) => v + d.content),
          onDone: () => {
            setStreaming(false);
            setLiveContent('');
            setLiveAssistantId(null);
            qc.invalidateQueries({ queryKey: ['messages', id] });
            qc.invalidateQueries({ queryKey: ['conversations'] });
          },
          onError: (e) => {
            setStreaming(false);
            setLiveContent('');
            setLiveAssistantId(null);
            qc.invalidateQueries({ queryKey: ['messages', id] });
            qc.invalidateQueries({ queryKey: ['conversations'] });
            if (e.code !== 'cancelled') {
              toast.error(e.message || 'AI request failed');
            }
          }
        }
      );
      abortRef.current = controller;
    },
    [id, qc, router, streaming]
  );

  function stop() {
    abortRef.current?.abort();
    setStreaming(false);
  }

  async function regenerate(messageId: string) {
    try {
      toast.info('Regenerating…');
      await messagesApi.regenerate(messageId);
      qc.invalidateQueries({ queryKey: ['messages', id] });
    } catch (e) {
      toast.error(e instanceof Error ? e.message : 'Regenerate failed');
    }
  }

  async function rename() {
    const c = conversation.data;
    if (!c) return;
    const title = window.prompt('Rename conversation', c.title);
    if (!title?.trim()) return;
    await conversationsApi.update(id, { title: title.trim() });
    qc.invalidateQueries({ queryKey: ['conversation', id] });
    qc.invalidateQueries({ queryKey: ['conversations'] });
  }

  async function togglePin() {
    const c = conversation.data;
    if (!c) return;
    await conversationsApi.update(id, { isPinned: !c.isPinned });
    qc.invalidateQueries({ queryKey: ['conversation', id] });
    qc.invalidateQueries({ queryKey: ['conversations'] });
  }

  async function remove() {
    if (!window.confirm('Delete this conversation?')) return;
    await conversationsApi.delete(id);
    qc.invalidateQueries({ queryKey: ['conversations'] });
    router.push('/chat');
  }

  const allMessages = useMemo(() => messages.data?.items ?? [], [messages.data]);
  const isEmpty = allMessages.length === 0 && !streaming;

  return (
    <div className="flex flex-col h-full pt-14 md:pt-0">
      <header className="h-14 border-b border-border/40 flex items-center justify-between px-4 shrink-0">
        <div className="min-w-0">
          <h1 className="truncate text-sm font-medium">
            {conversation.data?.title ?? 'Loading…'}
          </h1>
        </div>
        <DropdownMenu>
          <DropdownMenuTrigger asChild>
            <button className="p-2 rounded hover:bg-accent">
              <MoreHorizontal className="h-4 w-4" />
            </button>
          </DropdownMenuTrigger>
          <DropdownMenuContent align="end">
            <DropdownMenuItem onClick={rename}>
              <Pencil className="mr-2 h-4 w-4" /> Rename
            </DropdownMenuItem>
            <DropdownMenuItem onClick={togglePin}>
              <Pin className="mr-2 h-4 w-4" />
              {conversation.data?.isPinned ? 'Unpin' : 'Pin'}
            </DropdownMenuItem>
            <DropdownMenuSeparator />
            <DropdownMenuItem className="text-destructive focus:text-destructive" onClick={remove}>
              <Trash2 className="mr-2 h-4 w-4" /> Delete
            </DropdownMenuItem>
          </DropdownMenuContent>
        </DropdownMenu>
      </header>

      <div className="flex-1 overflow-y-auto">
        {isEmpty ? (
          <EmptyState onPrompt={send} />
        ) : (
          <div className="py-4">
            {allMessages.map((m) => (
              <MessageBubble key={m.id} message={m} user={user} onRegenerate={regenerate} />
            ))}

            {streaming && (
              <MessageBubble
                message={{
                  id: liveAssistantId ?? 'live',
                  conversationId: id,
                  role: 'assistant',
                  status: liveContent ? 'streaming' : 'pending',
                  content: liveContent,
                  parentMessageId: null,
                  branchRootId: null,
                  modelId: null,
                  inputTokens: null,
                  outputTokens: null,
                  estimatedCost: null,
                  durationMs: null,
                  errorCode: null,
                  errorMessage: null,
                  createdAt: new Date().toISOString(),
                  updatedAt: new Date().toISOString()
                }}
                user={user}
                showActions={false}
              />
            )}

            <div ref={bottomRef} />
          </div>
        )}
      </div>

      <Composer onSend={send} onStop={stop} isStreaming={streaming} />
    </div>
  );
}