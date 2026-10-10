import type { Metadata } from 'next';
import { PricingCards } from '@/components/marketing/PricingCards';

export const metadata: Metadata = {
  title: 'Pricing',
  description: 'Simple plans that scale with you. Start free.'
};

export default function PricingPage() {
  return (
    <section className="py-20 md:py-28">
      <div className="container">
        <div className="max-w-2xl mx-auto text-center mb-14">
          <h1 className="text-4xl md:text-5xl font-semibold tracking-tight">
            Simple, honest pricing
          </h1>
          <p className="mt-4 text-muted-foreground">
            Start free. Upgrade for higher limits and premium models. Cancel anytime.
          </p>
        </div>

        <PricingCards />

        <div className="mt-16 max-w-2xl mx-auto text-center text-sm text-muted-foreground">
          <p>
            All paid plans include a 7-day free trial when purchased through Google Play. Prices in
            USD. Taxes may apply depending on your region.
          </p>
        </div>
      </div>
    </section>
  );
}