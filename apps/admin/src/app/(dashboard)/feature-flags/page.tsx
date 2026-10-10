'use client';

import { useQuery, useQueryClient, useMutation } from '@tanstack/react-query';
import { toast } from 'sonner';
import { ToggleLeft, ToggleRight } from 'lucide-react';
import { Card, CardContent } from '@/components/ui/card';
import { Input } from '@/components/ui/input';
import { Button } from '@/components/ui/button';
import { adminFeatureFlagsApi } from '@/lib/api/admin';
import { useState } from 'react';

export default function FeatureFlagsPage() {
  const qc = useQueryClient();
  const flags = useQuery({
    queryKey: ['admin', 'feature-flags'],
    queryFn: adminFeatureFlagsApi.list
  });

  const update = useMutation({
    mutationFn: (args: { key: string; body: Parameters<typeof adminFeatureFlagsApi.update>[1] }) =>
      adminFeatureFlagsApi.update(args.key, args.body),
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: ['admin', 'feature-flags'] });
    },
    onError: (e) => toast.error(e instanceof Error ? e.message : 'Failed')
  });

  return (
    <div className="space-y-5">
      <div>
        <h1 className="text-2xl font-semibold tracking-tight">Feature Flags</h1>
        <p className="text-sm text-muted-foreground mt-1">
          Enable or disable features per environment
        </p>
      </div>

      <Card>
        <CardContent className="p-0">
          {flags.isLoading && (
            <div className="p-6 text-sm text-muted-foreground">Loading…</div>
          )}
          {!flags.isLoading && (flags.data ?? []).length === 0 && (
            <div className="p-6 text-sm text-muted-foreground">No flags configured.</div>
          )}
          <div className="divide-y">
            {(flags.data ?? []).map((f) => (
              <div key={f.key} className="p-4 flex items-start justify-between gap-4">
                <div className="flex-1 min-w-0">
                  <div className="flex items-center gap-2">
                    <span className="font-medium">{f.displayName}</span>
                    <code className="text-xs text-muted-foreground bg-muted px-1.5 py-0.5 rounded">
                      {f.key}
                    </code>
                  </div>
                  {f.description && (
                    <p className="text-xs text-muted-foreground mt-1">{f.description}</p>
                  )}
                  <div className="flex items-center gap-3 mt-2">
                    <label className="text-xs text-muted-foreground flex items-center gap-2">
                      Rollout
                      <Input
                        type="number"
                        min={0}
                        max={100}
                        value={f.rolloutPercent}
                        onChange={(e) =>
                          update.mutate({
                            key: f.key,
                            body: { rolloutPercent: parseInt(e.target.value) || 0 }
                          })
                        }
                        className="w-20 h-8 text-xs"
                      />
                      %
                    </label>
                  </div>
                </div>

                <button
                  onClick={() => update.mutate({ key: f.key, body: { isEnabled: !f.isEnabled } })}
                  className="shrink-0 mt-1"
                  aria-label="Toggle flag"
                >
                  {f.isEnabled ? (
                    <ToggleRight className="h-8 w-8 text-primary" />
                  ) : (
                    <ToggleLeft className="h-8 w-8 text-muted-foreground" />
                  )}
                </button>
              </div>
            ))}
          </div>
        </CardContent>
      </Card>
    </div>
  );
}