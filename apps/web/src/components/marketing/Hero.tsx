import Link from 'next/link';
import { ArrowRight, Sparkles } from 'lucide-react';
import { Button } from '@/components/ui/button';

export function Hero() {
  return (
    <section className="relative overflow-hidden">
      <div className="absolute inset-0 -z-10 bg-gradient-to-b from-primary/5 via-background to-background" />
      <div className="absolute top-0 left-1/2 -z-10 h-[500px] w-[900px] -translate-x-1/2 rounded-full bg-primary/20 blur-[120px] opacity-40" />

      <div className="container py-20 md:py-28 text-center">
        <div className="inline-flex items-center gap-2 rounded-full border border-border/60 bg-background px-3 py-1 text-xs text-muted-foreground mb-6">
          <Sparkles className="h-3.5 w-3.5 text-primary" />
          <span>Introducing Chima — your AI assistant</span>
        </div>

        <h1 className="text-4xl md:text-6xl font-semibold tracking-tight max-w-3xl mx-auto leading-[1.1]">
          An AI assistant that&apos;s fast,
          <span className="bg-gradient-to-r from-primary to-indigo-500 bg-clip-text text-transparent">
            {' '}private, and always on.
          </span>
        </h1>

        <p className="mt-6 text-lg text-muted-foreground max-w-2xl mx-auto">
          Write, code, learn, brainstorm, translate, and summarize. Chima handles every task in one
          clean interface — on the web and on Android.
        </p>

        <div className="mt-10 flex flex-col sm:flex-row gap-3 justify-center">
          <Button size="lg" asChild>
            <Link href="/register">
              Start for free
              <ArrowRight className="ml-2 h-4 w-4" />
            </Link>
          </Button>
          <Button size="lg" variant="outline" asChild>
            <Link href="/pricing">See pricing</Link>
          </Button>
        </div>

        <p className="mt-6 text-xs text-muted-foreground">
          No credit card required · 50 free messages every month
        </p>
      </div>
    </section>
  );
}