'use client';

import ReactMarkdown from 'react-markdown';
import remarkGfm from 'remark-gfm';
import rehypeHighlight from 'rehype-highlight';
import 'highlight.js/styles/github-dark.css';
import { CodeBlock } from './CodeBlock';

interface Props {
  content: string;
}

export function MarkdownRenderer({ content }: Props) {
  return (
    <div className="markdown">
      <ReactMarkdown
        remarkPlugins={[remarkGfm]}
        rehypePlugins={[rehypeHighlight]}
        components={{
          // Custom code renderer: intercept <pre><code> for language + copy button
          pre({ children }) {
            return <>{children}</>;
          },
          code(props: React.HTMLAttributes<HTMLElement> & { inline?: boolean }) {
            const { inline, className, children, ...rest } = props as {
              inline?: boolean;
              className?: string;
              children?: React.ReactNode;
            } & React.HTMLAttributes<HTMLElement>;

            if (inline) {
              return <code {...rest}>{children}</code>;
            }

            const match = /language-(\w+)/.exec(className || '');
            const lang = match?.[1] ?? 'text';
            const code = String(children).replace(/\n$/, '');
            return <CodeBlock language={lang} code={code} />;
          }
        }}
      >
        {content}
      </ReactMarkdown>
    </div>
  );
}