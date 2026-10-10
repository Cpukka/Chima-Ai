'use client';

import { useState } from 'react';
import { useQuery } from '@tanstack/react-query';
import { ChevronLeft, ChevronRight, Search } from 'lucide-react';
import { Badge } from '@/components/ui/badge';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { adminAuditApi } from '@/lib/api/admin';

const PAGE_SIZE = 50;

export default function AuditLogsPage() {
  const [page, setPage] = useState(1);
  const [actionFilter, setActionFilter] = useState('');

  const logs = useQuery({
    queryKey: ['admin', 'audit-logs', page, actionFilter],
    queryFn: () =>
      adminAuditApi.list({
        page,
        pageSize: PAGE_SIZE,
        action: actionFilter || undefined
      })
  });

  const data = logs.data;
  const totalPages = data ? Math.ceil(data.total / PAGE_SIZE) : 1;

  return (
    <div className="space-y-5">
      <div>
        <h1 className="text-2xl font-semibold tracking-tight">Audit Logs</h1>
        <p className="text-sm text-muted-foreground mt-1">
          {data?.total ?? 0} admin actions recorded
        </p>
      </div>

      <div className="relative max-w-md">
        <Search className="absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-muted-foreground" />
        <Input
          placeholder="Filter by action (e.g. user.suspend)"
          value={actionFilter}
          onChange={(e) => {
            setActionFilter(e.target.value);
            setPage(1);
          }}
          className="pl-9"
        />
      </div>

      <div className="rounded-xl border bg-card overflow-hidden">
        <table className="w-full text-sm">
          <thead className="bg-muted/50 text-muted-foreground text-xs uppercase tracking-wider">
            <tr>
              <th className="text-left font-medium px-4 py-3">When</th>
              <th className="text-left font-medium px-4 py-3">Actor</th>
              <th className="text-left font-medium px-4 py-3">Action</th>
              <th className="text-left font-medium px-4 py-3">Target</th>
              <th className="text-left font-medium px-4 py-3">IP</th>
            </tr>
          </thead>
          <tbody>
            {logs.isLoading && (
              <tr>
                <td colSpan={5} className="p-8 text-center text-muted-foreground">
                  Loading…
                </td>
              </tr>
            )}

            {!logs.isLoading && (data?.items ?? []).length === 0 && (
              <tr>
                <td colSpan={5} className="p-8 text-center text-muted-foreground">
                  No audit logs match this filter.
                </td>
              </tr>
            )}

            {(data?.items ?? []).map((l) => (
              <tr key={l.id} className="border-t border-border/40">
                <td className="px-4 py-3 text-xs text-muted-foreground whitespace-nowrap">
                  {new Date(l.createdAt).toLocaleString()}
                </td>
                <td className="px-4 py-3">
                  <div className="text-xs truncate max-w-[180px]">
                    {l.actorEmail ?? 'system'}
                  </div>
                </td>
                <td className="px-4 py-3">
                  <Badge variant="default">{l.action}</Badge>
                </td>
                <td className="px-4 py-3 text-xs text-muted-foreground">
                  {l.entityType ? (
                    <>
                      <span className="font-medium text-foreground">{l.entityType}</span>
                      {l.entityId && (
                        <span className="ml-1 font-mono truncate max-w-[120px] inline-block align-bottom">
                          {l.entityId.slice(0, 8)}…
                        </span>
                      )}
                    </>
                  ) : (
                    '—'
                  )}
                </td>
                <td className="px-4 py-3 text-xs font-mono text-muted-foreground">
                  {l.ipAddress ?? '—'}
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