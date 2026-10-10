export interface ApiResponse<T> {
  success: boolean;
  data: T | null;
  message: string | null;
  errors: { code: string; message: string; field?: string }[];
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

export interface DashboardMetrics {
  totalUsers: number;
  newUsersInPeriod: number;
  activeUsersInPeriod: number;
  paidUsers: number;
  freeUsers: number;
  revenueInPeriod: number;
  monthlyRecurringRevenue: number;
  aiRequestsInPeriod: number;
  estimatedAiCostInPeriod: number;
  grossMarginInPeriod: number;
  grossMarginPercent: number;
  failedRequestsInPeriod: number;
  totalConversations: number;
  totalMessages: number;
  totalStorageBytes: number;
  newUsersSeries: TimeSeriesPoint[];
  revenueSeries: TimeSeriesPoint[];
  aiRequestsSeries: TimeSeriesPoint[];
}

export interface TimeSeriesPoint {
  date: string;
  value: number;
}