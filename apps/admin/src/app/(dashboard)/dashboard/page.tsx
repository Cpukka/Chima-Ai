'use client';

import { useQuery } from '@tanstack/react-query';
import { useState } from 'react';
import {
  Users,
  DollarSign,
  Cpu,
  MessageSquare,
  TrendingUp,
  AlertTriangle,
  HardDrive,
  Sparkles
} from 'lucide-react';
import {
  ResponsiveContainer,
  AreaChart,
  Area,
  XAxis,
  YAxis,
  Tooltip,
  CartesianGrid,
  BarChart,
  Bar
} from 'recharts';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { adminApi } from '@/lib/api/admin';
import { cn } from '@/lib/utils/cn';

const RANGES = [
  { label: '7d', days: 7 },
  { label: '30d', days: 30 },
  { label: '90d', days: 90 },
  { label: '1y', days: 365 }
];

export default function DashboardPage() {
  const [days, setDays] = useState(30);

  const metrics = useQuery({
    queryKey: ['admin', 'dashboard', days],
    queryFn: () => adminApi.dashboard(days)
  });

  if (metrics.isLoading) {
    return <div className="text-sm text-muted-foreground">Loading metrics…</div>;
  }

  const m = metrics.data!;

  return (
    <div className="space-y-6">
      {/* Header + range picker */}
      <div className="flex items-center justify-between flex-wrap gap-3">
        <div>
          <h1 className="text-2xl font-semibold tracking-tight">Dashboard</h1>
          <p className="text-sm text-muted-foreground mt-1">
            Overview of users, revenue, and AI usage
          </p>
        </div>
        <div className="inline-flex rounded-lg border border-border p-1 text-xs">
          {RANGES.map((r) => (
            <button
              key={r.days}
              onClick={() => setDays(r.days)}
              className={cn(
                'px-3 py-1 rounded-md',
                days === r.days ? 'bg-primary text-primary-foreground' : 'text-muted-foreground'
              )}
            >
              {r.label}
            </button>
          ))}
        </div>
      </div>

      {/* Metric cards */}
      <div className="grid gap-4 sm:grid-cols-2 lg:grid-cols-4">
        <MetricCard
          icon={Users}
          label="Total users"
          value={m.totalUsers.toLocaleString()}
          hint={`${m.newUsersInPeriod} new · ${m.activeUsersInPeriod} active`}
        />
        <MetricCard
          icon={DollarSign}
          label="Revenue"
          value={`$${m.revenueInPeriod.toFixed(2)}`}
          hint={`MRR $${m.monthlyRecurringRevenue.toFixed(2)}`}
        />
        <MetricCard
          icon={Sparkles}
          label="Gross margin"
          value={`${m.grossMarginPercent.toFixed(1)}%`}
          hint={`$${m.grossMarginInPeriod.toFixed(2)} profit`}
          tone={m.grossMarginPercent < 50 ? 'warn' : 'ok'}
        />
        <MetricCard
          icon={Cpu}
          label="AI requests"
          value={m.aiRequestsInPeriod.toLocaleString()}
          hint={`Cost $${m.estimatedAiCostInPeriod.toFixed(4)}`}
        />
        <MetricCard
          icon={MessageSquare}
          label="Conversations"
          value={m.totalConversations.toLocaleString()}
          hint={`${m.totalMessages.toLocaleString()} messages`}
        />
        <MetricCard
          icon={TrendingUp}
          label="Paid users"
          value={m.paidUsers.toLocaleString()}
          hint={`${m.freeUsers} on free`}
        />
        <MetricCard
          icon={AlertTriangle}
          label="Failed requests"
          value={m.failedRequestsInPeriod.toLocaleString()}
          hint={m.aiRequestsInPeriod > 0
            ? `${((m.failedRequestsInPeriod / m.aiRequestsInPeriod) * 100).toFixed(1)}% fail rate`
            : '—'}
          tone={m.failedRequestsInPeriod > 0 ? 'warn' : 'ok'}
        />
        <MetricCard
          icon={HardDrive}
          label="Storage"
          value={formatBytes(m.totalStorageBytes)}
          hint="Uploaded documents"
        />
      </div>

      {/* Charts */}
      <div className="grid gap-4 lg:grid-cols-2">
        <ChartCard title="New users">
          <ResponsiveContainer width="100%" height={240}>
            <AreaChart data={m.newUsersSeries}>
              <defs>
                <linearGradient id="usersGrad" x1="0" y1="0" x2="0" y2="1">
                  <stop offset="0%" stopColor="hsl(262 83% 58%)" stopOpacity={0.4} />
                  <stop offset="100%" stopColor="hsl(262 83% 58%)" stopOpacity={0} />
                </linearGradient>
              </defs>
              <CartesianGrid strokeDasharray="3 3" vertical={false} stroke="hsl(var(--border))" />
              <XAxis dataKey="date" tickFormatter={shortDate} fontSize={11} stroke="hsl(var(--muted-foreground))" />
              <YAxis fontSize={11} stroke="hsl(var(--muted-foreground))" allowDecimals={false} />
              <Tooltip contentStyle={tooltipStyle} labelFormatter={longDate} />
              <Area type="monotone" dataKey="value" stroke="hsl(262 83% 58%)" fill="url(#usersGrad)" strokeWidth={2} />
            </AreaChart>
          </ResponsiveContainer>
        </ChartCard>

        <ChartCard title="Revenue">
          <ResponsiveContainer width="100%" height={240}>
            <BarChart data={m.revenueSeries}>
              <CartesianGrid strokeDasharray="3 3" vertical={false} stroke="hsl(var(--border))" />
              <XAxis dataKey="date" tickFormatter={shortDate} fontSize={11} stroke="hsl(var(--muted-foreground))" />
              <YAxis fontSize={11} stroke="hsl(var(--muted-foreground))" tickFormatter={(v) => `$${v}`} />
              <Tooltip contentStyle={tooltipStyle} labelFormatter={longDate} formatter={(v) => [`$${Number(v).toFixed(2)}`, 'Revenue']} />
              <Bar dataKey="value" fill="hsl(262 83% 58%)" radius={[4, 4, 0, 0]} />
            </BarChart>
          </ResponsiveContainer>
        </ChartCard>

        <ChartCard title="AI requests" className="lg:col-span-2">
          <ResponsiveContainer width="100%" height={240}>
            <AreaChart data={m.aiRequestsSeries}>
              <defs>
                <linearGradient id="aiGrad" x1="0" y1="0" x2="0" y2="1">
                  <stop offset="0%" stopColor="hsl(262 83% 58%)" stopOpacity={0.4} />
                  <stop offset="100%" stopColor="hsl(262 83% 58%)" stopOpacity={0} />
                </linearGradient>
              </defs>
              <CartesianGrid strokeDasharray="3 3" vertical={false} stroke="hsl(var(--border))" />
              <XAxis dataKey="date" tickFormatter={shortDate} fontSize={11} stroke="hsl(var(--muted-foreground))" />
              <YAxis fontSize={11} stroke="hsl(var(--muted-foreground))" allowDecimals={false} />
              <Tooltip contentStyle={tooltipStyle} labelFormatter={longDate} />
              <Area type="monotone" dataKey="value" stroke="hsl(262 83% 58%)" fill="url(#aiGrad)" strokeWidth={2} />
            </AreaChart>
          </ResponsiveContainer>
        </ChartCard>
      </div>
    </div>
  );
}

function MetricCard({
  icon: Icon,
  label,
  value,
  hint,
  tone = 'ok'
}: {
  icon: React.ComponentType<{ className?: string }>;
  label: string;
  value: string;
  hint?: string;
  tone?: 'ok' | 'warn';
}) {
  return (
    <Card>
      <CardContent className="p-5">
        <div className="flex items-start justify-between">
          <div>
            <div className="text-xs uppercase tracking-wider text-muted-foreground">{label}</div>
            <div className="text-2xl font-semibold mt-1 tabular-nums">{value}</div>
            {hint && (
              <div className={cn('text-xs mt-1', tone === 'warn' ? 'text-amber-600' : 'text-muted-foreground')}>
                {hint}
              </div>
            )}
          </div>
          <div
            className={cn(
              'h-9 w-9 rounded-lg flex items-center justify-center shrink-0',
              tone === 'warn' ? 'bg-amber-500/10 text-amber-600' : 'bg-primary/10 text-primary'
            )}
          >
            <Icon className="h-4 w-4" />
          </div>
        </div>
      </CardContent>
    </Card>
  );
}

function ChartCard({
  title,
  children,
  className
}: {
  title: string;
  children: React.ReactNode;
  className?: string;
}) {
  return (
    <Card className={className}>
      <CardHeader>
        <CardTitle className="text-sm">{title}</CardTitle>
      </CardHeader>
      <CardContent>{children}</CardContent>
    </Card>
  );
}

function shortDate(d: string): string {
  const dt = new Date(d);
  return dt.toLocaleDateString(undefined, { month: 'short', day: 'numeric' });
}

function longDate(d: string): string {
  return new Date(d).toLocaleDateString(undefined, {
    weekday: 'short',
    month: 'short',
    day: 'numeric'
  });
}

function formatBytes(bytes: number): string {
  if (bytes < 1024) return `${bytes} B`;
  if (bytes < 1024 * 1024) return `${(bytes / 1024).toFixed(1)} KB`;
  if (bytes < 1024 * 1024 * 1024) return `${(bytes / (1024 * 1024)).toFixed(1)} MB`;
  return `${(bytes / (1024 * 1024 * 1024)).toFixed(2)} GB`;
}

const tooltipStyle = {
  background: 'hsl(var(--card))',
  border: '1px solid hsl(var(--border))',
  borderRadius: 8,
  fontSize: 12
};