'use client';

import { useState } from 'react';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { toast } from 'sonner';
import { Cpu, Plus, Star, Check, X, Settings2 } from 'lucide-react';
import { Badge } from '@/components/ui/badge';
import { Button } from '@/components/ui/button';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { Input } from '@/components/ui/input';
import { Label } from '@/components/ui/label';
import {
  Select,
  SelectTrigger,
  SelectValue,
  SelectContent,
  SelectItem
} from '@/components/ui/select';
import { adminAiApi, type AdminAiModel } from '@/lib/api/admin';

export default function AiModelsPage() {
  const qc = useQueryClient();
  const [showNewProvider, setShowNewProvider] = useState(false);
  const [showNewModel, setShowNewModel] = useState(false);
  const [editingModel, setEditingModel] = useState<AdminAiModel | null>(null);

  const providers = useQuery({
    queryKey: ['admin', 'ai', 'providers'],
    queryFn: adminAiApi.listProviders
  });

  const models = useQuery({
    queryKey: ['admin', 'ai', 'models'],
    queryFn: adminAiApi.listModels
  });

  const setDefault = useMutation({
    mutationFn: adminAiApi.setDefaultModel,
    onSuccess: () => {
      toast.success('Default model updated');
      qc.invalidateQueries({ queryKey: ['admin', 'ai', 'models'] });
    },
    onError: (e) => toast.error(e instanceof Error ? e.message : 'Failed')
  });

  const toggleActive = useMutation({
    mutationFn: (args: { id: string; active: boolean }) =>
      adminAiApi.updateModel(args.id, { isActive: args.active }),
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: ['admin', 'ai', 'models'] });
    },
    onError: (e) => toast.error(e instanceof Error ? e.message : 'Failed')
  });

  return (
    <div className="space-y-6">
      <div className="flex items-end justify-between flex-wrap gap-3">
        <div>
          <h1 className="text-2xl font-semibold tracking-tight">AI Models</h1>
          <p className="text-sm text-muted-foreground mt-1">
            Providers and models available to the assistant
          </p>
        </div>
        <div className="flex gap-2">
          <Button variant="outline" onClick={() => setShowNewProvider(true)}>
            <Plus className="h-4 w-4 mr-2" /> Add provider
          </Button>
          <Button onClick={() => setShowNewModel(true)} disabled={!providers.data?.length}>
            <Plus className="h-4 w-4 mr-2" /> Add model
          </Button>
        </div>
      </div>

      {/* Providers */}
      <Card>
        <CardHeader>
          <CardTitle className="text-sm">Providers</CardTitle>
        </CardHeader>
        <CardContent className="space-y-3">
          {providers.isLoading && <div className="text-sm text-muted-foreground">Loading…</div>}
          {(providers.data ?? []).map((p) => (
            <div key={p.id} className="flex items-center justify-between rounded-lg border p-3">
              <div className="flex items-center gap-3">
                <div className="h-9 w-9 rounded-lg bg-primary/10 text-primary flex items-center justify-center">
                  <Cpu className="h-4 w-4" />
                </div>
                <div>
                  <div className="font-medium">{p.displayName}</div>
                  <div className="text-xs text-muted-foreground">
                    {p.name} · {p.modelCount} model{p.modelCount === 1 ? '' : 's'}
                  </div>
                </div>
              </div>
              <div className="flex items-center gap-3">
                <div className="text-xs text-muted-foreground font-mono max-w-[200px] truncate">
                  {p.baseUrl}
                </div>
                {p.isActive ? (
                  <Badge variant="success">Active</Badge>
                ) : (
                  <Badge variant="muted">Disabled</Badge>
                )}
              </div>
            </div>
          ))}
        </CardContent>
      </Card>

      {/* Models */}
      <Card>
        <CardHeader>
          <CardTitle className="text-sm">Models</CardTitle>
        </CardHeader>
        <CardContent>
          <div className="overflow-hidden rounded-lg border">
            <table className="w-full text-sm">
              <thead className="bg-muted/50 text-muted-foreground text-xs uppercase tracking-wider">
                <tr>
                  <th className="text-left font-medium px-4 py-2">Name</th>
                  <th className="text-left font-medium px-4 py-2">Identifier</th>
                  <th className="text-left font-medium px-4 py-2">Provider</th>
                  <th className="text-right font-medium px-4 py-2">Input $/1M</th>
                  <th className="text-right font-medium px-4 py-2">Output $/1M</th>
                  <th className="text-right font-medium px-4 py-2">Context</th>
                  <th className="text-left font-medium px-4 py-2">Status</th>
                  <th className="w-24"></th>
                </tr>
              </thead>
              <tbody>
                {models.isLoading && (
                  <tr>
                    <td colSpan={8} className="p-6 text-center text-muted-foreground">
                      Loading…
                    </td>
                  </tr>
                )}
                {(models.data ?? []).map((m) => (
                  <tr key={m.id} className="border-t border-border/40">
                    <td className="px-4 py-3">
                      <div className="flex items-center gap-2">
                        {m.isDefault && <Star className="h-3.5 w-3.5 text-amber-500 fill-amber-500" />}
                        <span className="font-medium">{m.displayName}</span>
                      </div>
                    </td>
                    <td className="px-4 py-3 text-xs font-mono text-muted-foreground">
                      {m.modelIdentifier}
                    </td>
                    <td className="px-4 py-3 text-muted-foreground">{m.providerName}</td>
                    <td className="px-4 py-3 text-right tabular-nums">${m.inputPricePer1M.toFixed(2)}</td>
                    <td className="px-4 py-3 text-right tabular-nums">${m.outputPricePer1M.toFixed(2)}</td>
                    <td className="px-4 py-3 text-right tabular-nums text-muted-foreground">
                      {(m.contextWindow / 1000).toFixed(0)}K
                    </td>
                    <td className="px-4 py-3">
                      {!m.isActive ? (
                        <Badge variant="muted">Disabled</Badge>
                      ) : m.isPremium ? (
                        <Badge variant="warning">Premium</Badge>
                      ) : (
                        <Badge variant="success">Active</Badge>
                      )}
                    </td>
                    <td className="px-4 py-3">
                      <div className="flex gap-1">
                        {!m.isDefault && m.isActive && (
                          <Button
                            size="sm"
                            variant="ghost"
                            onClick={() => setDefault.mutate(m.id)}
                            title="Set as default"
                          >
                            <Star className="h-3.5 w-3.5" />
                          </Button>
                        )}
                        <Button
                          size="sm"
                          variant="ghost"
                          onClick={() => setEditingModel(m)}
                          title="Edit"
                        >
                          <Settings2 className="h-3.5 w-3.5" />
                        </Button>
                        <Button
                          size="sm"
                          variant="ghost"
                          onClick={() => toggleActive.mutate({ id: m.id, active: !m.isActive })}
                          title={m.isActive ? 'Disable' : 'Enable'}
                        >
                          {m.isActive ? (
                            <X className="h-3.5 w-3.5 text-muted-foreground" />
                          ) : (
                            <Check className="h-3.5 w-3.5 text-primary" />
                          )}
                        </Button>
                      </div>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        </CardContent>
      </Card>

      {showNewProvider && (
        <NewProviderModal onClose={() => setShowNewProvider(false)} />
      )}
      {showNewModel && (
        <NewModelModal
          providers={providers.data ?? []}
          onClose={() => setShowNewModel(false)}
        />
      )}
      {editingModel && (
        <EditModelModal model={editingModel} onClose={() => setEditingModel(null)} />
      )}
    </div>
  );
}

function NewProviderModal({ onClose }: { onClose: () => void }) {
  const qc = useQueryClient();
  const [name, setName] = useState('');
  const [displayName, setDisplayName] = useState('');
  const [baseUrl, setBaseUrl] = useState('https://api.openai.com/v1');
  const [apiKeySecretName, setApiKeySecretName] = useState('AI_API_KEY');
  const [busy, setBusy] = useState(false);

  async function submit() {
    if (!name || !displayName || !baseUrl || !apiKeySecretName) {
      toast.error('All fields required');
      return;
    }
    setBusy(true);
    try {
      await adminAiApi.createProvider({ name, displayName, baseUrl, apiKeySecretName });
      toast.success('Provider created');
      qc.invalidateQueries({ queryKey: ['admin', 'ai'] });
      onClose();
    } catch (e) {
      toast.error(e instanceof Error ? e.message : 'Failed');
    } finally {
      setBusy(false);
    }
  }

  return (
    <Modal title="Add AI provider" onClose={onClose}>
      <div className="space-y-3">
        <Field label="Name (lowercase, no spaces)">
          <Input value={name} onChange={(e) => setName(e.target.value)} placeholder="openai" />
        </Field>
        <Field label="Display name">
          <Input
            value={displayName}
            onChange={(e) => setDisplayName(e.target.value)}
            placeholder="OpenAI"
          />
        </Field>
        <Field label="Base URL">
          <Input value={baseUrl} onChange={(e) => setBaseUrl(e.target.value)} />
        </Field>
        <Field label="API key secret env var">
          <Input
            value={apiKeySecretName}
            onChange={(e) => setApiKeySecretName(e.target.value)}
          />
        </Field>
        <div className="flex justify-end gap-2 pt-3">
          <Button variant="outline" onClick={onClose}>Cancel</Button>
          <Button onClick={submit} disabled={busy}>
            {busy ? 'Saving…' : 'Create'}
          </Button>
        </div>
      </div>
    </Modal>
  );
}

function NewModelModal({
  providers,
  onClose
}: {
  providers: { id: string; displayName: string }[];
  onClose: () => void;
}) {
  const qc = useQueryClient();
  const [providerId, setProviderId] = useState(providers[0]?.id ?? '');
  const [name, setName] = useState('');
  const [displayName, setDisplayName] = useState('');
  const [modelIdentifier, setModelIdentifier] = useState('');
  const [inputPricePer1M, setInputPrice] = useState('0');
  const [outputPricePer1M, setOutputPrice] = useState('0');
  const [contextWindow, setContextWindow] = useState('128000');
  const [maxOutputTokens, setMaxOutputTokens] = useState('4096');
  const [isPremium, setIsPremium] = useState(false);
  const [capabilitiesCsv, setCapabilitiesCsv] = useState('Text');
  const [busy, setBusy] = useState(false);

  async function submit() {
    if (!providerId || !name || !displayName || !modelIdentifier) {
      toast.error('Required fields missing');
      return;
    }
    setBusy(true);
    try {
      await adminAiApi.createModel({
        providerId,
        name,
        displayName,
        modelIdentifier,
        inputPricePer1M: parseFloat(inputPricePer1M) || 0,
        outputPricePer1M: parseFloat(outputPricePer1M) || 0,
        contextWindow: parseInt(contextWindow) || 128000,
        maxOutputTokens: parseInt(maxOutputTokens) || 4096,
        isPremium,
        capabilitiesCsv
      });
      toast.success('Model created');
      qc.invalidateQueries({ queryKey: ['admin', 'ai'] });
      onClose();
    } catch (e) {
      toast.error(e instanceof Error ? e.message : 'Failed');
    } finally {
      setBusy(false);
    }
  }

  return (
    <Modal title="Add AI model" onClose={onClose}>
      <div className="space-y-3">
        <Field label="Provider">
          <Select value={providerId} onValueChange={setProviderId}>
            <SelectTrigger>
              <SelectValue />
            </SelectTrigger>
            <SelectContent>
              {providers.map((p) => (
                <SelectItem key={p.id} value={p.id}>
                  {p.displayName}
                </SelectItem>
              ))}
            </SelectContent>
          </Select>
        </Field>

        <div className="grid grid-cols-2 gap-3">
          <Field label="Internal name">
            <Input value={name} onChange={(e) => setName(e.target.value)} placeholder="gpt-4o-mini" />
          </Field>
          <Field label="Display name">
            <Input
              value={displayName}
              onChange={(e) => setDisplayName(e.target.value)}
              placeholder="GPT-4o mini"
            />
          </Field>
        </div>

        <Field label="Model identifier (used by provider API)">
          <Input
            value={modelIdentifier}
            onChange={(e) => setModelIdentifier(e.target.value)}
            placeholder="gpt-4o-mini"
          />
        </Field>

        <div className="grid grid-cols-2 gap-3">
          <Field label="Input $ per 1M tokens">
            <Input value={inputPricePer1M} onChange={(e) => setInputPrice(e.target.value)} />
          </Field>
          <Field label="Output $ per 1M tokens">
            <Input value={outputPricePer1M} onChange={(e) => setOutputPrice(e.target.value)} />
          </Field>
        </div>

        <div className="grid grid-cols-2 gap-3">
          <Field label="Context window">
            <Input value={contextWindow} onChange={(e) => setContextWindow(e.target.value)} />
          </Field>
          <Field label="Max output tokens">
            <Input value={maxOutputTokens} onChange={(e) => setMaxOutputTokens(e.target.value)} />
          </Field>
        </div>

        <Field label="Capabilities (comma-separated)">
          <Input
            value={capabilitiesCsv}
            onChange={(e) => setCapabilitiesCsv(e.target.value)}
            placeholder="Text,Vision,FileAnalysis"
          />
        </Field>

        <label className="flex items-center gap-2 text-sm">
          <input
            type="checkbox"
            checked={isPremium}
            onChange={(e) => setIsPremium(e.target.checked)}
          />
          Premium (gated to paid plans)
        </label>

        <div className="flex justify-end gap-2 pt-3">
          <Button variant="outline" onClick={onClose}>Cancel</Button>
          <Button onClick={submit} disabled={busy}>
            {busy ? 'Saving…' : 'Create'}
          </Button>
        </div>
      </div>
    </Modal>
  );
}

function EditModelModal({ model, onClose }: { model: AdminAiModel; onClose: () => void }) {
  const qc = useQueryClient();
  const [displayName, setDisplayName] = useState(model.displayName);
  const [inputPricePer1M, setInputPrice] = useState(model.inputPricePer1M.toString());
  const [outputPricePer1M, setOutputPrice] = useState(model.outputPricePer1M.toString());
  const [contextWindow, setContextWindow] = useState(model.contextWindow.toString());
  const [maxOutputTokens, setMaxOutputTokens] = useState(model.maxOutputTokens.toString());
  const [isPremium, setIsPremium] = useState(model.isPremium);
  const [capabilitiesCsv, setCapabilitiesCsv] = useState(model.capabilitiesCsv);
  const [busy, setBusy] = useState(false);

  async function submit() {
    setBusy(true);
    try {
      await adminAiApi.updateModel(model.id, {
        displayName,
        inputPricePer1M: parseFloat(inputPricePer1M) || 0,
        outputPricePer1M: parseFloat(outputPricePer1M) || 0,
        contextWindow: parseInt(contextWindow) || 128000,
        maxOutputTokens: parseInt(maxOutputTokens) || 4096,
        isPremium,
        capabilitiesCsv
      });
      toast.success('Model updated');
      qc.invalidateQueries({ queryKey: ['admin', 'ai'] });
      onClose();
    } catch (e) {
      toast.error(e instanceof Error ? e.message : 'Failed');
    } finally {
      setBusy(false);
    }
  }

  return (
    <Modal title={`Edit ${model.name}`} onClose={onClose}>
      <div className="space-y-3">
        <Field label="Display name">
          <Input value={displayName} onChange={(e) => setDisplayName(e.target.value)} />
        </Field>
        <div className="grid grid-cols-2 gap-3">
          <Field label="Input $/1M">
            <Input value={inputPricePer1M} onChange={(e) => setInputPrice(e.target.value)} />
          </Field>
          <Field label="Output $/1M">
            <Input value={outputPricePer1M} onChange={(e) => setOutputPrice(e.target.value)} />
          </Field>
        </div>
        <div className="grid grid-cols-2 gap-3">
          <Field label="Context window">
            <Input value={contextWindow} onChange={(e) => setContextWindow(e.target.value)} />
          </Field>
          <Field label="Max output tokens">
            <Input value={maxOutputTokens} onChange={(e) => setMaxOutputTokens(e.target.value)} />
          </Field>
        </div>
        <Field label="Capabilities">
          <Input value={capabilitiesCsv} onChange={(e) => setCapabilitiesCsv(e.target.value)} />
        </Field>
        <label className="flex items-center gap-2 text-sm">
          <input
            type="checkbox"
            checked={isPremium}
            onChange={(e) => setIsPremium(e.target.checked)}
          />
          Premium
        </label>
        <div className="flex justify-end gap-2 pt-3">
          <Button variant="outline" onClick={onClose}>Cancel</Button>
          <Button onClick={submit} disabled={busy}>
            {busy ? 'Saving…' : 'Save'}
          </Button>
        </div>
      </div>
    </Modal>
  );
}

function Field({ label, children }: { label: string; children: React.ReactNode }) {
  return (
    <div className="space-y-1.5">
      <Label>{label}</Label>
      {children}
    </div>
  );
}

function Modal({
  title,
  onClose,
  children
}: {
  title: string;
  onClose: () => void;
  children: React.ReactNode;
}) {
  return (
    <div
      className="fixed inset-0 z-50 bg-black/40 backdrop-blur-sm flex items-center justify-center p-4"
      onClick={onClose}
    >
      <div
        className="w-full max-w-lg rounded-2xl border bg-card p-6 shadow-lg"
        onClick={(e) => e.stopPropagation()}
      >
        <div className="flex items-center justify-between mb-4">
          <h3 className="text-lg font-semibold">{title}</h3>
          <button onClick={onClose} className="text-muted-foreground hover:text-foreground">
            <X className="h-4 w-4" />
          </button>
        </div>
        {children}
      </div>
    </div>
  );
}