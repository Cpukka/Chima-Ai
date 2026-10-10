'use client';

import { useState } from 'react';
import { useQuery } from '@tanstack/react-query';
import { ChevronLeft, ChevronRight, Download, Search } from 'lucide-react';
import { Badge } from '@/components/ui/badge';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import {
  Select,
  SelectTrigger,
  SelectValue,
  SelectContent,
  SelectItem
} from '@/components/ui/select';
import { adminPaymentsApi } from '@/lib/api/admin';

const PAGE_SIZE = 25;

export default function PaymentsPage() {
  const [page, setPage] = useState(1);
  const [search, setSearch] = useState('');
  const [status, setStatus] = useState<string>('');
  const [provider, setProvider] = useState<string>('');

  const payments = useQuery({
    queryKey: ['admin', 'payments', page, search, status, provider],
    queryFn: () =>
      adminPaymentsApi.list({
        page,
        pageSize: PAGE_SIZE,
        search: search || undefined,
        status: status || undefined,
        provider: provider || undefined
      })
  });

  const data = payments.data;
  const totalPages = data?.totalPages ?? 1;
  const totalAmount = (data?.items ?? [])
    .filter((p) => p.status === 'succeeded')
    .reduce((sum, p) => sum + p.amount, 0);

  function exportCsv() {
    const rows = data?.items ?? [];
    const header = [
      'Date',
      'User',
      'Email',
      'Amount',
      'Currency',
      'Status',
      'Provider',
      'Plan',
      'ExternalId'
    ];
    const lines = [header.join(',')];
    for (const p of rows) {
      lines.push(
        [
          new Date(p.createdAt).toISOString(),
          `"${(p.userDisplayName ?? '').replace(/"/g, '""')}"`,
          p.userEmail,
          p.amount.toFixed(2),
          p.currency,
          p.status,
          p.provider,
          p.subscriptionPlanCode ?? '',
          p.externalPaymentId ?? ''
        ].join(',')
      );
    }
    const blob = new Blob([lines.join('\n')], { type: 'text/csv' });
    const url = URL.createObjectURL(blob);
    const a = document.createElement('a');
    a.href = url;
    a.download = `payments-${new Date().toISOString().slice(0, 10)}.csv`;
    a.click();
    URL.revokeObjectURL(url);
  }

  return (
    <div className="space-y-5">
      <div className="flex items-end justify-between flex-wrap gap-3">
        <div>
          <h1 className="text-2xl font-semibold tracking-tight">Payments</h1>
          <p className="text-sm text-muted-foreground mt-1">
            {data?.total ?? 0} total · ${totalAmount.toFixed(2)} on this page
          </p>
        </div>
        <Button variant="outline" size="sm" onClick={exportCsv} disabled={!data?.items?.length}>
          <Download className="h-4 w-4 mr-2" /> Export CSV
        </Button>
      </div>

      <div className="flex gap-3 flex-wrap items-center">
        <div className="relative flex-1 min-w-[240px]">
          <Search className="absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-muted-foreground" />
          <Input
            placeholder="Search by user or external id"
            value={search}
            onChange={(e) => {
              setSearch(e.target.value);
              setPage(1);
            }}
            className="pl-9"
          />
        </div>

        <Select
          value={status || 'all'}
          onValueChange={(v) => {
            setStatus(v === 'all' ? '' : v);
            setPage(1);
          }}
        >
          <SelectTrigger className="w-[160px]">
            <SelectValue placeholder="Status" />
          </SelectTrigger>
          <SelectContent>
            <SelectItem value="all">All statuses</SelectItem>
            <SelectItem value="succeeded">Succeeded</SelectItem>
            <SelectItem value="pending">Pending</SelectItem>
            <SelectItem value="failed">Failed</SelectItem>
            <SelectItem value="refunded">Refunded</SelectItem>
          </SelectContent>
        </Select>

        <Select
          value={provider || 'all'}
          onValueChange={(v) => {
            setProvider(v === 'all' ? '' : v);
            setPage(1);
          }}
        >
          <SelectTrigger className="w-[160px]">
            <SelectValue placeholder="Provider" />
          </SelectTrigger>
          <SelectContent>
            <SelectItem value="all">All providers</SelectItem>
            <SelectItem value="stripe">Stripe</SelectItem>
            <SelectItem value="googleplay">Google Play</SelectItem>
            <SelectItem value="paystack">Paystack</SelectItem>
            <SelectItem value="manual">Manual</SelectItem>
          </SelectContent>
        </Select>
      </div>

      <div className="rounded-xl border bg-card overflow-hidden">
        <table className="w-full text-sm">
          <thead className="bg-muted/50 text-muted-foreground text-xs uppercase tracking-wider">
            <tr>
              <th className="text-left font-medium px-4 py-3">Date</th>
              <th className="text-left font-medium px-4 py-3">User</th>
              <th className="text-left font-medium px-4 py-3">Plan</th>
              <th className="text-right font-medium px-4 py-3">Amount</th>
              <th className="text-left font-medium px-4 py-3">Status</th>
              <th className="text-left font-medium px-4 py-3">Provider</th>
              <th className="text-left font-medium px-4 py-3">External ID</th>
            </tr>
          </thead>
          <tbody>
            {payments.isLoading && (
              <tr>
                <td colSpan={7} className="p-8 text-center text-muted-foreground">
                  Loading…
                </td>
              </tr>
            )}

            {!payments.isLoading && (data?.items ?? []).length === 0 && (
              <tr>
                <td colSpan={7} className="p-8 text-center text-muted-foreground">
                  No payments match these filters.
                </td>
              </tr>
            )}

            {(data?.items ?? []).map((p) => (
              <tr key={p.id} className="border-t border-border/40">
                <td className="px-4 py-3 text-xs text-muted-foreground">
                  {new Date(p.createdAt).toLocaleDateString()}{' '}
                  {new Date(p.createdAt).toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' })}
                </td>
                <td className="px-4 py-3">
                  <div className="font-medium truncate">{p.userDisplayName ?? '—'}</div>
                  <div className="text-xs text-muted-foreground truncate">{p.userEmail}</div>
                </td>
                <td className="px-4 py-3">
                  {p.subscriptionPlanCode ? (
                    <Badge variant={p.subscriptionPlanCode === 'free' ? 'muted' : 'default'}>
                      {p.subscriptionPlanCode}
                    </Badge>
                  ) : (
                    <span className="text-muted-foreground">—</span>
                  )}
                </td>
                <td className="px-4 py-3 text-right tabular-nums font-medium">
                  ${p.amount.toFixed(2)}
                </td>
                <td className="px-4 py-3">
                  <PayStatusBadge status={p.status} />
                </td>
                <td className="px-4 py-3 text-xs text-muted-foreground">{p.provider}</td>
                <td className="px-4 py-3 text-xs font-mono text-muted-foreground truncate max-w-[140px]">
                  {p.externalPaymentId ?? '—'}
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>

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

function PayStatusBadge({ status }: { status: string }) {
  if (status === 'succeeded') return <Badge variant="success">Succeeded</Badge>;
  if (status === 'pending') return <Badge variant="warning">Pending</Badge>;
  if (status === 'failed') return <Badge variant="danger">Failed</Badge>;
  if (status === 'refunded') return <Badge variant="muted">Refunded</Badge>;
  if (status === 'disputed') return <Badge variant="danger">Disputed</Badge>;
  return <Badge variant="muted">{status}</Badge>;
}