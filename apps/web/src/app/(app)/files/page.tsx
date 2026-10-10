'use client';

import { useRef, useState } from 'react';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { toast } from 'sonner';
import {
  FileText,
  Upload,
  Trash2,
  Loader2,
  CheckCircle2,
  AlertCircle,
  Clock
} from 'lucide-react';
import { Button } from '@/components/ui/button';
import { filesApi, formatBytes, type UploadedFile } from '@/lib/api/files';
import { cn } from '@/lib/utils/cn';

const ACCEPT = '.txt,.md,.csv,.json,.pdf,.docx';

export default function FilesPage() {
  const qc = useQueryClient();
  const fileInput = useRef<HTMLInputElement>(null);
  const [uploading, setUploading] = useState(false);

  const files = useQuery({
    queryKey: ['files'],
    queryFn: filesApi.list,
    refetchInterval: 3000 // poll while files are processing
  });

  const remove = useMutation({
    mutationFn: filesApi.delete,
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: ['files'] });
      toast.success('File deleted');
    },
    onError: (e) => toast.error(e instanceof Error ? e.message : 'Delete failed')
  });

  async function onFileChosen(e: React.ChangeEvent<HTMLInputElement>) {
    const f = e.target.files?.[0];
    if (!f) return;
    setUploading(true);
    try {
      await filesApi.upload(f);
      qc.invalidateQueries({ queryKey: ['files'] });
      toast.success(`Uploaded ${f.name}`);
    } catch (err) {
      toast.error(err instanceof Error ? err.message : 'Upload failed');
    } finally {
      setUploading(false);
      if (fileInput.current) fileInput.current.value = '';
    }
  }

  return (
    <div className="flex flex-col h-full pt-14 md:pt-0 overflow-y-auto">
      <header className="h-14 border-b border-border/40 flex items-center justify-between px-6 shrink-0">
        <h1 className="text-sm font-medium">Files</h1>
        <input
          ref={fileInput}
          type="file"
          accept={ACCEPT}
          onChange={onFileChosen}
          className="hidden"
        />
        <Button
          size="sm"
          onClick={() => fileInput.current?.click()}
          disabled={uploading}
        >
          {uploading ? (
            <>
              <Loader2 className="mr-2 h-4 w-4 animate-spin" /> Uploading…
            </>
          ) : (
            <>
              <Upload className="mr-2 h-4 w-4" /> Upload
            </>
          )}
        </Button>
      </header>

      <div className="max-w-4xl mx-auto w-full px-6 py-8">
        {files.isLoading && (
          <div className="flex items-center justify-center py-20">
            <Loader2 className="h-6 w-6 animate-spin text-muted-foreground" />
          </div>
        )}

        {!files.isLoading && (files.data ?? []).length === 0 && (
          <div className="text-center py-20">
            <div className="mx-auto h-14 w-14 rounded-2xl bg-primary/10 text-primary flex items-center justify-center mb-4">
              <FileText className="h-6 w-6" />
            </div>
            <h2 className="text-lg font-semibold">No files yet</h2>
            <p className="mt-2 text-sm text-muted-foreground">
              Upload a PDF, DOCX, or text file to ask questions about it.
            </p>
            <Button className="mt-6" onClick={() => fileInput.current?.click()}>
              <Upload className="mr-2 h-4 w-4" /> Upload your first file
            </Button>
          </div>
        )}

        {!files.isLoading && (files.data ?? []).length > 0 && (
          <div className="space-y-3">
            {(files.data ?? []).map((f) => (
              <FileRow
                key={f.id}
                file={f}
                onDelete={() => remove.mutate(f.id)}
              />
            ))}
          </div>
        )}
      </div>
    </div>
  );
}

function FileRow({ file, onDelete }: { file: UploadedFile; onDelete: () => void }) {
  const status = file.processingStatus;
  const isReady = status === 'ready';
  const isFailed = status === 'failed';
  const isProcessing = !isReady && !isFailed;

  return (
    <div
      className={cn(
        'flex items-center gap-4 rounded-xl border bg-card p-4',
        isFailed && 'border-destructive/40'
      )}
    >
      <div
        className={cn(
          'h-10 w-10 rounded-lg flex items-center justify-center shrink-0',
          isReady && 'bg-primary/10 text-primary',
          isProcessing && 'bg-amber-500/10 text-amber-600',
          isFailed && 'bg-destructive/10 text-destructive'
        )}
      >
        <FileText className="h-5 w-5" />
      </div>

      <div className="flex-1 min-w-0">
        <div className="font-medium truncate">{file.originalFileName}</div>
        <div className="flex items-center gap-3 mt-1 text-xs text-muted-foreground">
          <span>{formatBytes(file.sizeBytes)}</span>
          {file.chunkCount != null && <span>{file.chunkCount} chunks</span>}
          {file.pageCount != null && <span>{file.pageCount} pages</span>}
          {file.extractedCharCount != null && (
            <span>{file.extractedCharCount.toLocaleString()} chars</span>
          )}
        </div>
        {isFailed && file.processingError && (
          <div className="text-xs text-destructive mt-1 flex items-start gap-1">
            <AlertCircle className="h-3 w-3 mt-0.5 shrink-0" />
            <span>{file.processingError}</span>
          </div>
        )}
      </div>

      <div className="shrink-0">
        {isReady && (
          <span className="inline-flex items-center gap-1 text-xs text-primary">
            <CheckCircle2 className="h-3.5 w-3.5" /> Ready
          </span>
        )}
        {isProcessing && (
          <span className="inline-flex items-center gap-1 text-xs text-amber-600">
            <Clock className="h-3.5 w-3.5 animate-pulse" />
            {status === 'pending' && 'Queued'}
            {status === 'extracting' && 'Extracting'}
            {status === 'chunking' && 'Chunking'}
            {status === 'embedding' && 'Embedding'}
          </span>
        )}
        {isFailed && (
          <span className="inline-flex items-center gap-1 text-xs text-destructive">
            <AlertCircle className="h-3.5 w-3.5" /> Failed
          </span>
        )}
      </div>

      <Button
        variant="ghost"
        size="icon"
        className="text-muted-foreground hover:text-destructive shrink-0"
        onClick={onDelete}
      >
        <Trash2 className="h-4 w-4" />
      </Button>
    </div>
  );
}