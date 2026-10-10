'use client';

import { useState } from 'react';
import { ChevronDown } from 'lucide-react';
import { cn } from '@/lib/utils/cn';

const QUESTIONS = [
  {
    q: 'Is Chima free to use?',
    a: 'Yes. The Free plan includes 50 messages per month, basic model access, and limited file uploads. No credit card is required.'
  },
  {
    q: 'Do you train on my conversations?',
    a: 'No. Your conversations are stored to give you history and continuity, and are never used to train models. You can delete them at any time.'
  },
  {
    q: 'Which AI models power Chima?',
    a: 'Chima uses multiple state-of-the-art models from leading providers. The default model is fast and accurate; paid plans unlock advanced models with higher limits.'
  },
  {
    q: 'Can I use Chima on my phone?',
    a: 'Yes. Chima is available as a responsive web app and as a native Android app (releasing soon). Both share the same account and history.'
  },
  {
    q: 'How do I cancel my subscription?',
    a: 'You can cancel any time from your Subscription page. Your plan remains active until the end of the current billing period, then reverts to Free.'
  },
  {
    q: 'Where is my data stored?',
    a: 'Data is stored in secure, ISO-certified cloud infrastructure. All traffic is encrypted in transit, and sensitive credentials are kept server-side only.'
  }
];

export function FAQ() {
  const [open, setOpen] = useState<number | null>(0);

  return (
    <section id="faq" className="py-20 md:py-28">
      <div className="container max-w-3xl">
        <div className="text-center mb-12">
          <h2 className="text-3xl md:text-4xl font-semibold tracking-tight">Frequently asked</h2>
          <p className="mt-4 text-muted-foreground">Answers to the questions we hear most.</p>
        </div>

        <div className="space-y-2">
          {QUESTIONS.map((item, i) => {
            const isOpen = open === i;
            return (
              <div key={item.q} className="rounded-lg border border-border/60 bg-card">
                <button
                  onClick={() => setOpen(isOpen ? null : i)}
                  className="w-full flex items-center justify-between p-5 text-left"
                  aria-expanded={isOpen}
                >
                  <span className="font-medium pr-4">{item.q}</span>
                  <ChevronDown
                    className={cn('h-4 w-4 shrink-0 text-muted-foreground transition-transform', isOpen && 'rotate-180')}
                  />
                </button>
                {isOpen && (
                  <div className="px-5 pb-5 text-sm text-muted-foreground leading-relaxed">{item.a}</div>
                )}
              </div>
            );
          })}
        </div>
      </div>
    </section>
  );
}