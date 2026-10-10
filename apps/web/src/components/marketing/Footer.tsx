import Link from 'next/link';

const SECTIONS = [
  {
    title: 'Product',
    links: [
      { href: '/#features', label: 'Features' },
      { href: '/pricing', label: 'Pricing' },
      { href: '/help', label: 'Help' }
    ]
  },
  {
    title: 'Company',
    links: [
      { href: '/help', label: 'Contact' },
      { href: '/#faq', label: 'FAQ' }
    ]
  },
  {
    title: 'Legal',
    links: [
      { href: '/privacy', label: 'Privacy' },
      { href: '/terms', label: 'Terms' }
    ]
  }
];

export function Footer() {
  return (
    <footer className="border-t border-border/40 bg-muted/30">
      <div className="container py-12">
        <div className="grid gap-10 md:grid-cols-4">
          <div>
            <Link href="/" className="flex items-center gap-2 font-semibold">
              <img src="/logo.svg" alt="Chima" className="h-7 w-7" />
              <span className="text-lg">Chima</span>
            </Link>
            <p className="mt-3 text-sm text-muted-foreground max-w-xs">
              A fast, private AI assistant for writing, coding, learning, and thinking.
            </p>
          </div>

          {SECTIONS.map((section) => (
            <div key={section.title}>
              <h4 className="text-sm font-semibold mb-3">{section.title}</h4>
              <ul className="space-y-2">
                {section.links.map((l) => (
                  <li key={l.href}>
                    <Link
                      href={l.href}
                      className="text-sm text-muted-foreground hover:text-foreground transition-colors"
                    >
                      {l.label}
                    </Link>
                  </li>
                ))}
              </ul>
            </div>
          ))}
        </div>

        <div className="mt-10 pt-6 border-t border-border/40 flex flex-col md:flex-row gap-4 items-center justify-between text-xs text-muted-foreground">
          <p>© {new Date().getFullYear()} Chima. All rights reserved.</p>
          <p>Chima is an AI assistant. It is not a human.</p>
        </div>
      </div>
    </footer>
  );
}