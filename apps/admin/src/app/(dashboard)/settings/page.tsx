'use client';

import { useState } from 'react';
import { useQuery, useQueryClient, useMutation } from '@tanstack/react-query';
import { toast } from 'sonner';
import { Check, X } from 'lucide-react';
import { Card, CardContent } from '@/components/ui/card';
import { Input } from '@/components/ui/input';
import { Button } from '@/components/ui/button';
import { adminSettingsApi, type AdminSystemSetting } from '@/lib/api/admin';

export default function SettingsPage() {
  const settings = useQuery({
    queryKey: ['admin', 'system-settings'],
    queryFn: adminSettingsApi.list
  });

  return (
    <div className="space-y-5">
      <div>
        <h1 className="text-2xl font-semibold tracking-tight">System Settings</h1>
        <p className="text-sm text-muted-foreground mt-1">
          Application-wide configuration editable at runtime
        </p>
      </div>

      <Card>
        <CardContent className="p-0">
          {settings.isLoading && <div className="p-6 text-sm text-muted-foreground">Loading…</div>}
          <div className="divide-y">
            {(settings.data ?? []).map((s) => (
              <SettingRow key={s.key} setting={s} />
            ))}
          </div>
        </CardContent>
      </Card>
    </div>
  );
}

function SettingRow({ setting }: { setting: AdminSystemSetting }) {
  const qc = useQueryClient();
  const [value, setValue] = useState(setting.value);
  const [editing, setEditing] = useState(false);

  const update = useMutation({
    mutationFn: (v: string) => adminSettingsApi.update(setting.key, v),
    onSuccess: () => {
      toast.success(`${setting.key} updated`);
      qc.invalidateQueries({ queryKey: ['admin', 'system-settings'] });
      setEditing(false);
    },
    onError: (e) => toast.error(e instanceof Error ? e.message : 'Failed')
  });

  const isBool = setting.valueType === 'bool';

  return (
    <div className="p-4 flex items-start gap-3">
      <div className="flex-1 min-w-0">
        <div className="flex items-center gap-2">
          <code className="text-xs text-muted-foreground bg-muted px-1.5 py-0.5 rounded">
            {setting.key}
          </code>
          {setting.isSecret && (
            <span className="text-xs text-amber-600">(secret)</span>
          )}
        </div>
        {setting.description && (
          <p className="text-xs text-muted-foreground mt-1">{setting.description}</p>
        )}
        <div className="mt-2 flex items-center gap-2">
          {editing ? (
            <>
              <Input
                value={value}
                onChange={(e) => setValue(e.target.value)}
                className="max-w-xs h-8 text-sm"
                disabled={setting.isSecret}
              />
              <Button
                size="sm"
                onClick={() => update.mutate(value)}
                disabled={update.isPending || setting.isSecret}
              >
                <Check className="h-3.5 w-3.5" />
              </Button>
              <Button
                size="sm"
                variant="ghost"
                onClick={() => {
                  setValue(setting.value);
                  setEditing(false);
                }}
              >
                <X className="h-3.5 w-3.5" />
              </Button>
            </>
          ) : (
            <>
              <span className="text-sm font-medium">{setting.value}</span>
              {!setting.isSecret && (
                <Button size="sm" variant="outline" onClick={() => setEditing(true)}>
                  Edit
                </Button>
              )}
            </>
          )}
        </div>
      </div>
    </div>
  );
}