'use client';

import Link from 'next/link';
import { usePathname, useRouter } from 'next/navigation';
import { useMemo, useState } from 'react';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { toast } from 'sonner';
import { FileText, Plus, Search,
  MoreHorizontal,
  Pin,
  PinOff,
  Pencil,
  Trash2,
  LogOut,
  Settings,
  CreditCard,
  Menu,
  X
} from 'lucide-react';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import {
  DropdownMenu,
  DropdownMenuTrigger,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuSeparator,
  DropdownMenuLabel
} from '@/components/ui/dropdown-menu';
import { Avatar, AvatarFallback, AvatarImage } from '@/components/ui/avatar';
import { conversationsApi } from '@/lib/api/conversations';
import { authApi } from '@/lib/api/auth';
import { session } from '@/lib/auth/session';
import { useAuthStore } from '@/store/auth-store';
import { groupByDate, type DateGroup } from '@/lib/utils/date';
import { cn } from '@/lib/utils/cn';
import type { ConversationDto } from '@/lib/api/types';

const GROUP_ORDER: DateGroup[] = ['Today', 'Yesterday', 'Previous 7 days', 'Older'];

export function ChatSidebar() {
  const router = useRouter();
  const pathname = usePathname();
  const qc = useQueryClient();
  const { user, logout } = useAuthStore();
  const [open, setOpen] = useState(false);
  const [search, setSearch] = useState('');

  const conversations = useQuery({
    queryKey: ['conversations'],
    queryFn: () => conversationsApi.list({ limit: 100 })
  });

  const createConv = useMutation({
    mutationFn: () => conversationsApi.create({}),
    onSuccess: (conv) => {
      qc.invalidateQueries({ queryKey: ['conversations'] });
      router.push(`/chat/${conv.id}`);
      setOpen(false);
    },
    onError: (e) => toast.error(e instanceof Error ? e.message : 'Could not create conversation')
  });

  const updateConv = useMutation({
    mutationFn: (args: { id: string; body: { title?: string; isPinned?: boolean; isArchived?: boolean } }) =>
      conversationsApi.update(args.id, args.body),
    onSuccess: () => qc.invalidateQueries({ queryKey: ['conversations'] })
  });

  const deleteConv = useMutation({
    mutationFn: (id: string) => conversationsApi.delete(id),
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: ['conversations'] });
      toast.success('Conversation deleted');
    }
  });

  async function signOut() {
    const rt = session.getRefresh();
    if (rt) try { await authApi.logout(rt); } catch { /* ignore */ }
    logout();
    router.push('/login');
  }

  const items = conversations.data?.items ?? [];
  const grouped = useMemo(() => {
    const map: Record<DateGroup, ConversationDto[]> = {
      Today: [], Yesterday: [], 'Previous 7 days': [], Older: []
    };
    for (const c of items) map[groupByDate(c.updatedAt)].push(c);
    return map;
  }, [items]);

  const filtered = useMemo(() => {
    if (!search.trim()) return grouped;
    const q = search.trim().toLowerCase();
    const out: Record<DateGroup, ConversationDto[]> = {
      Today: [], Yesterday: [], 'Previous 7 days': [], Older: []
    };
    for (const key of GROUP_ORDER) {
      out[key] = grouped[key].filter((c) => c.title.toLowerCase().includes(q));
    }
    return out;
  }, [grouped, search]);

  const initials =
    user?.displayName?.slice(0, 1).toUpperCase() || user?.email.slice(0, 1).toUpperCase() || 'C';

  return (
    <>
      {/* Mobile top bar */}
      <div className="md:hidden fixed top-0 inset-x-0 z-30 h-14 border-b bg-background/95 backdrop-blur flex items-center justify-between px-4">
        <button onClick={() => setOpen(true)} className="p-2 -ml-2">
          <Menu className="h-5 w-5" />
        </button>
        <Link href="/chat" className="flex items-center gap-2">
          <img src="/logo.svg" alt="" className="h-6 w-6" />
          <span className="font-semibold">Chima</span>
        </Link>
        <div className="w-9" />
      </div>

      {/* Sidebar */}
      <aside
        className={cn(
          'fixed md:static z-40 inset-y-0 left-0 w-72 border-r border-border/60 bg-muted/30 flex flex-col',
          'transition-transform md:translate-x-0',
          open ? 'translate-x-0' : '-translate-x-full'
        )}
      >
        <div className="p-3 flex items-center justify-between border-b border-border/40">
          <Link href="/chat" className="flex items-center gap-2 font-semibold">
            <img src="/logo.svg" alt="" className="h-6 w-6" />
            <span>Chima</span>
          </Link>
          <button onClick={() => setOpen(false)} className="md:hidden p-2 -mr-2">
            <X className="h-5 w-5" />
          </button>
        </div>

        <div className="p-3 space-y-2">
          <Button className="w-full" onClick={() => createConv.mutate()} disabled={createConv.isPending}>
            <Plus className="mr-2 h-4 w-4" />
            New chat
          </Button>

          <div className="relative">
            <Search className="absolute left-3 top-1/2 -translate-y-1/2 h-3.5 w-3.5 text-muted-foreground" />
            <Input
              placeholder="Search conversations"
              value={search}
              onChange={(e) => setSearch(e.target.value)}
              className="pl-9 h-9 text-sm"
            />
          </div>
        </div>

        <div className="flex-1 overflow-y-auto px-2 pb-2">
          {conversations.isLoading && (
            <div className="p-3 text-sm text-muted-foreground">Loadingâ€¦</div>
          )}

          {!conversations.isLoading && items.length === 0 && (
            <div className="p-3 text-sm text-muted-foreground">
              No conversations yet. Start a new chat.
            </div>
          )}

          {GROUP_ORDER.map((group) => {
            const groupItems = filtered[group];
            if (!groupItems.length) return null;
            return (
              <div key={group} className="mb-3">
                <div className="px-3 py-2 text-xs font-medium uppercase tracking-wider text-muted-foreground">
                  {group}
                </div>
                <div className="space-y-0.5">
                  {groupItems.map((c) => {
                    const active = pathname === `/chat/${c.id}`;
                    return (
                      <div
                        key={c.id}
                        className={cn(
                          'group flex items-center gap-2 rounded-lg px-2 py-2 text-sm cursor-pointer',
                          active ? 'bg-accent text-foreground' : 'hover:bg-accent/60'
                        )}
                        onClick={() => {
                          router.push(`/chat/${c.id}`);
                          setOpen(false);
                        }}
                      >
                        {c.isPinned && <Pin className="h-3 w-3 shrink-0 text-primary" />}
                        <span className="flex-1 truncate">{c.title}</span>

                        <DropdownMenu>
                          <DropdownMenuTrigger asChild>
                            <button
                              onClick={(e) => e.stopPropagation()}
                              className="opacity-0 group-hover:opacity-100 p-1 rounded hover:bg-background"
                            >
                              <MoreHorizontal className="h-4 w-4 text-muted-foreground" />
                            </button>
                          </DropdownMenuTrigger>
                          <DropdownMenuContent align="end" onClick={(e) => e.stopPropagation()}>
                            <DropdownMenuItem
                              onClick={() => {
                                const title = window.prompt('Rename conversation', c.title);
                                if (title && title.trim()) {
                                  updateConv.mutate({ id: c.id, body: { title: title.trim() } });
                                }
                              }}
                            >
                              <Pencil className="mr-2 h-4 w-4" /> Rename
                            </DropdownMenuItem>
                            <DropdownMenuItem
                              onClick={() =>
                                updateConv.mutate({ id: c.id, body: { isPinned: !c.isPinned } })
                              }
                            >
                              {c.isPinned ? (
                                <>
                                  <PinOff className="mr-2 h-4 w-4" /> Unpin
                                </>
                              ) : (
                                <>
                                  <Pin className="mr-2 h-4 w-4" /> Pin
                                </>
                              )}
                            </DropdownMenuItem>
                            <DropdownMenuSeparator />
                            <DropdownMenuItem
                              className="text-destructive focus:text-destructive"
                              onClick={() => {
                                if (window.confirm('Delete this conversation?')) {
                                  deleteConv.mutate(c.id);
                                  if (active) router.push('/chat');
                                }
                              }}
                            >
                              <Trash2 className="mr-2 h-4 w-4" /> Delete
                            </DropdownMenuItem>
                          </DropdownMenuContent>
                        </DropdownMenu>
                      </div>
                    );
                  })}
                </div>
              </div>
            );
          })}
        </div>

        <div className="p-3 border-t border-border/40">
          <DropdownMenu>
            <DropdownMenuTrigger asChild>
              <button className="w-full flex items-center gap-3 p-2 rounded-lg hover:bg-accent text-left">
                <Avatar>
                  {user?.avatarUrl && <AvatarImage src={user.avatarUrl} alt="" />}
                  <AvatarFallback>{initials}</AvatarFallback>
                </Avatar>
                <div className="flex-1 min-w-0">
                  <div className="text-sm truncate">{user?.displayName ?? 'Account'}</div>
                  <div className="text-xs text-muted-foreground truncate">{user?.email}</div>
                </div>
              </button>
            </DropdownMenuTrigger>
            <DropdownMenuContent align="end" className="w-56">
              <DropdownMenuLabel>{user?.email}</DropdownMenuLabel>
              <DropdownMenuSeparator />
              <DropdownMenuItem asChild>
                <Link href="/settings">
                  <Settings className="mr-2 h-4 w-4" /> Settings
                </Link>
              </DropdownMenuItem>
                            <DropdownMenuItem asChild>
                <Link href="/files">
                  <FileText className="mr-2 h-4 w-4" /> Files
                </Link>
              </DropdownMenuItem>
<DropdownMenuItem asChild>
                <Link href="/subscription">
                  <CreditCard className="mr-2 h-4 w-4" /> Subscription
                </Link>
              </DropdownMenuItem>
              <DropdownMenuSeparator />
              <DropdownMenuItem onClick={signOut}>
                <LogOut className="mr-2 h-4 w-4" /> Sign out
              </DropdownMenuItem>
            </DropdownMenuContent>
          </DropdownMenu>
        </div>
      </aside>

      {/* Mobile backdrop */}
      {open && (
        <div
          className="md:hidden fixed inset-0 z-30 bg-black/40"
          onClick={() => setOpen(false)}
        />
      )}
    </>
  );
}