export interface SseHandlers {
  onMeta?: (data: { userMessageId: string; assistantMessageId: string; conversationId: string }) => void;
  onDelta?: (data: { content: string }) => void;
  onDone?: (data: {
    inputTokens: number;
    outputTokens: number;
    estimatedCost: number;
    durationMs: number;
    finishReason: string;
  }) => void;
  onError?: (data: { code: string; message: string }) => void;
}

export function streamMessage(
  url: string,
  body: { content: string; modelId?: string },
  token: string,
  handlers: SseHandlers
): AbortController {
  const controller = new AbortController();

  (async () => {
    try {
      const res = await fetch(url, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
          Authorization: `Bearer ${token}`,
          Accept: 'text/event-stream'
        },
        body: JSON.stringify(body),
        signal: controller.signal
      });

      if (!res.ok || !res.body) {
        const text = await res.text().catch(() => '');
        handlers.onError?.({ code: 'stream_failed', message: text || `HTTP ${res.status}` });
        return;
      }

      const reader = res.body.getReader();
      const decoder = new TextDecoder();
      let buffer = '';

      while (true) {
        const { done, value } = await reader.read();
        if (done) break;

        buffer += decoder.decode(value, { stream: true });

        let idx: number;
        while ((idx = buffer.indexOf('\n\n')) !== -1) {
          const frame = buffer.slice(0, idx);
          buffer = buffer.slice(idx + 2);

          let eventName = 'message';
          let dataLine = '';
          for (const raw of frame.split('\n')) {
            const line = raw.replace(/\r$/, '');
            if (line.startsWith('event: ')) eventName = line.slice(7).trim();
            else if (line.startsWith('data: ')) dataLine += line.slice(6);
          }
          if (!dataLine) continue;

          let payload: unknown;
          try {
            payload = JSON.parse(dataLine);
          } catch {
            continue;
          }

          switch (eventName) {
            case 'meta':
              handlers.onMeta?.(payload as Parameters<NonNullable<SseHandlers['onMeta']>>[0]);
              break;
            case 'delta':
              handlers.onDelta?.(payload as { content: string });
              break;
            case 'done':
              handlers.onDone?.(payload as Parameters<NonNullable<SseHandlers['onDone']>>[0]);
              break;
            case 'error':
              handlers.onError?.(payload as { code: string; message: string });
              break;
          }
        }
      }
    } catch (err) {
      if ((err as { name?: string }).name === 'AbortError') return;
      handlers.onError?.({
        code: 'stream_network_error',
        message: (err as Error).message || 'Stream failed'
      });
    }
  })();

  return controller;
}