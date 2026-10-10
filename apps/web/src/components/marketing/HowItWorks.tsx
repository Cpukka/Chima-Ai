const STEPS = [
  { n: '01', title: 'Create an account', desc: 'Email or Google. Takes seconds, no credit card.' },
  { n: '02', title: 'Start a conversation', desc: 'Ask anything, or upload a document to work with.' },
  { n: '03', title: 'Get real answers', desc: 'Chima streams its response as it thinks — no waiting.' },
  { n: '04', title: 'Organize and come back', desc: 'Every conversation is saved, searchable, and private.' }
];

export function HowItWorks() {
  return (
    <section id="how" className="py-20 md:py-28 bg-muted/30 border-y border-border/40">
      <div className="container">
        <div className="max-w-2xl mx-auto text-center mb-14">
          <h2 className="text-3xl md:text-4xl font-semibold tracking-tight">How it works</h2>
          <p className="mt-4 text-muted-foreground">
            From signing up to your first useful answer in under a minute.
          </p>
        </div>

        <div className="grid gap-6 md:grid-cols-4">
          {STEPS.map((s) => (
            <div key={s.n} className="relative">
              <div className="text-4xl font-semibold text-primary/30">{s.n}</div>
              <h3 className="mt-3 font-medium">{s.title}</h3>
              <p className="mt-1 text-sm text-muted-foreground">{s.desc}</p>
            </div>
          ))}
        </div>
      </div>
    </section>
  );
}