import type { Metadata } from 'next';
import { Hero } from '@/components/marketing/Hero';
import { Features } from '@/components/marketing/Features';
import { HowItWorks } from '@/components/marketing/HowItWorks';
import { UseCases } from '@/components/marketing/UseCases';
import { Security } from '@/components/marketing/Security';
import { FAQ } from '@/components/marketing/FAQ';
import { CallToAction } from '@/components/marketing/CallToAction';

export const metadata: Metadata = {
  title: 'Chima — The AI assistant for real work',
  description:
    'Write, code, learn, and analyze documents with a fast, private AI assistant. Free to start.'
};

export default function LandingPage() {
  return (
    <>
      <Hero />
      <Features />
      <HowItWorks />
      <UseCases />
      <Security />
      <FAQ />
      <CallToAction />
    </>
  );
}