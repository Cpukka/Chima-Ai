import type { Metadata } from 'next';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { Mail, BookOpen, MessageCircle } from 'lucide-react';

export const metadata: Metadata = { title: 'Help' };

const TOPICS = [
  {
    icon: BookOpen,
    title: 'Getting started',
    body: 'Create an account, start a conversation, and try a few prompts. Chima works best with clear questions.'
  },
  {
    icon: MessageCircle,
    title: 'Using conversations',
    body: 'Rename, pin, archive, and search conversations. Every response can be copied, regenerated, or stopped mid-stream.'
  },
  {
    icon: Mail,
    title: 'Contact support',
    body: 'Email support@chima.app and we will respond within one business day. Include your account email for faster help.'
  }
];

export default function HelpPage() {
  return (
    <section className="py-20 md:py-28">
      <div className="container max-w-3xl">
        <h1 className="text-4xl font-semibold tracking-tight text-center">Help center</h1>
        <p className="mt-4 text-muted-foreground text-center">
          Short answers to common questions. Need more? Email us.
        </p>

        <div className="mt-12 grid gap-6">
          {TOPICS.map((t) => {
            const Icon = t.icon;
            return (
              <Card key={t.title}>
                <CardHeader className="flex-row items-center gap-3 space-y-0">
                  <div className="h-9 w-9 rounded-lg bg-primary/10 text-primary flex items-center justify-center">
                    <Icon className="h-4 w-4" />
                  </div>
                  <CardTitle>{t.title}</CardTitle>
                </CardHeader>
                <CardContent>
                  <p className="text-sm text-muted-foreground leading-relaxed">{t.body}</p>
                </CardContent>
              </Card>
            );
          })}
        </div>
      </div>
    </section>
  );
}