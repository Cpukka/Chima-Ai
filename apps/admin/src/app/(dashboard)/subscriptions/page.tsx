'use client';

import { useState } from 'react';
import { useQuery } from '@tanstack/react-query';
import { ChevronLeft, ChevronRight, DollarSign, Users, TrendingDown, TrendingUp } from 'lucide-react';
import { Badge } from '@/components/ui/badge';
import { Button } from '@/components/ui/button';
import {
  Select,
  SelectTrigger,
  SelectValue,
  SelectContent,
  SelectItem
} from '@/components/ui/select';
import { Card, CardContent } from '@/components/ui/card';
import { adminSubscriptionsApi, adminApi } from '@/lib/api/admin';

const PAGE_SIZE = 25;

export default function SubscriptionsPage() {
  const [page, setPage] = useState(1);
  const [status, setStatus] = useState<string>('');
  const [planCode, setPlanCode] = useState<string>('');

  const subs = useQuery({
    queryKey: ['admin', 'subscriptions', page, status, planCode],
    queryFn: () =>
      adminSubscriptionsApi.list({
        page,
        pageSize: PAGE_SIZE,
        status: status || undefined,
        planCode: planCode || undefined
      })
  });

  const revenue = useQuery({
    queryKey: ['admin', 'revenue', 30],
    queryFn: () => adminApi.revenue(30)
  });

  const data = subs.data;
  const r = revenue.data;
  const totalPages = data?.totalPages ?? 1;

  return (
    <div className="space-y-5">
      <div>
        <h1 className="text-2xl font-semibold tracking-tight">Subscriptions</h1>
        <p className="text-sm text-muted-foreground mt-1">
          Recurring revenue and subscription lifecycle
        </p>
      </div>

      {/* Revenue stats */}
      {r && (
        <div className="grid gap-4 sm:grid-cols-2 lg:grid-cols-4">
          <Stat icon={DollarSign} label="MRR" value={`$${r.mrr.toFixed(2)}`} />
          <Stat icon={TrendingUp} label="ARR" value={`$${r.arr.toFixed(2)}`} />
          <Stat icon={Users} label="Active subs" value={r.activeSubscriptions.toString()} />
          <Stat
            icon={TrendingDown}
            label="Churn (30d)"
            value={`${r.churnPercent.toFixed(1)}%`}
            tone={r.churnPercent > 5 ? 'warn' : 'ok'}
          />
        </div>
      )}

      {/* Plan breakdown */}
      {r && r.byPlan.length > 0 && (
        <div className="rounded-xl border bg-card">
          <div className="px-4 py-3 border-b text-sm font-medium">Revenue by plan</div>
          <div className="divide-y">
            {r.byPlan.map((p) => (
              <div key={p.planCode} className="flex items-center justify-between px-4 py-3 text-sm">
                <div className="font-medium">{p.planName}</div>
                <div className="flex items-center gap-6 text-muted-foreground">
                  <span>{p.subscribers} subs</span>
                  <span className="tabular-nums font-medium text-foreground">
                    ${p.mrr.toFixed(2)} / mo
                  </span>
                </div>
              </div>
            ))}
          </div>
        </div>
      )}

      {/* Filters */}
      <div className="flex gap-3">
        <Select
          value={status || 'all'}
          onValueChange={(v) => {
            setStatus(v === 'all' ? '' : v);
            setPage(1);
          }}
        >
          <SelectTrigger className="w-[180px]">
            <SelectValue placeholder="Status" />
          </SelectTrigger>
          <SelectContent>
            <SelectItem value="all">All statuses</SelectItem>
            <SelectItem value="active">Active</SelectItem>
            <SelectItem value="cancelled">Cancelled</SelectItem>
            <SelectItem value="expired">Expired</SelectItem>
            <SelectItem value="pastdue">Past due</SelectItem>
          </SelectContent>
        </Select>

        <Select
          value={planCode || 'all'}
          onValueChange={(v) => {
            setPlanCode(v === 'all' ? '' : v);
            setPage(1);
          }}
        >
          <SelectTrigger className="w-[180px]">
            <SelectValue placeholder="Plan" />
          </SelectTrigger>
          <SelectContent>
            <SelectItem value="all">All plans</SelectItem>
            <SelectItem value="free">Free</SelectItem>
            <SelectItem value="basic">Basic</SelectItem>
            <SelectItem value="pro">Pro</SelectItem>
            <SelectItem value="premium">Premium</SelectItem>
          </SelectContent>
        </Select>
      </div>

      {/* Table */}
      <div className="rounded-xl border bg-card overflow-hidden">
        <table className="w-full text-sm">
          <thead className="bg-muted/50 text-muted-foreground text-xs uppercase tracking-wider">
            <tr>
              <th className="text-left font-medium px-4 py-3">User</th>
              <th className="text-left font-medium px-4 py-3">Plan</th>
              <th className="text-left font-medium px-4 py-3">Status</th>
              <th className="text-left font-medium px-4 py-3">Period</th>
              <th className="text-right font-medium px-4 py-3">Price</th>
              <th className="text-left font-medium px-4 py-3">Provider</th>
            </tr>
          </thead>
          <tbody>
            {subs.isLoading && (
              <tr>
                <td colSpan={6} className="p-8 text-center text-muted-foreground">
                  Loading…
                </td>
              </tr>
            )}

            {!subs.isLoading && (data?.items ?? []).length === 0 && (
              <tr>
                <td colSpan={6} className="p-8 text-center text-muted-foreground">
                  No subscriptions match these filters.
                </td>
              </tr>
            )}

            {(data?.items ?? []).map((s) => (
              <tr key={s.id} className="border-t border-border/40">
                <td className="px-4 py-3">
                  <div className="font-medium truncate">{s.userDisplayName ?? '—'}</div>
                  <div className="text-xs text-muted-foreground truncate">{s.userEmail}</div>
                </td>
                <td className="px-4 py-3">
                  <Badge variant={s.planCode === 'free' ? 'muted' : 'default'}>
                    {s.planName}
                  </Badge>
                </td>
                <td className="px-4 py-3">
                  <StatusBadge status={s.status} />
                </td>
                <td className="px-4 py-3 text-xs text-muted-foreground">
                  {new Date(s.currentPeriodStart).toLocaleDateString()} →{' '}
                  {new Date(s.currentPeriodEnd).toLocaleDateString()}
                </td>
                <td className="px-4 py-3 text-right tabular-nums">
                  ${s.monthlyPrice.toFixed(2)}
                </td>
                <td className="px-4 py-3 text-xs text-muted-foreground">{s.provider}</td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>

      {/* Pagination */}
      <div className="flex items-center justify-between">
        <div className="text-sm text-muted-foreground">
          Page {page} of {totalPages}
        </div>
        <div className="flex gap-2">
          <Button
            variant="outline"
            size="sm"
            disabled={page === 1}
            onClick={() => setPage((p) => Math.max(1, p - 1))}
          >
            <ChevronLeft className="h-4 w-4 mr-1" /> Prev
          </Button>
          <Button
            variant="outline"
            size="sm"
            disabled={page >= totalPages}
            onClick={() => setPage((p) => p + 1)}
          >
            Next <ChevronRight className="h-4 w-4 ml-1" />
          </Button>
        </div>
      </div>
    </div>
  );
}

function StatusBadge({ status }: { status: string }) {
  if (status === 'active') return <Badge variant="success">Active</Badge>;
  if (status === 'cancelled') return <Badge variant="warning">Cancelled</Badge>;
  if (status === 'pastdue') return <Badge variant="danger">Past due</Badge>;
  if (status === 'expired') return <Badge variant="muted">Expired</Badge>;
  return <Badge variant="muted">{status}</Badge>;
}

function Stat({
  icon: Icon,
  label,
  value,
  tone = 'ok'
}: {
  icon: React.ComponentType<{ className?: string }>;
  label: string;
  value: string;
  tone?: 'ok' | 'warn';
}) {
  return (
    <Card>
      <CardContent className="p-5">
        <div className="flex items-start justify-between">
          <div>
            <div className="text-xs uppercase tracking-wider text-muted-foreground">{label}</div>
            <div className="text-2xl font-semibold mt-1 tabular-nums">{value}</div>
          </div>
          <div
            className={`h-9 w-9 rounded-lg flex items-center justify-center shrink-0 ${
              tone === 'warn' ? 'bg-amber-500/10 text-amber-600' : 'bg-primary/10 text-primary'
            }`}
          >
            <Icon className="h-4 w-4" />
          </div>
        </div>
      </CardContent>
    </Card>
  );
}