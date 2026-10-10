export const palette = {
  primary: '#8b5cf6',
  primaryForeground: '#ffffff',
  backgroundLight: '#ffffff',
  backgroundDark: '#0b0b12',
  surfaceLight: '#f7f7f8',
  surfaceDark: '#15151f',
  surfaceElevatedLight: '#ffffff',
  surfaceElevatedDark: '#1c1c27',
  borderLight: '#e5e7eb',
  borderDark: '#26262f',
  textLight: '#0f172a',
  textDark: '#f8fafc',
  textMutedLight: '#64748b',
  textMutedDark: '#94a3b8',
  destructive: '#ef4444',
  success: '#22c55e',
  warning: '#f59e0b'
} as const;

export type ThemeColors = {
  primary: string;
  primaryForeground: string;
  background: string;
  surface: string;
  surfaceElevated: string;
  border: string;
  text: string;
  textMuted: string;
  destructive: string;
  success: string;
  warning: string;
};

export const lightColors: ThemeColors = {
  primary: palette.primary,
  primaryForeground: palette.primaryForeground,
  background: palette.backgroundLight,
  surface: palette.surfaceLight,
  surfaceElevated: palette.surfaceElevatedLight,
  border: palette.borderLight,
  text: palette.textLight,
  textMuted: palette.textMutedLight,
  destructive: palette.destructive,
  success: palette.success,
  warning: palette.warning
};

export const darkColors: ThemeColors = {
  primary: palette.primary,
  primaryForeground: palette.primaryForeground,
  background: palette.backgroundDark,
  surface: palette.surfaceDark,
  surfaceElevated: palette.surfaceElevatedDark,
  border: palette.borderDark,
  text: palette.textDark,
  textMuted: palette.textMutedDark,
  destructive: palette.destructive,
  success: palette.success,
  warning: palette.warning
};