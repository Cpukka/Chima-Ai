import type { Metadata } from 'next';
import { Suspense } from 'react';
import { ResetForm } from '@/components/auth/ResetForm';

export const metadata: Metadata = { title: 'Reset password' };

export default function ResetPasswordPage() {
  return (
    <Suspense fallback={null}>
      <ResetForm />
    </Suspense>
  );
}