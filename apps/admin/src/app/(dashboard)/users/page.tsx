'use client';

import { useState } from 'react';
import { useQuery } from '@tanstack/react-query';
import { Search, ChevronLeft, ChevronRight, Shield, ShieldOff } from 'lucide-react';
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
import { adminApi } from '@/lib/api/admin';
import { UserDetailDrawer } from '@/components/dashboard/UserDetailDrawer';

const PAGE_SIZE = 25;

export default function UsersPage() {
  const [page, setPage] = useState(1);
  const [search, setSearch] = useState('');
  const [status, setStatus] = useState<string>('');
  const [plan, setPlan] = useState<string>('');
  const [openUserId, setOpenUserId] = useState<string | null>(null);

  const users = useQuery({
    queryKey: ['admin', 'users', page, search, status, plan],
    queryFn: () =>
      adminApi.users({
        page,
        pageSize: PAGE_SIZE,
        search: search || undefined,
        status: status || undefined,
        plan: plan || undefined
      })
  });

  const data = users.data;
  const totalPages = data?.totalPages ?? 1;

  return (
    <div className="space-y-5">
      <div className="flex items-end justify-between flex-wrap gap-3">
        <div>
          <h1 className="text-2xl font-semibold tracking-tight">Users</h1>
          <p className="text-sm text-muted-foreground mt-1">
            {data?.total ?? 0} total {data?.total === 1 ? 'user' : 'users'}
          </p>
        </div>
      </div>

      {/* Filters */}
      <div className="flex gap-3 flex-wrap items-center">
        <div className="relative flex-1 min-w-[240px]">
          <Search className="absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-muted-foreground" />
          <Input
            placeholder="Search by email or name"
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
          <SelectTrigger className="w-[140px]">
            <SelectValue placeholder="Status" />
          </SelectTrigger>
          <SelectContent>
            <SelectItem value="all">All statuses</SelectItem>
            <SelectItem value="active">Active</SelectItem>
            <SelectItem value="suspended">Suspended</SelectItem>
          </SelectContent>
        </Select>

        <Select
          value={plan || 'all'}
          onValueChange={(v) => {
            setPlan(v === 'all' ? '' : v);
            setPage(1);
          }}
        >
          <SelectTrigger className="w-[140px]">
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
              <th className="text-right font-medium px-4 py-3">Messages</th>
              <th className="text-right font-medium px-4 py-3">Spent</th>
              <th className="text-left font-medium px-4 py-3">Joined</th>
              <th className="w-10"></th>
            </tr>
          </thead>
          <tbody>
            {users.isLoading && (
              <tr>
                <td colSpan={7} className="p-8 text-center text-muted-foreground">
                  Loading…
                </td>
              </tr>
            )}

            {!users.isLoading && (data?.items ?? []).length === 0 && (
              <tr>
                <td colSpan={7} className="p-8 text-center text-muted-foreground">
                  No users match these filters.
                </td>
              </tr>
            )}

            {(data?.items ?? []).map((u) => (
              <tr
                key={u.id}
                className="border-t border-border/40 hover:bg-muted/30 cursor-pointer"
                onClick={() => setOpenUserId(u.id)}
              >
                <td className="px-4 py-3">
                  <div className="flex items-center gap-2">
                    {u.isAdmin && <Shield className="h-3.5 w-3.5 text-primary shrink-0" />}
                    <div className="min-w-0">
                      <div className="font-medium truncate">
                        {u.displayName ?? '—'}
                      </div>
                      <div className="text-xs text-muted-foreground truncate">
                        {u.email}
                      </div>
                    </div>
                  </div>
                </td>
                <td className="px-4 py-3">
                  <Badge variant={u.planCode === 'free' ? 'muted' : 'default'}>
                    {u.planName}
                  </Badge>
                </td>
                <td className="px-4 py-3">
                  {u.status === 'active' ? (
                    <Badge variant="success">Active</Badge>
                  ) : u.status === 'suspended' ? (
                    <Badge variant="danger">Suspended</Badge>
                  ) : (
                    <Badge variant="muted">{u.status}</Badge>
                  )}
                </td>
                <td className="px-4 py-3 text-right tabular-nums">
                  {u.totalMessages.toLocaleString()}
                </td>
                <td className="px-4 py-3 text-right tabular-nums">
                  ${u.totalSpent.toFixed(2)}
                </td>
                <td className="px-4 py-3 text-muted-foreground text-xs">
                  {new Date(u.createdAt).toLocaleDateString()}
                </td>
                <td className="px-4 py-3"></td>
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

      {/* Detail drawer */}
      <UserDetailDrawer
        userId={openUserId}
        onClose={() => setOpenUserId(null)}
      />
    </div>
  );
}