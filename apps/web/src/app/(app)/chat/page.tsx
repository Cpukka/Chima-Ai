'use client';

import { useRouter } from 'next/navigation';
import { EmptyState } from '@/components/chat/EmptyState';
import { conversationsApi } from '@/lib/api/conversations';
import { toast } from 'sonner';

export default function NewChatPage() {
  const router = useRouter();

  async function start(prompt?: string) {
    try {
      const conv = await conversationsApi.create({});
      // If a prompt was provided, pass it via URL and let the conversation page send it
      if (prompt) {
        router.push(`/chat/${conv.id}?prompt=${encodeURIComponent(prompt)}`);
      } else {
        router.push(`/chat/${conv.id}`);
      }
    } catch (e) {
      toast.error(e instanceof Error ? e.message : 'Could not start a chat');
    }
  }

  return (
    <div className="flex flex-col h-full pt-14 md:pt-0">
      <EmptyState onPrompt={(p) => start(p)} />
    </div>
  );
}