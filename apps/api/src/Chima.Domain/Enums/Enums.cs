namespace Chima.Domain.Enums;

public enum UserStatus { Active = 0, Suspended = 1, Deleted = 2 }
public enum MessageRole { System = 0, User = 1, Assistant = 2 }
public enum MessageStatus { Pending = 0, Streaming = 1, Completed = 2, Failed = 3, Cancelled = 4 }
public enum SubscriptionStatus { Active = 0, Trialing = 1, PastDue = 2, Cancelled = 3, Expired = 4, Refunded = 5 }
public enum PaymentStatus { Pending = 0, Succeeded = 1, Failed = 2, Refunded = 3, Disputed = 4 }
public enum PaymentProvider { GooglePlay = 0, Stripe = 1, Paystack = 2, Manual = 99 }
public enum CreditTransactionType { Grant = 0, Purchase = 1, Consume = 2, Refund = 3, Expire = 4 }
public enum FileProcessingStatus { Pending = 0, Extracting = 1, Chunking = 2, Embedding = 3, Ready = 4, Failed = 5 }
public enum NotificationType { System = 0, Subscription = 1, UsageWarning = 2, SecurityAlert = 3, Announcement = 4 }
public enum AdminRole { SuperAdmin = 0, Admin = 1, Support = 2, Analyst = 3 }
public enum ModelCapability { Text = 0, Vision = 1, FileAnalysis = 2, Reasoning = 3, ImageGeneration = 4, Embeddings = 5 }
public enum ReportStatus { Open = 0, InReview = 1, Resolved = 2, Dismissed = 3 }
