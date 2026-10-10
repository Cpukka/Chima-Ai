'use client';

import { useState } from 'react';
import { useQuery } from '@tanstack/react-query';
import {
  ResponsiveContainer,
  AreaChart,
  Area,
  XAxis,
  YAxis,
  Tooltip,
  CartesianGrid
} from 'recharts';
import { Cpu, DollarSign, Zap, TrendingUp } from 'lucide-react';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { adminApi } from '@/lib/api/admin';
import { cn } from '@/lib/utils/cn';

const RANGES = [
  { label: '7d', days: 7 },
  { label: '30d', days: 30 },
  { label: '90d', days: 90 },
  { label: '1y', days: 365 }
];

export default function UsagePage() {
  const [days, setDays] = useState(30);

  const costs = useQuery({
    queryKey: ['admin', 'ai-costs', days],
    queryFn: () => adminApi.aiCosts(days)
  });

  const data = costs.data;

  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between flex-wrap gap-3">
        <div>
          <h1 className="text-2xl font-semibold tracking-tight">Usage & Costs</h1>
          <p className="text-sm text-muted-foreground mt-1">
            AI consumption and gross margin
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

      {data && (
        <>
          <div className="grid gap-4 sm:grid-cols-2 lg:grid-cols-4">
            <StatCard icon={Zap} label="Requests" value={data.requestsInPeriod.toLocaleString()} />
            <StatCard
              icon={Cpu}
              label="Tokens"
              value={`${((data.inputTokensInPeriod + data.outputTokensInPeriod) / 1000).toFixed(0)}K`}
              hint={`${(data.inputTokensInPeriod / 1000).toFixed(0)}K in · ${(data.outputTokensInPeriod / 1000).toFixed(0)}K out`}
            />
            <StatCard
              icon={DollarSign}
              label="AI cost"
              value={`$${data.estimatedCostInPeriod.toFixed(4)}`}
            />
            <StatCard
              icon={TrendingUp}
              label="Margin"
              value={`${data.grossMarginPercent.toFixed(1)}%`}
              hint={`$${data.grossMarginInPeriod.toFixed(2)} profit`}
              tone={data.grossMarginPercent < 50 ? 'warn' : 'ok'}
            />
          </div>

          <Card>
            <CardHeader>
              <CardTitle className="text-sm">Daily AI cost</CardTitle>
            </CardHeader>
            <CardContent>
              <ResponsiveContainer width="100%" height={260}>
                <AreaChart data={data.dailyCost}>
                  <defs>
                    <linearGradient id="costGrad" x1="0" y1="0" x2="0" y2="1">
                      <stop offset="0%" stopColor="hsl(262 83% 58%)" stopOpacity={0.4} />
                      <stop offset="100%" stopColor="hsl(262 83% 58%)" stopOpacity={0} />
                    </linearGradient>
                  </defs>
                  <CartesianGrid strokeDasharray="3 3" vertical={false} stroke="hsl(var(--border))" />
                  <XAxis
                    dataKey="date"
                    tickFormatter={(d) => new Date(d).toLocaleDateString(undefined, { month: 'short', day: 'numeric' })}
                    fontSize={11}
                    stroke="hsl(var(--muted-foreground))"
                  />
                  <YAxis
                    fontSize={11}
                    stroke="hsl(var(--muted-foreground))"
                    tickFormatter={(v) => `$${Number(v).toFixed(3)}`}
                  />
                  <Tooltip
                    contentStyle={{
                      background: 'hsl(var(--card))',
                      border: '1px solid hsl(var(--border))',
                      borderRadius: 8,
                      fontSize: 12
                    }}
                    formatter={(v) => [`$${Number(v).toFixed(4)}`, 'Cost']}
                  />
                  <Area
                    type="monotone"
                    dataKey="value"
                    stroke="hsl(262 83% 58%)"
                    fill="url(#costGrad)"
                    strokeWidth={2}
                  />
                </AreaChart>
              </ResponsiveContainer>
            </CardContent>
          </Card>

          <Card>
            <CardHeader>
              <CardTitle className="text-sm">Cost by model</CardTitle>
            </CardHeader>
            <CardContent>
              {data.byModel.length === 0 ? (
                <div className="text-sm text-muted-foreground py-6 text-center">
                  No AI usage in this period.
                </div>
              ) : (
                <div className="overflow-hidden rounded-lg border">
                  <table className="w-full text-sm">
                    <thead className="bg-muted/50 text-muted-foreground text-xs uppercase tracking-wider">
                      <tr>
                        <th className="text-left font-medium px-4 py-2">Model</th>
                        <th className="text-left font-medium px-4 py-2">Provider</th>
                        <th className="text-right font-medium px-4 py-2">Requests</th>
                        <th className="text-right font-medium px-4 py-2">In tokens</th>
                        <th className="text-right font-medium px-4 py-2">Out tokens</th>
                        <th className="text-right font-medium px-4 py-2">Cost</th>
                      </tr>
                    </thead>
                    <tbody>
                      {data.byModel.map((m) => (
                        <tr key={m.modelId} className="border-t border-border/40">
                          <td className="px-4 py-3 font-medium">{m.modelName}</td>
                          <td className="px-4 py-3 text-muted-foreground">{m.providerName}</td>
                          <td className="px-4 py-3 text-right tabular-nums">
                            {m.requests.toLocaleString()}
                          </td>
                          <td className="px-4 py-3 text-right tabular-nums text-muted-foreground">
                            {m.inputTokens.toLocaleString()}
                          </td>
                          <td className="px-4 py-3 text-right tabular-nums text-muted-foreground">
                            {m.outputTokens.toLocaleString()}
                          </td>
                          <td className="px-4 py-3 text-right tabular-nums font-medium">
                            ${m.estimatedCost.toFixed(4)}
                          </td>
                        </tr>
                      ))}
                    </tbody>
                  </table>
                </div>
              )}
            </CardContent>
          </Card>
        </>
      )}
    </div>
  );
}

function StatCard({
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