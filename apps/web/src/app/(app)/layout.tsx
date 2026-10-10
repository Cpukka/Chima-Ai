'use client';

import { useRequireAuth } from '@/hooks/useRequireAuth';
import { ChatSidebar } from '@/components/chat/ChatSidebar';

export default function AppLayout({ children }: { children: React.ReactNode }) {
  const { loading } = useRequireAuth();

  if (loading) {
    return (
      <div className="min-h-screen flex items-center justify-center">
        <div className="animate-pulse text-sm text-muted-foreground">Loading…</div>
      </div>
    );
  }

  return (
    <div className="flex h-screen overflow-hidden bg-background">
      <ChatSidebar />
      <main className="flex-1 min-w-0 flex flex-col">{children}</main>
    </div>
  );
}