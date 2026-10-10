import { PenLine, Code2, BookOpen, Sparkles, FileText, Languages, Lightbulb, MessageSquare } from 'lucide-react';

const FEATURES = [
  { icon: PenLine, title: 'Writing', desc: 'Draft emails, essays, and posts that read naturally.' },
  { icon: Code2, title: 'Coding', desc: 'Explain, review, or generate code in any language.' },
  { icon: BookOpen, title: 'Learning', desc: 'Get clear explanations on any topic, at any depth.' },
  { icon: FileText, title: 'Document analysis', desc: 'Upload PDFs, DOCX, and more. Ask questions about them.' },
  { icon: Languages, title: 'Translation', desc: 'Translate between languages with tone and context.' },
  { icon: Lightbulb, title: 'Brainstorming', desc: 'Generate ideas, outlines, and creative directions.' },
  { icon: Sparkles, title: 'Summarization', desc: 'Turn long content into tight, accurate summaries.' },
  { icon: MessageSquare, title: 'General Q&A', desc: 'Ask anything. Get structured, useful answers.' }
];

export function Features() {
  return (
    <section id="features" className="py-20 md:py-28">
      <div className="container">
        <div className="max-w-2xl mx-auto text-center mb-14">
          <h2 className="text-3xl md:text-4xl font-semibold tracking-tight">
            One assistant. Every task.
          </h2>
          <p className="mt-4 text-muted-foreground">
            Chima is designed for real work — not just chat. Here&apos;s what you can do on day one.
          </p>
        </div>

        <div className="grid gap-6 sm:grid-cols-2 lg:grid-cols-4">
          {FEATURES.map((f) => {
            const Icon = f.icon;
            return (
              <div
                key={f.title}
                className="rounded-xl border border-border/60 bg-card p-6 hover:border-primary/40 hover:shadow-md transition-all"
              >
                <div className="h-10 w-10 rounded-lg bg-primary/10 text-primary flex items-center justify-center mb-4">
                  <Icon className="h-5 w-5" />
                </div>
                <h3 className="font-medium">{f.title}</h3>
                <p className="mt-1 text-sm text-muted-foreground leading-relaxed">{f.desc}</p>
              </div>
            );
          })}
        </div>
      </div>
    </section>
  );
}