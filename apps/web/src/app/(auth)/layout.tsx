import Link from 'next/link';
import type { ReactNode } from 'react';

export default function AuthLayout({ children }: { children: ReactNode }) {
  return (
    <div className="min-h-screen grid lg:grid-cols-2">
      <div className="hidden lg:flex flex-col justify-between p-10 bg-gradient-to-br from-primary/15 via-background to-indigo-500/10 border-r border-border/40">
        <Link href="/" className="flex items-center gap-2 font-semibold">
          <img src="/logo.svg" alt="Chima" className="h-7 w-7" />
          <span className="text-lg">Chima</span>
        </Link>
        <div>
          <h2 className="text-3xl font-semibold tracking-tight max-w-md leading-tight">
            Your AI assistant for real work.
          </h2>
          <p className="mt-4 text-muted-foreground max-w-md">
            Write, code, learn, and analyze documents — with a fast, private, always-available
            assistant.
          </p>
        </div>
        <p className="text-xs text-muted-foreground">
          © {new Date().getFullYear()} Chima. All rights reserved.
        </p>
      </div>

      <div className="flex items-center justify-center p-6">
        <div className="w-full max-w-md">{children}</div>
      </div>
    </div>
  );
}