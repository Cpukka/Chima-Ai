'use client';

import { useState } from 'react';
import { useQuery, useQueryClient } from '@tanstack/react-query';
import { toast } from 'sonner';
import { Loader2, Mail, ShieldCheck, ShieldOff, UserX, UserCheck } from 'lucide-react';
import { Sheet, SheetContent, SheetHeader, SheetTitle } from '@/components/ui/sheet';
import { Badge } from '@/components/ui/badge';
import { Button } from '@/components/ui/button';
import {
  Select,
  SelectTrigger,
  SelectValue,
  SelectContent,
  SelectItem
} from '@/components/ui/select';
import { adminApi, adminPlansApi } from '@/lib/api/admin';

interface Props {
  userId: string | null;
  onClose: () => void;
}

export function UserDetailDrawer({ userId, onClose }: Props) {
  const qc = useQueryClient();
  const [busy, setBusy] = useState(false);

  const detail = useQuery({
    queryKey: ['admin', 'user', userId],
    queryFn: () => adminApi.user(userId!),
    enabled: Boolean(userId)
  });

  const plans = useQuery({
    queryKey: ['admin', 'plans'],
    queryFn: adminPlansApi.list,
    enabled: Boolean(userId)
  });

  const u = detail.data;

  async function suspend() {
    if (!u) return;
    const reason = window.prompt('Reason for suspension?') ?? 'Administrative action';
    setBusy(true);
    try {
      await adminApi.suspendUser(u.id, reason);
      toast.success('User suspended');
      qc.invalidateQueries({ queryKey: ['admin', 'user', u.id] });
      qc.invalidateQueries({ queryKey: ['admin', 'users'] });
    } catch (e) {
      toast.error(e instanceof Error ? e.message : 'Failed');
    } finally {
      setBusy(false);
    }
  }

  async function unsuspend() {
    if (!u) return;
    setBusy(true);
    try {
      await adminApi.unsuspendUser(u.id);
      toast.success('User reactivated');
      qc.invalidateQueries({ queryKey: ['admin', 'user', u.id] });
      qc.invalidateQueries({ queryKey: ['admin', 'users'] });
    } catch (e) {
      toast.error(e instanceof Error ? e.message : 'Failed');
    } finally {
      setBusy(false);
    }
  }

  async function changePlan(planCode: string) {
    if (!u) return;
    setBusy(true);
    try {
      await adminApi.changeUserPlan(u.id, planCode, 'Admin plan change');
      toast.success(`Switched to ${planCode}`);
      qc.invalidateQueries({ queryKey: ['admin', 'user', u.id] });
      qc.invalidateQueries({ queryKey: ['admin', 'users'] });
      qc.invalidateQueries({ queryKey: ['admin', 'dashboard'] });
    } catch (e) {
      toast.error(e instanceof Error ? e.message : 'Failed');
    } finally {
      setBusy(false);
    }
  }

  return (
    <Sheet open={Boolean(userId)} onOpenChange={(open) => !open && onClose()}>
      <SheetContent className="w-full sm:max-w-xl">
        <SheetHeader>
          <SheetTitle>{u?.displayName ?? 'User detail'}</SheetTitle>
        </SheetHeader>

        {detail.isLoading && (
          <div className="flex items-center justify-center py-20">
            <Loader2 className="h-6 w-6 animate-spin text-muted-foreground" />
          </div>
        )}

        {u && (
          <div className="mt-6 space-y-6">
            {/* Header */}
            <div className="flex items-center gap-3">
              <div className="h-12 w-12 rounded-full bg-primary/10 text-primary flex items-center justify-center text-lg font-semibold">
                {(u.displayName ?? u.email).slice(0, 1).toUpperCase()}
              </div>
              <div className="min-w-0 flex-1">
                <div className="font-semibold truncate">{u.displayName ?? '—'}</div>
                <div className="text-sm text-muted-foreground truncate">{u.email}</div>
              </div>
              {u.status === 'active' ? (
                <Badge variant="success">Active</Badge>
              ) : u.status === 'suspended' ? (
                <Badge variant="danger">Suspended</Badge>
              ) : (
                <Badge variant="muted">{u.status}</Badge>
              )}
            </div>

            {/* Quick facts */}
            <div className="grid grid-cols-2 gap-3 text-sm">
              <Fact label="Plan" value={u.planName} />
              <Fact label="Email verified" value={u.emailVerified ? 'Yes' : 'No'} />
              <Fact label="Joined" value={new Date(u.createdAt).toLocaleDateString()} />
              <Fact
                label="Last login"
                value={u.lastLoginAt ? new Date(u.lastLoginAt).toLocaleDateString() : 'Never'}
              />
              {u.isAdmin && <Fact label="Admin role" value={u.adminRole ?? 'Admin'} />}
            </div>

            {/* Usage */}
            <div className="rounded-lg border p-4 space-y-3">
              <div className="text-sm font-medium">This month</div>
              <UsageBar label="Messages" used={u.messagesUsedThisMonth} limit={u.messagesLimit} />
              <UsageBar
                label="File uploads"
                used={u.fileUploadsUsedThisMonth}
                limit={u.fileUploadsLimit}
              />
            </div>

            {/* Content counts */}
            <div className="grid grid-cols-3 gap-3 text-sm">
              <Fact label="Conversations" value={u.conversationCount.toLocaleString()} />
              <Fact label="Messages" value={u.messageCount.toLocaleString()} />
              <Fact label="Files" value={u.fileCount.toLocaleString()} />
            </div>

            {/* Actions */}
            <div className="space-y-3">
              <div className="text-sm font-medium">Plan</div>
              <Select
                value={u.planCode}
                onValueChange={changePlan}
                disabled={busy}
              >
                <SelectTrigger>
                  <SelectValue />
                </SelectTrigger>
                <SelectContent>
                  {(plans.data ?? []).map((p) => (
                    <SelectItem key={p.id} value={p.code}>
                      {p.displayName} — ${p.monthlyPrice}/mo
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
            </div>

            {/* Suspend/Unsuspend */}
            <div className="pt-2 border-t">
              {u.status === 'suspended' ? (
                <Button onClick={unsuspend} disabled={busy} variant="outline" className="w-full">
                  <UserCheck className="mr-2 h-4 w-4" /> Reactivate user
                </Button>
              ) : (
                <Button
                  onClick={suspend}
                  disabled={busy}
                  variant="outline"
                  className="w-full text-destructive hover:text-destructive"
                >
                  <UserX className="mr-2 h-4 w-4" /> Suspend user
                </Button>
              )}
            </div>
          </div>
        )}
      </SheetContent>
    </Sheet>
  );
}

function Fact({ label, value }: { label: string; value: string }) {
  return (
    <div>
      <div className="text-xs uppercase tracking-wider text-muted-foreground">{label}</div>
      <div className="mt-0.5">{value}</div>
    </div>
  );
}

function UsageBar({ label, used, limit }: { label: string; used: number; limit: number }) {
  const unlimited = limit < 0;
  const pct = unlimited ? 0 : Math.min(100, (used / Math.max(1, limit)) * 100);
  const tone = pct >= 90 ? 'bg-destructive' : pct >= 70 ? 'bg-amber-500' : 'bg-primary';

  return (
    <div>
      <div className="flex justify-between text-xs mb-1">
        <span className="text-muted-foreground">{label}</span>
        <span className="tabular-nums">
          {used.toLocaleString()} / {unlimited ? '∞' : limit.toLocaleString()}
        </span>
      </div>
      <div className="h-1.5 rounded-full bg-muted overflow-hidden">
        <div
          className={`h-full ${tone}`}
          style={{ width: `${unlimited ? 5 : Math.max(2, pct)}%` }}
        />
      </div>
    </div>
  );
}