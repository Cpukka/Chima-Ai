export interface ApiResponse<T> {
  success: boolean;
  data: T | null;
  message: string | null;
  errors: { code: string; message: string; field?: string }[];
}

export interface PagedResult<T> {
  items: T[];
  nextCursor: string | null;
  hasMore: boolean;
  total?: number;
}

export interface UserDto {
  id: string;
  email: string;
  displayName: string | null;
  emailVerified: boolean;
  avatarUrl: string | null;
  isAdmin: boolean;
}

export interface AuthResponse {
  accessToken: string;
  refreshToken: string;
  expiresIn: number;
  user: UserDto;
}

export interface ConversationDto {
  id: string;
  title: string;
  isPinned: boolean;
  isArchived: boolean;
  deletedAt: string | null;
  createdAt: string;
  updatedAt: string;
  messageCount: number;
  lastMessagePreview: string | null;
  lastMessageAt: string | null;
}

export interface MessageDto {
  id: string;
  conversationId: string;
  role: 'user' | 'assistant' | 'system';
  status: 'pending' | 'streaming' | 'completed' | 'failed' | 'cancelled';
  content: string;
  parentMessageId: string | null;
  branchRootId: string | null;
  modelId: string | null;
  inputTokens: number | null;
  outputTokens: number | null;
  estimatedCost: number | null;
  durationMs: number | null;
  errorCode: string | null;
  errorMessage: string | null;
  createdAt: string;
  updatedAt: string;
}