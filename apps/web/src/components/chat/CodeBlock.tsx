'use client';

import { useState } from 'react';
import { Check, Copy } from 'lucide-react';
import { cn } from '@/lib/utils/cn';

interface Props {
  language: string;
  code: string;
}

export function CodeBlock({ language, code }: Props) {
  const [copied, setCopied] = useState(false);

  async function copy() {
    try {
      await navigator.clipboard.writeText(code);
      setCopied(true);
      setTimeout(() => setCopied(false), 1500);
    } catch {
      /* ignore */
    }
  }

  return (
    <div className="relative group rounded-lg overflow-hidden my-4 bg-[hsl(222,47%,8%)] text-[hsl(210,40%,96%)]">
      <div className="flex items-center justify-between px-3 py-1.5 border-b border-white/10 bg-black/20">
        <span className="text-xs text-white/60 font-mono">{language}</span>
        <button
          onClick={copy}
          className={cn(
            'flex items-center gap-1 text-xs px-2 py-1 rounded transition-colors',
            'text-white/70 hover:text-white hover:bg-white/10'
          )}
          aria-label="Copy code"
        >
          {copied ? <Check className="h-3 w-3" /> : <Copy className="h-3 w-3" />}
          {copied ? 'Copied' : 'Copy'}
        </button>
      </div>
      <pre className="p-4 overflow-x-auto text-[0.85rem] leading-relaxed">
        <code className={`language-${language}`}>{code}</code>
      </pre>
    </div>
  );
}