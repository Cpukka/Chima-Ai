import { format, formatDistanceToNow, isToday, isYesterday, subDays, isAfter } from 'date-fns';

export function formatRelative(date: string | Date): string {
  const d = typeof date === 'string' ? new Date(date) : date;
  return formatDistanceToNow(d, { addSuffix: true });
}

export function formatTime(date: string | Date): string {
  const d = typeof date === 'string' ? new Date(date) : date;
  return format(d, 'h:mm a');
}

export type DateGroup = 'Today' | 'Yesterday' | 'Previous 7 days' | 'Older';

export function groupByDate(date: string | Date): DateGroup {
  const d = typeof date === 'string' ? new Date(date) : date;
  if (isToday(d)) return 'Today';
  if (isYesterday(d)) return 'Yesterday';
  if (isAfter(d, subDays(new Date(), 7))) return 'Previous 7 days';
  return 'Older';
}