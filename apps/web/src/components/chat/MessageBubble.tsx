'use client';

import { useState } from 'react';
import { Check, Copy, RefreshCw, AlertTriangle } from 'lucide-react';
import { Avatar, AvatarFallback, AvatarImage } from '@/components/ui/avatar';
import { Button } from '@/components/ui/button';
import { cn } from '@/lib/utils/cn';
import { MarkdownRenderer } from './MarkdownRenderer';
import type { MessageDto, UserDto } from '@/lib/api/types';

interface Props {
  message: MessageDto;
  user: UserDto | null;
  onRegenerate?: (messageId: string) => void;
  showActions?: boolean;
}

export function MessageBubble({ message, user, onRegenerate, showActions = true }: Props) {
  const [copied, setCopied] = useState(false);
  const isUser = message.role === 'user';
  const isStreaming = message.status === 'streaming' || message.status === 'pending';
  const isFailed = message.status === 'failed';

  async function copy() {
    try {
      await navigator.clipboard.writeText(message.content);
      setCopied(true);
      setTimeout(() => setCopied(false), 1500);
    } catch {
      /* ignore */
    }
  }

  const userInitials =
    user?.displayName?.slice(0, 1).toUpperCase() || user?.email.slice(0, 1).toUpperCase() || 'U';

  return (
    <div className={cn('group px-4 md:px-8 py-5', !isUser && 'bg-muted/20')}>
      <div className="max-w-3xl mx-auto flex gap-4">
        <Avatar className="h-8 w-8 mt-0.5 shrink-0">
          {isUser ? (
            <>
              {user?.avatarUrl && <AvatarImage src={user.avatarUrl} alt="" />}
              <AvatarFallback>{userInitials}</AvatarFallback>
            </>
          ) : (
            <AvatarFallback className="bg-primary/15 text-primary">
              <img src="/logo.svg" alt="" className="h-4 w-4" />
            </AvatarFallback>
          )}
        </Avatar>

        <div className="flex-1 min-w-0">
          <div className="flex items-center gap-2 mb-1">
            <span className="text-sm font-medium">{isUser ? 'You' : 'Chima'}</span>
            {isStreaming && (
              <span className="flex items-center gap-1 text-xs text-muted-foreground">
                <span className="inline-flex gap-0.5">
                  <span className="w-1 h-1 rounded-full bg-current animate-pulse-dot" />
                  <span className="w-1 h-1 rounded-full bg-current animate-pulse-dot [animation-delay:0.2s]" />
                  <span className="w-1 h-1 rounded-full bg-current animate-pulse-dot [animation-delay:0.4s]" />
                </span>
                {message.status === 'pending' ? 'Thinking' : 'Writing'}
              </span>
            )}
          </div>

          <div className="text-[0.9375rem] leading-relaxed">
            {isUser ? (
              <p className="whitespace-pre-wrap break-words">{message.content}</p>
            ) : (
              <>
                {message.content ? (
                  <MarkdownRenderer content={message.content} />
                ) : isStreaming ? (
                  <span className="text-muted-foreground text-sm">…</span>
                ) : null}
              </>
            )}
          </div>

          {isFailed && (
            <div className="mt-2 flex items-start gap-2 rounded-lg border border-destructive/40 bg-destructive/5 p-3 text-sm">
              <AlertTriangle className="h-4 w-4 text-destructive shrink-0 mt-0.5" />
              <div className="flex-1">
                <div className="font-medium text-destructive">Response failed</div>
                <div className="text-muted-foreground text-xs mt-0.5">
                  {message.errorMessage ?? 'Something went wrong.'}
                </div>
              </div>
              {onRegenerate && (
                <Button
                  size="sm"
                  variant="outline"
                  onClick={() => onRegenerate(message.id)}
                >
                  Retry
                </Button>
              )}
            </div>
          )}

          {showActions && !isStreaming && !isFailed && (
            <div className="mt-2 flex items-center gap-1 opacity-0 group-hover:opacity-100 transition-opacity">
              <Button
                variant="ghost"
                size="sm"
                className="h-7 px-2 text-xs text-muted-foreground"
                onClick={copy}
              >
                {copied ? <Check className="h-3 w-3 mr-1" /> : <Copy className="h-3 w-3 mr-1" />}
                {copied ? 'Copied' : 'Copy'}
              </Button>

              {!isUser && onRegenerate && (
                <Button
                  variant="ghost"
                  size="sm"
                  className="h-7 px-2 text-xs text-muted-foreground"
                  onClick={() => onRegenerate(message.id)}
                >
                  <RefreshCw className="h-3 w-3 mr-1" />
                  Regenerate
                </Button>
              )}
            </div>
          )}
        </div>
      </div>
    </div>
  );
}