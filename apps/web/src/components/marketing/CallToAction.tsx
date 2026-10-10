import Link from 'next/link';
import { ArrowRight } from 'lucide-react';
import { Button } from '@/components/ui/button';

export function CallToAction() {
  return (
    <section className="py-20 md:py-28">
      <div className="container">
        <div className="rounded-2xl border border-border/60 bg-gradient-to-br from-primary/10 via-background to-indigo-500/5 p-10 md:p-16 text-center">
          <h2 className="text-3xl md:text-4xl font-semibold tracking-tight max-w-2xl mx-auto">
            Ready to try Chima?
          </h2>
          <p className="mt-4 text-muted-foreground max-w-xl mx-auto">
            Start free. Upgrade only when you need higher limits and premium models.
          </p>
          <div className="mt-8 flex flex-col sm:flex-row gap-3 justify-center">
            <Button size="lg" asChild>
              <Link href="/register">
                Create your account
                <ArrowRight className="ml-2 h-4 w-4" />
              </Link>
            </Button>
            <Button size="lg" variant="outline" asChild>
              <Link href="/pricing">Compare plans</Link>
            </Button>
          </div>
        </div>
      </div>
    </section>
  );
}