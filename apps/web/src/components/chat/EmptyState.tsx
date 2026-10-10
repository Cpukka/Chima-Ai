'use client';

import { Sparkles, PenLine, Code2, BookOpen, Lightbulb } from 'lucide-react';

const PROMPTS = [
  { icon: BookOpen, label: 'Explain something to me', prompt: 'Explain how X works in simple terms.' },
  { icon: PenLine, label: 'Help me write', prompt: 'Help me draft a professional email about X.' },
  { icon: Code2, label: 'Help me code', prompt: 'Write a function that does X in TypeScript.' },
  { icon: Lightbulb, label: 'Brainstorm ideas', prompt: 'Give me 10 creative ideas for X.' }
];

interface Props {
  onPrompt: (prompt: string) => void;
}

export function EmptyState({ onPrompt }: Props) {
  return (
    <div className="flex-1 flex items-center justify-center px-4">
      <div className="max-w-2xl w-full text-center py-16">
        <div className="mx-auto h-14 w-14 rounded-2xl bg-primary/10 text-primary flex items-center justify-center mb-5">
          <Sparkles className="h-6 w-6" />
        </div>
        <h1 className="text-3xl md:text-4xl font-semibold tracking-tight">
          How can I help you today?
        </h1>
        <p className="mt-3 text-muted-foreground">
          Ask anything. Chima is here to write, code, explain, and think with you.
        </p>

        <div className="mt-10 grid gap-3 sm:grid-cols-2">
          {PROMPTS.map((p) => {
            const Icon = p.icon;
            return (
              <button
                key={p.label}
                onClick={() => onPrompt(p.prompt)}
                className="flex items-center gap-3 rounded-xl border border-border/60 bg-card p-4 text-left hover:border-primary/40 hover:bg-accent/50 transition-colors"
              >
                <div className="h-8 w-8 rounded-lg bg-primary/10 text-primary flex items-center justify-center shrink-0">
                  <Icon className="h-4 w-4" />
                </div>
                <span className="text-sm">{p.label}</span>
              </button>
            );
          })}
        </div>
      </div>
    </div>
  );
}