import { Shield, Lock, ServerCog, FileCheck } from 'lucide-react';

const ITEMS = [
  { icon: Lock, title: 'Encrypted in transit', desc: 'All traffic over HTTPS with modern TLS.' },
  { icon: Shield, title: 'Password hashing', desc: 'BCrypt with strong work factors. Plaintext is never stored.' },
  { icon: ServerCog, title: 'Server-side secrets', desc: 'API keys and provider credentials never reach the browser.' },
  { icon: FileCheck, title: 'Your data, your control', desc: 'Export, delete, or restrict your data at any time.' }
];

export function Security() {
  return (
    <section className="py-20 md:py-28 bg-muted/30 border-y border-border/40">
      <div className="container">
        <div className="max-w-2xl mx-auto text-center mb-14">
          <h2 className="text-3xl md:text-4xl font-semibold tracking-tight">Security first</h2>
          <p className="mt-4 text-muted-foreground">
            Chima is engineered to protect your account and your conversations.
          </p>
        </div>

        <div className="grid gap-6 md:grid-cols-2 lg:grid-cols-4">
          {ITEMS.map((i) => {
            const Icon = i.icon;
            return (
              <div key={i.title} className="text-center">
                <div className="mx-auto h-12 w-12 rounded-full bg-primary/10 text-primary flex items-center justify-center mb-4">
                  <Icon className="h-5 w-5" />
                </div>
                <h3 className="font-medium">{i.title}</h3>
                <p className="mt-2 text-sm text-muted-foreground">{i.desc}</p>
              </div>
            );
          })}
        </div>
      </div>
    </section>
  );
}