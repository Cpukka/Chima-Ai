const CASES = [
  {
    tag: 'For students',
    title: 'Study smarter',
    desc: 'Ask for step-by-step explanations, generate practice questions, and summarize textbooks and lecture notes.'
  },
  {
    tag: 'For developers',
    title: 'Ship faster',
    desc: 'Explain unfamiliar code, review pull requests, generate tests, and translate between languages.'
  },
  {
    tag: 'For writers',
    title: 'Write better',
    desc: 'Draft, edit, and rephrase. Match tone, tighten structure, and get unstuck when you need a fresh angle.'
  },
  {
    tag: 'For teams',
    title: 'Work together',
    desc: 'Analyze documents, extract key points, and turn messy notes into clear summaries your team can use.'
  }
];

export function UseCases() {
  return (
    <section className="py-20 md:py-28">
      <div className="container">
        <div className="max-w-2xl mx-auto text-center mb-14">
          <h2 className="text-3xl md:text-4xl font-semibold tracking-tight">Built for real work</h2>
          <p className="mt-4 text-muted-foreground">
            Whatever you do, Chima fits into the way you already work.
          </p>
        </div>

        <div className="grid gap-6 md:grid-cols-2">
          {CASES.map((c) => (
            <div key={c.title} className="rounded-xl border border-border/60 bg-card p-8">
              <div className="text-xs uppercase tracking-wider text-primary font-medium">{c.tag}</div>
              <h3 className="mt-2 text-xl font-semibold">{c.title}</h3>
              <p className="mt-2 text-muted-foreground leading-relaxed">{c.desc}</p>
            </div>
          ))}
        </div>
      </div>
    </section>
  );
}