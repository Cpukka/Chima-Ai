'use client';

import { useRef, useEffect, useState } from 'react';
import { ArrowUp, Square, Paperclip, X, Loader2 } from 'lucide-react';
import { toast } from 'sonner';
import { Button } from '@/components/ui/button';
import { filesApi, formatBytes, type UploadedFile } from '@/lib/api/files';
import { cn } from '@/lib/utils/cn';

interface Props {
  onSend: (content: string, attachmentIds: string[]) => void;
  onStop?: () => void;
  isStreaming?: boolean;
  disabled?: boolean;
  initialValue?: string;
}

export function Composer({ onSend, onStop, isStreaming, disabled, initialValue }: Props) {
  const [value, setValue] = useState(initialValue ?? '');
  const [attachments, setAttachments] = useState<UploadedFile[]>([]);
  const [uploading, setUploading] = useState(false);
  const ref = useRef<HTMLTextAreaElement>(null);
  const fileInput = useRef<HTMLInputElement>(null);

  useEffect(() => {
    const el = ref.current;
    if (!el) return;
    el.style.height = 'auto';
    el.style.height = Math.min(el.scrollHeight, 240) + 'px';
  }, [value]);

  useEffect(() => {
    ref.current?.focus();
  }, []);

  useEffect(() => {
    if (initialValue !== undefined) setValue(initialValue);
  }, [initialValue]);

  async function pickFile(e: React.ChangeEvent<HTMLInputElement>) {
    const f = e.target.files?.[0];
    if (!f) return;
    setUploading(true);
    try {
      const uploaded = await filesApi.upload(f);
      setAttachments((prev) => [...prev, uploaded]);
      toast.success(`Attached ${f.name}`);
    } catch (err) {
      toast.error(err instanceof Error ? err.message : 'Upload failed');
    } finally {
      setUploading(false);
      if (fileInput.current) fileInput.current.value = '';
    }
  }

  function submit() {
    const v = value.trim();
    if ((!v && attachments.length === 0) || disabled || isStreaming) return;
    onSend(v || 'Please analyze the attached document.', attachments.map((a) => a.id));
    setValue('');
    setAttachments([]);
  }

  function onKeyDown(e: React.KeyboardEvent<HTMLTextAreaElement>) {
    if (e.key === 'Enter' && !e.shiftKey) {
      e.preventDefault();
      submit();
    }
  }

  return (
    <div className="border-t border-border/40 bg-background">
      <div className="max-w-3xl mx-auto p-4">
        {attachments.length > 0 && (
          <div className="flex flex-wrap gap-2 mb-2">
            {attachments.map((a) => (
              <div
                key={a.id}
                className="flex items-center gap-2 rounded-lg border bg-muted/50 px-2.5 py-1.5 text-xs"
              >
                <Paperclip className="h-3 w-3 text-muted-foreground" />
                <span className="max-w-[180px] truncate">{a.originalFileName}</span>
                <span className="text-muted-foreground">{formatBytes(a.sizeBytes)}</span>
                <button
                  onClick={() =>
                    setAttachments((prev) => prev.filter((x) => x.id !== a.id))
                  }
                  className="text-muted-foreground hover:text-foreground"
                >
                  <X className="h-3 w-3" />
                </button>
              </div>
            ))}
          </div>
        )}

        <div
          className={cn(
            'relative rounded-2xl border border-input bg-background shadow-sm',
            'focus-within:ring-2 focus-within:ring-ring focus-within:ring-offset-2 focus-within:ring-offset-background'
          )}
        >
          <textarea
            ref={ref}
            value={value}
            onChange={(e) => setValue(e.target.value)}
            onKeyDown={onKeyDown}
            placeholder="Ask Chima anything…"
            rows={1}
            className="w-full resize-none bg-transparent px-4 py-3 pr-24 text-sm outline-none placeholder:text-muted-foreground max-h-[240px]"
            disabled={disabled}
          />

          <div className="absolute right-2 bottom-2 flex items-center gap-1">
            <input
              ref={fileInput}
              type="file"
              accept=".txt,.md,.csv,.json,.pdf,.docx"
              onChange={pickFile}
              className="hidden"
            />

            <Button
              size="icon"
              variant="ghost"
              className="h-9 w-9 rounded-full"
              onClick={() => fileInput.current?.click()}
              disabled={uploading || disabled || isStreaming}
              aria-label="Attach file"
            >
              {uploading ? (
                <Loader2 className="h-4 w-4 animate-spin" />
              ) : (
                <Paperclip className="h-4 w-4" />
              )}
            </Button>

            {isStreaming ? (
              <Button
                size="icon"
                variant="secondary"
                className="h-9 w-9 rounded-full"
                onClick={onStop}
                aria-label="Stop generating"
              >
                <Square className="h-3.5 w-3.5" fill="currentColor" />
              </Button>
            ) : (
              <Button
                size="icon"
                className="h-9 w-9 rounded-full"
                onClick={submit}
                disabled={(!value.trim() && attachments.length === 0) || disabled}
                aria-label="Send"
              >
                <ArrowUp className="h-4 w-4" />
              </Button>
            )}
          </div>
        </div>

        <p className="mt-2 text-center text-xs text-muted-foreground">
          Chima can make mistakes. Check important information.
        </p>
      </div>
    </div>
  );
}