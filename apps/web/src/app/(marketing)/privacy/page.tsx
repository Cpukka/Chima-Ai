import type { Metadata } from 'next';

export const metadata: Metadata = { title: 'Privacy Policy' };

export default function PrivacyPage() {
  return (
    <section className="py-20 md:py-28">
      <div className="container max-w-3xl">
        <h1 className="text-4xl font-semibold tracking-tight">Privacy Policy</h1>
        <p className="mt-2 text-sm text-muted-foreground">Last updated: [date to be set]</p>

        <div className="mt-10 space-y-6 text-sm leading-relaxed text-muted-foreground">
          <div className="rounded-lg border border-amber-500/40 bg-amber-500/5 p-4 text-amber-700 dark:text-amber-300 text-sm">
            <strong>Placeholder notice:</strong> This document is a template. It must be reviewed by
            a qualified lawyer before public release. Replace all bracketed text with real values.
          </div>

          <h2 className="text-lg font-semibold text-foreground">1. Who we are</h2>
          <p>
            Chima ("we", "us") operates the Chima AI assistant. This policy explains what data we
            collect, how we use it, and your rights. [Company legal name, address, and jurisdiction
            to be filled in.]
          </p>

          <h2 className="text-lg font-semibold text-foreground">2. What we collect</h2>
          <p>
            Account information (email, password hash, display name), conversations you create, files
            you upload, and metadata such as your IP address for security purposes.
          </p>

          <h2 className="text-lg font-semibold text-foreground">3. How we use it</h2>
          <p>
            To operate the service, provide AI responses, prevent abuse, and improve reliability. We
            do not sell your data. We do not use your conversations to train AI models.
          </p>

          <h2 className="text-lg font-semibold text-foreground">4. Sharing</h2>
          <p>
            We share data with service providers that help us operate the platform (hosting, AI
            inference, payments, email). These providers are contractually bound to use data only for
            the services they provide to us.
          </p>

          <h2 className="text-lg font-semibold text-foreground">5. Your rights</h2>
          <p>
            You can access, export, correct, or delete your data at any time from your account
            settings. For requests that require assistance, contact privacy@chima.app.
          </p>

          <h2 className="text-lg font-semibold text-foreground">6. Retention</h2>
          <p>
            We retain your data while your account is active. When you delete your account, we delete
            personal data unless the law requires us to retain it (for example, payment records for
            tax purposes).
          </p>

          <h2 className="text-lg font-semibold text-foreground">7. Contact</h2>
          <p>privacy@chima.app</p>
        </div>
      </div>
    </section>
  );
}