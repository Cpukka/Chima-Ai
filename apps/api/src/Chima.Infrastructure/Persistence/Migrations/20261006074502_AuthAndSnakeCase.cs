using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Chima.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AuthAndSnakeCase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_admin_users_users_UserId",
                table: "admin_users");

            migrationBuilder.DropForeignKey(
                name: "FK_ai_models_ai_providers_ProviderId",
                table: "ai_models");

            migrationBuilder.DropForeignKey(
                name: "FK_conversations_ai_models_LastModelId",
                table: "conversations");

            migrationBuilder.DropForeignKey(
                name: "FK_conversations_users_UserId",
                table: "conversations");

            migrationBuilder.DropForeignKey(
                name: "FK_credit_transactions_users_UserId",
                table: "credit_transactions");

            migrationBuilder.DropForeignKey(
                name: "FK_device_tokens_users_UserId",
                table: "device_tokens");

            migrationBuilder.DropForeignKey(
                name: "FK_file_chunks_uploaded_files_FileId",
                table: "file_chunks");

            migrationBuilder.DropForeignKey(
                name: "FK_message_attachments_messages_MessageId",
                table: "message_attachments");

            migrationBuilder.DropForeignKey(
                name: "FK_message_attachments_uploaded_files_UploadedFileId",
                table: "message_attachments");

            migrationBuilder.DropForeignKey(
                name: "FK_messages_ai_models_ModelId",
                table: "messages");

            migrationBuilder.DropForeignKey(
                name: "FK_messages_conversations_ConversationId",
                table: "messages");

            migrationBuilder.DropForeignKey(
                name: "FK_messages_messages_ParentMessageId",
                table: "messages");

            migrationBuilder.DropForeignKey(
                name: "FK_notifications_users_UserId",
                table: "notifications");

            migrationBuilder.DropForeignKey(
                name: "FK_payments_user_subscriptions_SubscriptionId",
                table: "payments");

            migrationBuilder.DropForeignKey(
                name: "FK_payments_users_UserId",
                table: "payments");

            migrationBuilder.DropForeignKey(
                name: "FK_refresh_tokens_users_UserId",
                table: "refresh_tokens");

            migrationBuilder.DropForeignKey(
                name: "FK_uploaded_files_users_UserId",
                table: "uploaded_files");

            migrationBuilder.DropForeignKey(
                name: "FK_usage_limits_subscription_plans_PlanId",
                table: "usage_limits");

            migrationBuilder.DropForeignKey(
                name: "FK_usage_records_ai_models_ModelId",
                table: "usage_records");

            migrationBuilder.DropForeignKey(
                name: "FK_usage_records_users_UserId",
                table: "usage_records");

            migrationBuilder.DropForeignKey(
                name: "FK_user_profiles_users_UserId",
                table: "user_profiles");

            migrationBuilder.DropForeignKey(
                name: "FK_user_subscriptions_subscription_plans_PlanId",
                table: "user_subscriptions");

            migrationBuilder.DropForeignKey(
                name: "FK_user_subscriptions_users_UserId",
                table: "user_subscriptions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_users",
                table: "users");

            migrationBuilder.DropPrimaryKey(
                name: "PK_user_subscriptions",
                table: "user_subscriptions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_user_profiles",
                table: "user_profiles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_usage_records",
                table: "usage_records");

            migrationBuilder.DropPrimaryKey(
                name: "PK_usage_limits",
                table: "usage_limits");

            migrationBuilder.DropPrimaryKey(
                name: "PK_uploaded_files",
                table: "uploaded_files");

            migrationBuilder.DropPrimaryKey(
                name: "PK_system_settings",
                table: "system_settings");

            migrationBuilder.DropPrimaryKey(
                name: "PK_subscription_plans",
                table: "subscription_plans");

            migrationBuilder.DropPrimaryKey(
                name: "PK_reports",
                table: "reports");

            migrationBuilder.DropPrimaryKey(
                name: "PK_refresh_tokens",
                table: "refresh_tokens");

            migrationBuilder.DropPrimaryKey(
                name: "PK_payments",
                table: "payments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_notifications",
                table: "notifications");

            migrationBuilder.DropPrimaryKey(
                name: "PK_messages",
                table: "messages");

            migrationBuilder.DropPrimaryKey(
                name: "PK_message_attachments",
                table: "message_attachments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_file_chunks",
                table: "file_chunks");

            migrationBuilder.DropPrimaryKey(
                name: "PK_feedback",
                table: "feedback");

            migrationBuilder.DropPrimaryKey(
                name: "PK_feature_flags",
                table: "feature_flags");

            migrationBuilder.DropPrimaryKey(
                name: "PK_device_tokens",
                table: "device_tokens");

            migrationBuilder.DropPrimaryKey(
                name: "PK_credit_transactions",
                table: "credit_transactions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_conversations",
                table: "conversations");

            migrationBuilder.DropPrimaryKey(
                name: "PK_audit_logs",
                table: "audit_logs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ai_providers",
                table: "ai_providers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ai_models",
                table: "ai_models");

            migrationBuilder.DropPrimaryKey(
                name: "PK_admin_users",
                table: "admin_users");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "users",
                newName: "status");

            migrationBuilder.RenameColumn(
                name: "Email",
                table: "users",
                newName: "email");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "users",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "users",
                newName: "updated_at");

            migrationBuilder.RenameColumn(
                name: "PasswordHash",
                table: "users",
                newName: "password_hash");

            migrationBuilder.RenameColumn(
                name: "LastLoginAt",
                table: "users",
                newName: "last_login_at");

            migrationBuilder.RenameColumn(
                name: "GoogleSubjectId",
                table: "users",
                newName: "google_subject_id");

            migrationBuilder.RenameColumn(
                name: "EmailVerifiedAt",
                table: "users",
                newName: "email_verified_at");

            migrationBuilder.RenameColumn(
                name: "EmailVerified",
                table: "users",
                newName: "email_verified");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "users",
                newName: "created_at");

            migrationBuilder.RenameIndex(
                name: "IX_users_GoogleSubjectId",
                table: "users",
                newName: "i_x_users_google_subject_id");

            migrationBuilder.RenameIndex(
                name: "IX_users_Email",
                table: "users",
                newName: "i_x_users_email");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "user_subscriptions",
                newName: "status");

            migrationBuilder.RenameColumn(
                name: "Provider",
                table: "user_subscriptions",
                newName: "provider");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "user_subscriptions",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "user_subscriptions",
                newName: "user_id");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "user_subscriptions",
                newName: "updated_at");

            migrationBuilder.RenameColumn(
                name: "PlanId",
                table: "user_subscriptions",
                newName: "plan_id");

            migrationBuilder.RenameColumn(
                name: "GracePeriodEndsAt",
                table: "user_subscriptions",
                newName: "grace_period_ends_at");

            migrationBuilder.RenameColumn(
                name: "ExternalSubscriptionId",
                table: "user_subscriptions",
                newName: "external_subscription_id");

            migrationBuilder.RenameColumn(
                name: "ExternalPurchaseToken",
                table: "user_subscriptions",
                newName: "external_purchase_token");

            migrationBuilder.RenameColumn(
                name: "EndedAt",
                table: "user_subscriptions",
                newName: "ended_at");

            migrationBuilder.RenameColumn(
                name: "CurrentPeriodStart",
                table: "user_subscriptions",
                newName: "current_period_start");

            migrationBuilder.RenameColumn(
                name: "CurrentPeriodEnd",
                table: "user_subscriptions",
                newName: "current_period_end");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "user_subscriptions",
                newName: "created_at");

            migrationBuilder.RenameColumn(
                name: "CancelledAt",
                table: "user_subscriptions",
                newName: "cancelled_at");

            migrationBuilder.RenameColumn(
                name: "AutoRenew",
                table: "user_subscriptions",
                newName: "auto_renew");

            migrationBuilder.RenameIndex(
                name: "IX_user_subscriptions_UserId",
                table: "user_subscriptions",
                newName: "i_x_user_subscriptions_user_id");

            migrationBuilder.RenameIndex(
                name: "IX_user_subscriptions_Status",
                table: "user_subscriptions",
                newName: "i_x_user_subscriptions_status");

            migrationBuilder.RenameIndex(
                name: "IX_user_subscriptions_PlanId",
                table: "user_subscriptions",
                newName: "i_x_user_subscriptions_plan_id");

            migrationBuilder.RenameIndex(
                name: "IX_user_subscriptions_ExternalSubscriptionId",
                table: "user_subscriptions",
                newName: "i_x_user_subscriptions_external_subscription_id");

            migrationBuilder.RenameColumn(
                name: "Theme",
                table: "user_profiles",
                newName: "theme");

            migrationBuilder.RenameColumn(
                name: "Language",
                table: "user_profiles",
                newName: "language");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "user_profiles",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "user_profiles",
                newName: "user_id");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "user_profiles",
                newName: "updated_at");

            migrationBuilder.RenameColumn(
                name: "TimeZone",
                table: "user_profiles",
                newName: "time_zone");

            migrationBuilder.RenameColumn(
                name: "NotifyUsageWarning",
                table: "user_profiles",
                newName: "notify_usage_warning");

            migrationBuilder.RenameColumn(
                name: "NotifySubscriptionRenewal",
                table: "user_profiles",
                newName: "notify_subscription_renewal");

            migrationBuilder.RenameColumn(
                name: "NotifySecurityAlert",
                table: "user_profiles",
                newName: "notify_security_alert");

            migrationBuilder.RenameColumn(
                name: "NotifyProductAnnouncements",
                table: "user_profiles",
                newName: "notify_product_announcements");

            migrationBuilder.RenameColumn(
                name: "DisplayName",
                table: "user_profiles",
                newName: "display_name");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "user_profiles",
                newName: "created_at");

            migrationBuilder.RenameColumn(
                name: "AvatarUrl",
                table: "user_profiles",
                newName: "avatar_url");

            migrationBuilder.RenameColumn(
                name: "AnalyticsOptIn",
                table: "user_profiles",
                newName: "analytics_opt_in");

            migrationBuilder.RenameIndex(
                name: "IX_user_profiles_UserId",
                table: "user_profiles",
                newName: "i_x_user_profiles_user_id");

            migrationBuilder.RenameColumn(
                name: "Success",
                table: "usage_records",
                newName: "success");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "usage_records",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "usage_records",
                newName: "user_id");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "usage_records",
                newName: "updated_at");

            migrationBuilder.RenameColumn(
                name: "TotalTokens",
                table: "usage_records",
                newName: "total_tokens");

            migrationBuilder.RenameColumn(
                name: "RequestId",
                table: "usage_records",
                newName: "request_id");

            migrationBuilder.RenameColumn(
                name: "OutputTokens",
                table: "usage_records",
                newName: "output_tokens");

            migrationBuilder.RenameColumn(
                name: "ModelId",
                table: "usage_records",
                newName: "model_id");

            migrationBuilder.RenameColumn(
                name: "MessageId",
                table: "usage_records",
                newName: "message_id");

            migrationBuilder.RenameColumn(
                name: "InputTokens",
                table: "usage_records",
                newName: "input_tokens");

            migrationBuilder.RenameColumn(
                name: "EstimatedCost",
                table: "usage_records",
                newName: "estimated_cost");

            migrationBuilder.RenameColumn(
                name: "ErrorCode",
                table: "usage_records",
                newName: "error_code");

            migrationBuilder.RenameColumn(
                name: "DurationMs",
                table: "usage_records",
                newName: "duration_ms");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "usage_records",
                newName: "created_at");

            migrationBuilder.RenameColumn(
                name: "ConversationId",
                table: "usage_records",
                newName: "conversation_id");

            migrationBuilder.RenameIndex(
                name: "IX_usage_records_UserId_CreatedAt",
                table: "usage_records",
                newName: "i_x_usage_records_user_id_created_at");

            migrationBuilder.RenameIndex(
                name: "IX_usage_records_RequestId",
                table: "usage_records",
                newName: "i_x_usage_records_request_id");

            migrationBuilder.RenameIndex(
                name: "IX_usage_records_ModelId",
                table: "usage_records",
                newName: "i_x_usage_records_model_id");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "usage_limits",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "usage_limits",
                newName: "updated_at");

            migrationBuilder.RenameColumn(
                name: "PlanId",
                table: "usage_limits",
                newName: "plan_id");

            migrationBuilder.RenameColumn(
                name: "MonthlyTokens",
                table: "usage_limits",
                newName: "monthly_tokens");

            migrationBuilder.RenameColumn(
                name: "MonthlyMessages",
                table: "usage_limits",
                newName: "monthly_messages");

            migrationBuilder.RenameColumn(
                name: "MonthlyFileUploads",
                table: "usage_limits",
                newName: "monthly_file_uploads");

            migrationBuilder.RenameColumn(
                name: "MonthlyCreditAllowance",
                table: "usage_limits",
                newName: "monthly_credit_allowance");

            migrationBuilder.RenameColumn(
                name: "MaxFileSizeMb",
                table: "usage_limits",
                newName: "max_file_size_mb");

            migrationBuilder.RenameColumn(
                name: "MaxConversations",
                table: "usage_limits",
                newName: "max_conversations");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "usage_limits",
                newName: "created_at");

            migrationBuilder.RenameIndex(
                name: "IX_usage_limits_PlanId",
                table: "usage_limits",
                newName: "i_x_usage_limits_plan_id");

            migrationBuilder.RenameColumn(
                name: "Sha256",
                table: "uploaded_files",
                newName: "sha256");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "uploaded_files",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "uploaded_files",
                newName: "user_id");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "uploaded_files",
                newName: "updated_at");

            migrationBuilder.RenameColumn(
                name: "StorageKey",
                table: "uploaded_files",
                newName: "storage_key");

            migrationBuilder.RenameColumn(
                name: "SizeBytes",
                table: "uploaded_files",
                newName: "size_bytes");

            migrationBuilder.RenameColumn(
                name: "ProcessingStatus",
                table: "uploaded_files",
                newName: "processing_status");

            migrationBuilder.RenameColumn(
                name: "ProcessingError",
                table: "uploaded_files",
                newName: "processing_error");

            migrationBuilder.RenameColumn(
                name: "ProcessedAt",
                table: "uploaded_files",
                newName: "processed_at");

            migrationBuilder.RenameColumn(
                name: "PageCount",
                table: "uploaded_files",
                newName: "page_count");

            migrationBuilder.RenameColumn(
                name: "OriginalFileName",
                table: "uploaded_files",
                newName: "original_file_name");

            migrationBuilder.RenameColumn(
                name: "ExtractedCharCount",
                table: "uploaded_files",
                newName: "extracted_char_count");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "uploaded_files",
                newName: "created_at");

            migrationBuilder.RenameColumn(
                name: "ContentType",
                table: "uploaded_files",
                newName: "content_type");

            migrationBuilder.RenameColumn(
                name: "ChunkCount",
                table: "uploaded_files",
                newName: "chunk_count");

            migrationBuilder.RenameIndex(
                name: "IX_uploaded_files_UserId",
                table: "uploaded_files",
                newName: "i_x_uploaded_files_user_id");

            migrationBuilder.RenameIndex(
                name: "IX_uploaded_files_ProcessingStatus",
                table: "uploaded_files",
                newName: "i_x_uploaded_files_processing_status");

            migrationBuilder.RenameColumn(
                name: "Value",
                table: "system_settings",
                newName: "value");

            migrationBuilder.RenameColumn(
                name: "Key",
                table: "system_settings",
                newName: "key");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "system_settings",
                newName: "description");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "system_settings",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "ValueType",
                table: "system_settings",
                newName: "value_type");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "system_settings",
                newName: "updated_at");

            migrationBuilder.RenameColumn(
                name: "IsSecret",
                table: "system_settings",
                newName: "is_secret");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "system_settings",
                newName: "created_at");

            migrationBuilder.RenameIndex(
                name: "IX_system_settings_Key",
                table: "system_settings",
                newName: "i_x_system_settings_key");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "subscription_plans",
                newName: "description");

            migrationBuilder.RenameColumn(
                name: "Currency",
                table: "subscription_plans",
                newName: "currency");

            migrationBuilder.RenameColumn(
                name: "Code",
                table: "subscription_plans",
                newName: "code");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "subscription_plans",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "YearlyPrice",
                table: "subscription_plans",
                newName: "yearly_price");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "subscription_plans",
                newName: "updated_at");

            migrationBuilder.RenameColumn(
                name: "StripeYearlyPriceId",
                table: "subscription_plans",
                newName: "stripe_yearly_price_id");

            migrationBuilder.RenameColumn(
                name: "StripeMonthlyPriceId",
                table: "subscription_plans",
                newName: "stripe_monthly_price_id");

            migrationBuilder.RenameColumn(
                name: "SortOrder",
                table: "subscription_plans",
                newName: "sort_order");

            migrationBuilder.RenameColumn(
                name: "PaystackYearlyPlanCode",
                table: "subscription_plans",
                newName: "paystack_yearly_plan_code");

            migrationBuilder.RenameColumn(
                name: "PaystackMonthlyPlanCode",
                table: "subscription_plans",
                newName: "paystack_monthly_plan_code");

            migrationBuilder.RenameColumn(
                name: "MonthlyPrice",
                table: "subscription_plans",
                newName: "monthly_price");

            migrationBuilder.RenameColumn(
                name: "IsDefault",
                table: "subscription_plans",
                newName: "is_default");

            migrationBuilder.RenameColumn(
                name: "IsActive",
                table: "subscription_plans",
                newName: "is_active");

            migrationBuilder.RenameColumn(
                name: "GooglePlayYearlyProductId",
                table: "subscription_plans",
                newName: "google_play_yearly_product_id");

            migrationBuilder.RenameColumn(
                name: "GooglePlayMonthlyProductId",
                table: "subscription_plans",
                newName: "google_play_monthly_product_id");

            migrationBuilder.RenameColumn(
                name: "DisplayName",
                table: "subscription_plans",
                newName: "display_name");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "subscription_plans",
                newName: "created_at");

            migrationBuilder.RenameIndex(
                name: "IX_subscription_plans_Code",
                table: "subscription_plans",
                newName: "i_x_subscription_plans_code");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "reports",
                newName: "status");

            migrationBuilder.RenameColumn(
                name: "Resolution",
                table: "reports",
                newName: "resolution");

            migrationBuilder.RenameColumn(
                name: "Reason",
                table: "reports",
                newName: "reason");

            migrationBuilder.RenameColumn(
                name: "Details",
                table: "reports",
                newName: "details");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "reports",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "reports",
                newName: "updated_at");

            migrationBuilder.RenameColumn(
                name: "ResolvedByAdminId",
                table: "reports",
                newName: "resolved_by_admin_id");

            migrationBuilder.RenameColumn(
                name: "ResolvedAt",
                table: "reports",
                newName: "resolved_at");

            migrationBuilder.RenameColumn(
                name: "ReporterUserId",
                table: "reports",
                newName: "reporter_user_id");

            migrationBuilder.RenameColumn(
                name: "MessageId",
                table: "reports",
                newName: "message_id");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "reports",
                newName: "created_at");

            migrationBuilder.RenameColumn(
                name: "ConversationId",
                table: "reports",
                newName: "conversation_id");

            migrationBuilder.RenameIndex(
                name: "IX_reports_Status",
                table: "reports",
                newName: "i_x_reports_status");

            migrationBuilder.RenameIndex(
                name: "IX_reports_ReporterUserId",
                table: "reports",
                newName: "i_x_reports_reporter_user_id");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "refresh_tokens",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "refresh_tokens",
                newName: "user_id");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "refresh_tokens",
                newName: "updated_at");

            migrationBuilder.RenameColumn(
                name: "TokenHash",
                table: "refresh_tokens",
                newName: "token_hash");

            migrationBuilder.RenameColumn(
                name: "RevokedAt",
                table: "refresh_tokens",
                newName: "revoked_at");

            migrationBuilder.RenameColumn(
                name: "ReplacedByTokenHash",
                table: "refresh_tokens",
                newName: "replaced_by_token_hash");

            migrationBuilder.RenameColumn(
                name: "IpAddress",
                table: "refresh_tokens",
                newName: "ip_address");

            migrationBuilder.RenameColumn(
                name: "ExpiresAt",
                table: "refresh_tokens",
                newName: "expires_at");

            migrationBuilder.RenameColumn(
                name: "DeviceInfo",
                table: "refresh_tokens",
                newName: "device_info");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "refresh_tokens",
                newName: "created_at");

            migrationBuilder.RenameIndex(
                name: "IX_refresh_tokens_UserId",
                table: "refresh_tokens",
                newName: "i_x_refresh_tokens_user_id");

            migrationBuilder.RenameIndex(
                name: "IX_refresh_tokens_TokenHash",
                table: "refresh_tokens",
                newName: "i_x_refresh_tokens_token_hash");

            migrationBuilder.RenameIndex(
                name: "IX_refresh_tokens_ExpiresAt",
                table: "refresh_tokens",
                newName: "i_x_refresh_tokens_expires_at");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "payments",
                newName: "status");

            migrationBuilder.RenameColumn(
                name: "Provider",
                table: "payments",
                newName: "provider");

            migrationBuilder.RenameColumn(
                name: "Currency",
                table: "payments",
                newName: "currency");

            migrationBuilder.RenameColumn(
                name: "Amount",
                table: "payments",
                newName: "amount");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "payments",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "payments",
                newName: "user_id");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "payments",
                newName: "updated_at");

            migrationBuilder.RenameColumn(
                name: "SubscriptionId",
                table: "payments",
                newName: "subscription_id");

            migrationBuilder.RenameColumn(
                name: "RefundedAt",
                table: "payments",
                newName: "refunded_at");

            migrationBuilder.RenameColumn(
                name: "ReceiptUrl",
                table: "payments",
                newName: "receipt_url");

            migrationBuilder.RenameColumn(
                name: "RawEventJson",
                table: "payments",
                newName: "raw_event_json");

            migrationBuilder.RenameColumn(
                name: "PaidAt",
                table: "payments",
                newName: "paid_at");

            migrationBuilder.RenameColumn(
                name: "ExternalPaymentId",
                table: "payments",
                newName: "external_payment_id");

            migrationBuilder.RenameColumn(
                name: "ExternalOrderId",
                table: "payments",
                newName: "external_order_id");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "payments",
                newName: "created_at");

            migrationBuilder.RenameIndex(
                name: "IX_payments_UserId",
                table: "payments",
                newName: "i_x_payments_user_id");

            migrationBuilder.RenameIndex(
                name: "IX_payments_SubscriptionId",
                table: "payments",
                newName: "i_x_payments_subscription_id");

            migrationBuilder.RenameIndex(
                name: "IX_payments_Status",
                table: "payments",
                newName: "i_x_payments_status");

            migrationBuilder.RenameIndex(
                name: "IX_payments_ExternalPaymentId",
                table: "payments",
                newName: "i_x_payments_external_payment_id");

            migrationBuilder.RenameColumn(
                name: "Type",
                table: "notifications",
                newName: "type");

            migrationBuilder.RenameColumn(
                name: "Title",
                table: "notifications",
                newName: "title");

            migrationBuilder.RenameColumn(
                name: "Body",
                table: "notifications",
                newName: "body");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "notifications",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "notifications",
                newName: "user_id");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "notifications",
                newName: "updated_at");

            migrationBuilder.RenameColumn(
                name: "SentAt",
                table: "notifications",
                newName: "sent_at");

            migrationBuilder.RenameColumn(
                name: "ReadAt",
                table: "notifications",
                newName: "read_at");

            migrationBuilder.RenameColumn(
                name: "MetadataJson",
                table: "notifications",
                newName: "metadata_json");

            migrationBuilder.RenameColumn(
                name: "IsRead",
                table: "notifications",
                newName: "is_read");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "notifications",
                newName: "created_at");

            migrationBuilder.RenameColumn(
                name: "ActionUrl",
                table: "notifications",
                newName: "action_url");

            migrationBuilder.RenameIndex(
                name: "IX_notifications_UserId_IsRead",
                table: "notifications",
                newName: "i_x_notifications_user_id_is_read");

            migrationBuilder.RenameIndex(
                name: "IX_notifications_CreatedAt",
                table: "notifications",
                newName: "i_x_notifications_created_at");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "messages",
                newName: "status");

            migrationBuilder.RenameColumn(
                name: "Role",
                table: "messages",
                newName: "role");

            migrationBuilder.RenameColumn(
                name: "Content",
                table: "messages",
                newName: "content");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "messages",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "messages",
                newName: "updated_at");

            migrationBuilder.RenameColumn(
                name: "ParentMessageId",
                table: "messages",
                newName: "parent_message_id");

            migrationBuilder.RenameColumn(
                name: "OutputTokens",
                table: "messages",
                newName: "output_tokens");

            migrationBuilder.RenameColumn(
                name: "ModelId",
                table: "messages",
                newName: "model_id");

            migrationBuilder.RenameColumn(
                name: "InputTokens",
                table: "messages",
                newName: "input_tokens");

            migrationBuilder.RenameColumn(
                name: "EstimatedCost",
                table: "messages",
                newName: "estimated_cost");

            migrationBuilder.RenameColumn(
                name: "ErrorMessage",
                table: "messages",
                newName: "error_message");

            migrationBuilder.RenameColumn(
                name: "ErrorCode",
                table: "messages",
                newName: "error_code");

            migrationBuilder.RenameColumn(
                name: "DurationMs",
                table: "messages",
                newName: "duration_ms");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "messages",
                newName: "created_at");

            migrationBuilder.RenameColumn(
                name: "ConversationId",
                table: "messages",
                newName: "conversation_id");

            migrationBuilder.RenameColumn(
                name: "BranchRootId",
                table: "messages",
                newName: "branch_root_id");

            migrationBuilder.RenameIndex(
                name: "IX_messages_Status",
                table: "messages",
                newName: "i_x_messages_status");

            migrationBuilder.RenameIndex(
                name: "IX_messages_ParentMessageId",
                table: "messages",
                newName: "i_x_messages_parent_message_id");

            migrationBuilder.RenameIndex(
                name: "IX_messages_ModelId",
                table: "messages",
                newName: "i_x_messages_model_id");

            migrationBuilder.RenameIndex(
                name: "IX_messages_ConversationId_CreatedAt",
                table: "messages",
                newName: "i_x_messages_conversation_id_created_at");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "message_attachments",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "UploadedFileId",
                table: "message_attachments",
                newName: "uploaded_file_id");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "message_attachments",
                newName: "updated_at");

            migrationBuilder.RenameColumn(
                name: "MessageId",
                table: "message_attachments",
                newName: "message_id");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "message_attachments",
                newName: "created_at");

            migrationBuilder.RenameIndex(
                name: "IX_message_attachments_UploadedFileId",
                table: "message_attachments",
                newName: "i_x_message_attachments_uploaded_file_id");

            migrationBuilder.RenameIndex(
                name: "IX_message_attachments_MessageId",
                table: "message_attachments",
                newName: "i_x_message_attachments_message_id");

            migrationBuilder.RenameColumn(
                name: "Embedding",
                table: "file_chunks",
                newName: "embedding");

            migrationBuilder.RenameColumn(
                name: "Content",
                table: "file_chunks",
                newName: "content");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "file_chunks",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "file_chunks",
                newName: "updated_at");

            migrationBuilder.RenameColumn(
                name: "TokenCount",
                table: "file_chunks",
                newName: "token_count");

            migrationBuilder.RenameColumn(
                name: "PageNumber",
                table: "file_chunks",
                newName: "page_number");

            migrationBuilder.RenameColumn(
                name: "FileId",
                table: "file_chunks",
                newName: "file_id");

            migrationBuilder.RenameColumn(
                name: "EmbeddingModel",
                table: "file_chunks",
                newName: "embedding_model");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "file_chunks",
                newName: "created_at");

            migrationBuilder.RenameColumn(
                name: "ChunkIndex",
                table: "file_chunks",
                newName: "chunk_index");

            migrationBuilder.RenameIndex(
                name: "IX_file_chunks_FileId_ChunkIndex",
                table: "file_chunks",
                newName: "i_x_file_chunks_file_id_chunk_index");

            migrationBuilder.RenameColumn(
                name: "Rating",
                table: "feedback",
                newName: "rating");

            migrationBuilder.RenameColumn(
                name: "Category",
                table: "feedback",
                newName: "category");

            migrationBuilder.RenameColumn(
                name: "Body",
                table: "feedback",
                newName: "body");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "feedback",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "feedback",
                newName: "user_id");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "feedback",
                newName: "updated_at");

            migrationBuilder.RenameColumn(
                name: "MetadataJson",
                table: "feedback",
                newName: "metadata_json");

            migrationBuilder.RenameColumn(
                name: "IsReviewed",
                table: "feedback",
                newName: "is_reviewed");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "feedback",
                newName: "created_at");

            migrationBuilder.RenameColumn(
                name: "ContactEmail",
                table: "feedback",
                newName: "contact_email");

            migrationBuilder.RenameIndex(
                name: "IX_feedback_CreatedAt",
                table: "feedback",
                newName: "i_x_feedback_created_at");

            migrationBuilder.RenameColumn(
                name: "Key",
                table: "feature_flags",
                newName: "key");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "feature_flags",
                newName: "description");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "feature_flags",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "feature_flags",
                newName: "updated_at");

            migrationBuilder.RenameColumn(
                name: "RolloutPercent",
                table: "feature_flags",
                newName: "rollout_percent");

            migrationBuilder.RenameColumn(
                name: "IsEnabled",
                table: "feature_flags",
                newName: "is_enabled");

            migrationBuilder.RenameColumn(
                name: "EnabledForPlansCsv",
                table: "feature_flags",
                newName: "enabled_for_plans_csv");

            migrationBuilder.RenameColumn(
                name: "DisplayName",
                table: "feature_flags",
                newName: "display_name");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "feature_flags",
                newName: "created_at");

            migrationBuilder.RenameIndex(
                name: "IX_feature_flags_Key",
                table: "feature_flags",
                newName: "i_x_feature_flags_key");

            migrationBuilder.RenameColumn(
                name: "Token",
                table: "device_tokens",
                newName: "token");

            migrationBuilder.RenameColumn(
                name: "Platform",
                table: "device_tokens",
                newName: "platform");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "device_tokens",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "device_tokens",
                newName: "user_id");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "device_tokens",
                newName: "updated_at");

            migrationBuilder.RenameColumn(
                name: "LastUsedAt",
                table: "device_tokens",
                newName: "last_used_at");

            migrationBuilder.RenameColumn(
                name: "IsActive",
                table: "device_tokens",
                newName: "is_active");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "device_tokens",
                newName: "created_at");

            migrationBuilder.RenameIndex(
                name: "IX_device_tokens_UserId",
                table: "device_tokens",
                newName: "i_x_device_tokens_user_id");

            migrationBuilder.RenameIndex(
                name: "IX_device_tokens_Token",
                table: "device_tokens",
                newName: "i_x_device_tokens_token");

            migrationBuilder.RenameColumn(
                name: "Type",
                table: "credit_transactions",
                newName: "type");

            migrationBuilder.RenameColumn(
                name: "Reason",
                table: "credit_transactions",
                newName: "reason");

            migrationBuilder.RenameColumn(
                name: "Amount",
                table: "credit_transactions",
                newName: "amount");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "credit_transactions",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "credit_transactions",
                newName: "user_id");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "credit_transactions",
                newName: "updated_at");

            migrationBuilder.RenameColumn(
                name: "ReferenceId",
                table: "credit_transactions",
                newName: "reference_id");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "credit_transactions",
                newName: "created_at");

            migrationBuilder.RenameColumn(
                name: "BalanceAfter",
                table: "credit_transactions",
                newName: "balance_after");

            migrationBuilder.RenameIndex(
                name: "IX_credit_transactions_UserId",
                table: "credit_transactions",
                newName: "i_x_credit_transactions_user_id");

            migrationBuilder.RenameIndex(
                name: "IX_credit_transactions_CreatedAt",
                table: "credit_transactions",
                newName: "i_x_credit_transactions_created_at");

            migrationBuilder.RenameColumn(
                name: "Title",
                table: "conversations",
                newName: "title");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "conversations",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "conversations",
                newName: "user_id");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "conversations",
                newName: "updated_at");

            migrationBuilder.RenameColumn(
                name: "LastModelId",
                table: "conversations",
                newName: "last_model_id");

            migrationBuilder.RenameColumn(
                name: "IsPinned",
                table: "conversations",
                newName: "is_pinned");

            migrationBuilder.RenameColumn(
                name: "IsArchived",
                table: "conversations",
                newName: "is_archived");

            migrationBuilder.RenameColumn(
                name: "DeletedAt",
                table: "conversations",
                newName: "deleted_at");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "conversations",
                newName: "created_at");

            migrationBuilder.RenameIndex(
                name: "IX_conversations_UserId_UpdatedAt",
                table: "conversations",
                newName: "i_x_conversations_user_id_updated_at");

            migrationBuilder.RenameIndex(
                name: "IX_conversations_UserId_DeletedAt",
                table: "conversations",
                newName: "i_x_conversations_user_id_deleted_at");

            migrationBuilder.RenameIndex(
                name: "IX_conversations_LastModelId",
                table: "conversations",
                newName: "i_x_conversations_last_model_id");

            migrationBuilder.RenameIndex(
                name: "IX_conversations_IsPinned",
                table: "conversations",
                newName: "i_x_conversations_is_pinned");

            migrationBuilder.RenameColumn(
                name: "Action",
                table: "audit_logs",
                newName: "action");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "audit_logs",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "UserAgent",
                table: "audit_logs",
                newName: "user_agent");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "audit_logs",
                newName: "updated_at");

            migrationBuilder.RenameColumn(
                name: "MetadataJson",
                table: "audit_logs",
                newName: "metadata_json");

            migrationBuilder.RenameColumn(
                name: "IpAddress",
                table: "audit_logs",
                newName: "ip_address");

            migrationBuilder.RenameColumn(
                name: "EntityType",
                table: "audit_logs",
                newName: "entity_type");

            migrationBuilder.RenameColumn(
                name: "EntityId",
                table: "audit_logs",
                newName: "entity_id");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "audit_logs",
                newName: "created_at");

            migrationBuilder.RenameColumn(
                name: "ActorUserId",
                table: "audit_logs",
                newName: "actor_user_id");

            migrationBuilder.RenameColumn(
                name: "ActorEmail",
                table: "audit_logs",
                newName: "actor_email");

            migrationBuilder.RenameIndex(
                name: "IX_audit_logs_CreatedAt",
                table: "audit_logs",
                newName: "i_x_audit_logs_created_at");

            migrationBuilder.RenameIndex(
                name: "IX_audit_logs_ActorUserId",
                table: "audit_logs",
                newName: "i_x_audit_logs_actor_user_id");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "ai_providers",
                newName: "name");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "ai_providers",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "ai_providers",
                newName: "updated_at");

            migrationBuilder.RenameColumn(
                name: "IsActive",
                table: "ai_providers",
                newName: "is_active");

            migrationBuilder.RenameColumn(
                name: "DisplayName",
                table: "ai_providers",
                newName: "display_name");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "ai_providers",
                newName: "created_at");

            migrationBuilder.RenameColumn(
                name: "BaseUrl",
                table: "ai_providers",
                newName: "base_url");

            migrationBuilder.RenameColumn(
                name: "ApiKeySecretName",
                table: "ai_providers",
                newName: "api_key_secret_name");

            migrationBuilder.RenameIndex(
                name: "IX_ai_providers_Name",
                table: "ai_providers",
                newName: "i_x_ai_providers_name");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "ai_models",
                newName: "name");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "ai_models",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "ai_models",
                newName: "updated_at");

            migrationBuilder.RenameColumn(
                name: "ProviderId",
                table: "ai_models",
                newName: "provider_id");

            migrationBuilder.RenameColumn(
                name: "OutputPricePer1M",
                table: "ai_models",
                newName: "output_price_per1_m");

            migrationBuilder.RenameColumn(
                name: "ModelIdentifier",
                table: "ai_models",
                newName: "model_identifier");

            migrationBuilder.RenameColumn(
                name: "MaxOutputTokens",
                table: "ai_models",
                newName: "max_output_tokens");

            migrationBuilder.RenameColumn(
                name: "IsPremium",
                table: "ai_models",
                newName: "is_premium");

            migrationBuilder.RenameColumn(
                name: "IsDefault",
                table: "ai_models",
                newName: "is_default");

            migrationBuilder.RenameColumn(
                name: "IsActive",
                table: "ai_models",
                newName: "is_active");

            migrationBuilder.RenameColumn(
                name: "InputPricePer1M",
                table: "ai_models",
                newName: "input_price_per1_m");

            migrationBuilder.RenameColumn(
                name: "DisplayName",
                table: "ai_models",
                newName: "display_name");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "ai_models",
                newName: "created_at");

            migrationBuilder.RenameColumn(
                name: "ContextWindow",
                table: "ai_models",
                newName: "context_window");

            migrationBuilder.RenameColumn(
                name: "CapabilitiesCsv",
                table: "ai_models",
                newName: "capabilities_csv");

            migrationBuilder.RenameIndex(
                name: "IX_ai_models_ProviderId",
                table: "ai_models",
                newName: "i_x_ai_models_provider_id");

            migrationBuilder.RenameIndex(
                name: "IX_ai_models_Name",
                table: "ai_models",
                newName: "i_x_ai_models_name");

            migrationBuilder.RenameIndex(
                name: "IX_ai_models_IsDefault",
                table: "ai_models",
                newName: "i_x_ai_models_is_default");

            migrationBuilder.RenameIndex(
                name: "IX_ai_models_IsActive",
                table: "ai_models",
                newName: "i_x_ai_models_is_active");

            migrationBuilder.RenameColumn(
                name: "Role",
                table: "admin_users",
                newName: "role");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "admin_users",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "admin_users",
                newName: "user_id");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "admin_users",
                newName: "updated_at");

            migrationBuilder.RenameColumn(
                name: "LastActionAt",
                table: "admin_users",
                newName: "last_action_at");

            migrationBuilder.RenameColumn(
                name: "IsActive",
                table: "admin_users",
                newName: "is_active");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "admin_users",
                newName: "created_at");

            migrationBuilder.RenameIndex(
                name: "IX_admin_users_UserId",
                table: "admin_users",
                newName: "i_x_admin_users_user_id");

            migrationBuilder.AddColumn<int>(
                name: "failed_login_count",
                table: "users",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "locked_until",
                table: "users",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "p_k_users",
                table: "users",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "p_k_user_subscriptions",
                table: "user_subscriptions",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "p_k_user_profiles",
                table: "user_profiles",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "p_k_usage_records",
                table: "usage_records",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "p_k_usage_limits",
                table: "usage_limits",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "p_k_uploaded_files",
                table: "uploaded_files",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "p_k_system_settings",
                table: "system_settings",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "p_k_subscription_plans",
                table: "subscription_plans",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "p_k_reports",
                table: "reports",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "p_k_refresh_tokens",
                table: "refresh_tokens",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "p_k_payments",
                table: "payments",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "p_k_notifications",
                table: "notifications",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "p_k_messages",
                table: "messages",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "p_k_message_attachments",
                table: "message_attachments",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "p_k_file_chunks",
                table: "file_chunks",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "p_k_feedback",
                table: "feedback",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "p_k_feature_flags",
                table: "feature_flags",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "p_k_device_tokens",
                table: "device_tokens",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "p_k_credit_transactions",
                table: "credit_transactions",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "p_k_conversations",
                table: "conversations",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "p_k_audit_logs",
                table: "audit_logs",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "p_k_ai_providers",
                table: "ai_providers",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "p_k_ai_models",
                table: "ai_models",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "p_k_admin_users",
                table: "admin_users",
                column: "id");

            migrationBuilder.CreateTable(
                name: "email_verification_tokens",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    token_hash = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    expires_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    consumed_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_email_verification_tokens", x => x.id);
                    table.ForeignKey(
                        name: "f_k_email_verification_tokens_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "password_reset_tokens",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    token_hash = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    expires_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    consumed_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    requested_from_ip = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_password_reset_tokens", x => x.id);
                    table.ForeignKey(
                        name: "f_k_password_reset_tokens_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "i_x_email_verification_tokens_expires_at",
                table: "email_verification_tokens",
                column: "expires_at");

            migrationBuilder.CreateIndex(
                name: "i_x_email_verification_tokens_token_hash",
                table: "email_verification_tokens",
                column: "token_hash",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "i_x_email_verification_tokens_user_id",
                table: "email_verification_tokens",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "i_x_password_reset_tokens_expires_at",
                table: "password_reset_tokens",
                column: "expires_at");

            migrationBuilder.CreateIndex(
                name: "i_x_password_reset_tokens_token_hash",
                table: "password_reset_tokens",
                column: "token_hash",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "i_x_password_reset_tokens_user_id",
                table: "password_reset_tokens",
                column: "user_id");

            migrationBuilder.AddForeignKey(
                name: "f_k_admin_users_users_user_id",
                table: "admin_users",
                column: "user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "f_k_ai_models_ai_providers_provider_id",
                table: "ai_models",
                column: "provider_id",
                principalTable: "ai_providers",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "f_k_conversations_ai_models_last_model_id",
                table: "conversations",
                column: "last_model_id",
                principalTable: "ai_models",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "f_k_conversations_users_user_id",
                table: "conversations",
                column: "user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "f_k_credit_transactions_users_user_id",
                table: "credit_transactions",
                column: "user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "f_k_device_tokens_users_user_id",
                table: "device_tokens",
                column: "user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "f_k_file_chunks_uploaded_files_file_id",
                table: "file_chunks",
                column: "file_id",
                principalTable: "uploaded_files",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "f_k_message_attachments_messages_message_id",
                table: "message_attachments",
                column: "message_id",
                principalTable: "messages",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "f_k_message_attachments_uploaded_files_uploaded_file_id",
                table: "message_attachments",
                column: "uploaded_file_id",
                principalTable: "uploaded_files",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "f_k_messages_ai_models_model_id",
                table: "messages",
                column: "model_id",
                principalTable: "ai_models",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "f_k_messages_conversations_conversation_id",
                table: "messages",
                column: "conversation_id",
                principalTable: "conversations",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "f_k_messages_messages_parent_message_id",
                table: "messages",
                column: "parent_message_id",
                principalTable: "messages",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "f_k_notifications_users_user_id",
                table: "notifications",
                column: "user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "f_k_payments_user_subscriptions_subscription_id",
                table: "payments",
                column: "subscription_id",
                principalTable: "user_subscriptions",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "f_k_payments_users_user_id",
                table: "payments",
                column: "user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "f_k_refresh_tokens_users_user_id",
                table: "refresh_tokens",
                column: "user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "f_k_uploaded_files_users_user_id",
                table: "uploaded_files",
                column: "user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "f_k_usage_limits_subscription_plans_plan_id",
                table: "usage_limits",
                column: "plan_id",
                principalTable: "subscription_plans",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "f_k_usage_records_ai_models_model_id",
                table: "usage_records",
                column: "model_id",
                principalTable: "ai_models",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "f_k_usage_records_users_user_id",
                table: "usage_records",
                column: "user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "f_k_user_profiles_users_user_id",
                table: "user_profiles",
                column: "user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "f_k_user_subscriptions_subscription_plans_plan_id",
                table: "user_subscriptions",
                column: "plan_id",
                principalTable: "subscription_plans",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "f_k_user_subscriptions_users_user_id",
                table: "user_subscriptions",
                column: "user_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "f_k_admin_users_users_user_id",
                table: "admin_users");

            migrationBuilder.DropForeignKey(
                name: "f_k_ai_models_ai_providers_provider_id",
                table: "ai_models");

            migrationBuilder.DropForeignKey(
                name: "f_k_conversations_ai_models_last_model_id",
                table: "conversations");

            migrationBuilder.DropForeignKey(
                name: "f_k_conversations_users_user_id",
                table: "conversations");

            migrationBuilder.DropForeignKey(
                name: "f_k_credit_transactions_users_user_id",
                table: "credit_transactions");

            migrationBuilder.DropForeignKey(
                name: "f_k_device_tokens_users_user_id",
                table: "device_tokens");

            migrationBuilder.DropForeignKey(
                name: "f_k_file_chunks_uploaded_files_file_id",
                table: "file_chunks");

            migrationBuilder.DropForeignKey(
                name: "f_k_message_attachments_messages_message_id",
                table: "message_attachments");

            migrationBuilder.DropForeignKey(
                name: "f_k_message_attachments_uploaded_files_uploaded_file_id",
                table: "message_attachments");

            migrationBuilder.DropForeignKey(
                name: "f_k_messages_ai_models_model_id",
                table: "messages");

            migrationBuilder.DropForeignKey(
                name: "f_k_messages_conversations_conversation_id",
                table: "messages");

            migrationBuilder.DropForeignKey(
                name: "f_k_messages_messages_parent_message_id",
                table: "messages");

            migrationBuilder.DropForeignKey(
                name: "f_k_notifications_users_user_id",
                table: "notifications");

            migrationBuilder.DropForeignKey(
                name: "f_k_payments_user_subscriptions_subscription_id",
                table: "payments");

            migrationBuilder.DropForeignKey(
                name: "f_k_payments_users_user_id",
                table: "payments");

            migrationBuilder.DropForeignKey(
                name: "f_k_refresh_tokens_users_user_id",
                table: "refresh_tokens");

            migrationBuilder.DropForeignKey(
                name: "f_k_uploaded_files_users_user_id",
                table: "uploaded_files");

            migrationBuilder.DropForeignKey(
                name: "f_k_usage_limits_subscription_plans_plan_id",
                table: "usage_limits");

            migrationBuilder.DropForeignKey(
                name: "f_k_usage_records_ai_models_model_id",
                table: "usage_records");

            migrationBuilder.DropForeignKey(
                name: "f_k_usage_records_users_user_id",
                table: "usage_records");

            migrationBuilder.DropForeignKey(
                name: "f_k_user_profiles_users_user_id",
                table: "user_profiles");

            migrationBuilder.DropForeignKey(
                name: "f_k_user_subscriptions_subscription_plans_plan_id",
                table: "user_subscriptions");

            migrationBuilder.DropForeignKey(
                name: "f_k_user_subscriptions_users_user_id",
                table: "user_subscriptions");

            migrationBuilder.DropTable(
                name: "email_verification_tokens");

            migrationBuilder.DropTable(
                name: "password_reset_tokens");

            migrationBuilder.DropPrimaryKey(
                name: "p_k_users",
                table: "users");

            migrationBuilder.DropPrimaryKey(
                name: "p_k_user_subscriptions",
                table: "user_subscriptions");

            migrationBuilder.DropPrimaryKey(
                name: "p_k_user_profiles",
                table: "user_profiles");

            migrationBuilder.DropPrimaryKey(
                name: "p_k_usage_records",
                table: "usage_records");

            migrationBuilder.DropPrimaryKey(
                name: "p_k_usage_limits",
                table: "usage_limits");

            migrationBuilder.DropPrimaryKey(
                name: "p_k_uploaded_files",
                table: "uploaded_files");

            migrationBuilder.DropPrimaryKey(
                name: "p_k_system_settings",
                table: "system_settings");

            migrationBuilder.DropPrimaryKey(
                name: "p_k_subscription_plans",
                table: "subscription_plans");

            migrationBuilder.DropPrimaryKey(
                name: "p_k_reports",
                table: "reports");

            migrationBuilder.DropPrimaryKey(
                name: "p_k_refresh_tokens",
                table: "refresh_tokens");

            migrationBuilder.DropPrimaryKey(
                name: "p_k_payments",
                table: "payments");

            migrationBuilder.DropPrimaryKey(
                name: "p_k_notifications",
                table: "notifications");

            migrationBuilder.DropPrimaryKey(
                name: "p_k_messages",
                table: "messages");

            migrationBuilder.DropPrimaryKey(
                name: "p_k_message_attachments",
                table: "message_attachments");

            migrationBuilder.DropPrimaryKey(
                name: "p_k_file_chunks",
                table: "file_chunks");

            migrationBuilder.DropPrimaryKey(
                name: "p_k_feedback",
                table: "feedback");

            migrationBuilder.DropPrimaryKey(
                name: "p_k_feature_flags",
                table: "feature_flags");

            migrationBuilder.DropPrimaryKey(
                name: "p_k_device_tokens",
                table: "device_tokens");

            migrationBuilder.DropPrimaryKey(
                name: "p_k_credit_transactions",
                table: "credit_transactions");

            migrationBuilder.DropPrimaryKey(
                name: "p_k_conversations",
                table: "conversations");

            migrationBuilder.DropPrimaryKey(
                name: "p_k_audit_logs",
                table: "audit_logs");

            migrationBuilder.DropPrimaryKey(
                name: "p_k_ai_providers",
                table: "ai_providers");

            migrationBuilder.DropPrimaryKey(
                name: "p_k_ai_models",
                table: "ai_models");

            migrationBuilder.DropPrimaryKey(
                name: "p_k_admin_users",
                table: "admin_users");

            migrationBuilder.DropColumn(
                name: "failed_login_count",
                table: "users");

            migrationBuilder.DropColumn(
                name: "locked_until",
                table: "users");

            migrationBuilder.RenameColumn(
                name: "status",
                table: "users",
                newName: "Status");

            migrationBuilder.RenameColumn(
                name: "email",
                table: "users",
                newName: "Email");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "users",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "updated_at",
                table: "users",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "password_hash",
                table: "users",
                newName: "PasswordHash");

            migrationBuilder.RenameColumn(
                name: "last_login_at",
                table: "users",
                newName: "LastLoginAt");

            migrationBuilder.RenameColumn(
                name: "google_subject_id",
                table: "users",
                newName: "GoogleSubjectId");

            migrationBuilder.RenameColumn(
                name: "email_verified_at",
                table: "users",
                newName: "EmailVerifiedAt");

            migrationBuilder.RenameColumn(
                name: "email_verified",
                table: "users",
                newName: "EmailVerified");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "users",
                newName: "CreatedAt");

            migrationBuilder.RenameIndex(
                name: "i_x_users_google_subject_id",
                table: "users",
                newName: "IX_users_GoogleSubjectId");

            migrationBuilder.RenameIndex(
                name: "i_x_users_email",
                table: "users",
                newName: "IX_users_Email");

            migrationBuilder.RenameColumn(
                name: "status",
                table: "user_subscriptions",
                newName: "Status");

            migrationBuilder.RenameColumn(
                name: "provider",
                table: "user_subscriptions",
                newName: "Provider");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "user_subscriptions",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "user_id",
                table: "user_subscriptions",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "updated_at",
                table: "user_subscriptions",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "plan_id",
                table: "user_subscriptions",
                newName: "PlanId");

            migrationBuilder.RenameColumn(
                name: "grace_period_ends_at",
                table: "user_subscriptions",
                newName: "GracePeriodEndsAt");

            migrationBuilder.RenameColumn(
                name: "external_subscription_id",
                table: "user_subscriptions",
                newName: "ExternalSubscriptionId");

            migrationBuilder.RenameColumn(
                name: "external_purchase_token",
                table: "user_subscriptions",
                newName: "ExternalPurchaseToken");

            migrationBuilder.RenameColumn(
                name: "ended_at",
                table: "user_subscriptions",
                newName: "EndedAt");

            migrationBuilder.RenameColumn(
                name: "current_period_start",
                table: "user_subscriptions",
                newName: "CurrentPeriodStart");

            migrationBuilder.RenameColumn(
                name: "current_period_end",
                table: "user_subscriptions",
                newName: "CurrentPeriodEnd");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "user_subscriptions",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "cancelled_at",
                table: "user_subscriptions",
                newName: "CancelledAt");

            migrationBuilder.RenameColumn(
                name: "auto_renew",
                table: "user_subscriptions",
                newName: "AutoRenew");

            migrationBuilder.RenameIndex(
                name: "i_x_user_subscriptions_user_id",
                table: "user_subscriptions",
                newName: "IX_user_subscriptions_UserId");

            migrationBuilder.RenameIndex(
                name: "i_x_user_subscriptions_status",
                table: "user_subscriptions",
                newName: "IX_user_subscriptions_Status");

            migrationBuilder.RenameIndex(
                name: "i_x_user_subscriptions_plan_id",
                table: "user_subscriptions",
                newName: "IX_user_subscriptions_PlanId");

            migrationBuilder.RenameIndex(
                name: "i_x_user_subscriptions_external_subscription_id",
                table: "user_subscriptions",
                newName: "IX_user_subscriptions_ExternalSubscriptionId");

            migrationBuilder.RenameColumn(
                name: "theme",
                table: "user_profiles",
                newName: "Theme");

            migrationBuilder.RenameColumn(
                name: "language",
                table: "user_profiles",
                newName: "Language");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "user_profiles",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "user_id",
                table: "user_profiles",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "updated_at",
                table: "user_profiles",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "time_zone",
                table: "user_profiles",
                newName: "TimeZone");

            migrationBuilder.RenameColumn(
                name: "notify_usage_warning",
                table: "user_profiles",
                newName: "NotifyUsageWarning");

            migrationBuilder.RenameColumn(
                name: "notify_subscription_renewal",
                table: "user_profiles",
                newName: "NotifySubscriptionRenewal");

            migrationBuilder.RenameColumn(
                name: "notify_security_alert",
                table: "user_profiles",
                newName: "NotifySecurityAlert");

            migrationBuilder.RenameColumn(
                name: "notify_product_announcements",
                table: "user_profiles",
                newName: "NotifyProductAnnouncements");

            migrationBuilder.RenameColumn(
                name: "display_name",
                table: "user_profiles",
                newName: "DisplayName");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "user_profiles",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "avatar_url",
                table: "user_profiles",
                newName: "AvatarUrl");

            migrationBuilder.RenameColumn(
                name: "analytics_opt_in",
                table: "user_profiles",
                newName: "AnalyticsOptIn");

            migrationBuilder.RenameIndex(
                name: "i_x_user_profiles_user_id",
                table: "user_profiles",
                newName: "IX_user_profiles_UserId");

            migrationBuilder.RenameColumn(
                name: "success",
                table: "usage_records",
                newName: "Success");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "usage_records",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "user_id",
                table: "usage_records",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "updated_at",
                table: "usage_records",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "total_tokens",
                table: "usage_records",
                newName: "TotalTokens");

            migrationBuilder.RenameColumn(
                name: "request_id",
                table: "usage_records",
                newName: "RequestId");

            migrationBuilder.RenameColumn(
                name: "output_tokens",
                table: "usage_records",
                newName: "OutputTokens");

            migrationBuilder.RenameColumn(
                name: "model_id",
                table: "usage_records",
                newName: "ModelId");

            migrationBuilder.RenameColumn(
                name: "message_id",
                table: "usage_records",
                newName: "MessageId");

            migrationBuilder.RenameColumn(
                name: "input_tokens",
                table: "usage_records",
                newName: "InputTokens");

            migrationBuilder.RenameColumn(
                name: "estimated_cost",
                table: "usage_records",
                newName: "EstimatedCost");

            migrationBuilder.RenameColumn(
                name: "error_code",
                table: "usage_records",
                newName: "ErrorCode");

            migrationBuilder.RenameColumn(
                name: "duration_ms",
                table: "usage_records",
                newName: "DurationMs");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "usage_records",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "conversation_id",
                table: "usage_records",
                newName: "ConversationId");

            migrationBuilder.RenameIndex(
                name: "i_x_usage_records_user_id_created_at",
                table: "usage_records",
                newName: "IX_usage_records_UserId_CreatedAt");

            migrationBuilder.RenameIndex(
                name: "i_x_usage_records_request_id",
                table: "usage_records",
                newName: "IX_usage_records_RequestId");

            migrationBuilder.RenameIndex(
                name: "i_x_usage_records_model_id",
                table: "usage_records",
                newName: "IX_usage_records_ModelId");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "usage_limits",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "updated_at",
                table: "usage_limits",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "plan_id",
                table: "usage_limits",
                newName: "PlanId");

            migrationBuilder.RenameColumn(
                name: "monthly_tokens",
                table: "usage_limits",
                newName: "MonthlyTokens");

            migrationBuilder.RenameColumn(
                name: "monthly_messages",
                table: "usage_limits",
                newName: "MonthlyMessages");

            migrationBuilder.RenameColumn(
                name: "monthly_file_uploads",
                table: "usage_limits",
                newName: "MonthlyFileUploads");

            migrationBuilder.RenameColumn(
                name: "monthly_credit_allowance",
                table: "usage_limits",
                newName: "MonthlyCreditAllowance");

            migrationBuilder.RenameColumn(
                name: "max_file_size_mb",
                table: "usage_limits",
                newName: "MaxFileSizeMb");

            migrationBuilder.RenameColumn(
                name: "max_conversations",
                table: "usage_limits",
                newName: "MaxConversations");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "usage_limits",
                newName: "CreatedAt");

            migrationBuilder.RenameIndex(
                name: "i_x_usage_limits_plan_id",
                table: "usage_limits",
                newName: "IX_usage_limits_PlanId");

            migrationBuilder.RenameColumn(
                name: "sha256",
                table: "uploaded_files",
                newName: "Sha256");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "uploaded_files",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "user_id",
                table: "uploaded_files",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "updated_at",
                table: "uploaded_files",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "storage_key",
                table: "uploaded_files",
                newName: "StorageKey");

            migrationBuilder.RenameColumn(
                name: "size_bytes",
                table: "uploaded_files",
                newName: "SizeBytes");

            migrationBuilder.RenameColumn(
                name: "processing_status",
                table: "uploaded_files",
                newName: "ProcessingStatus");

            migrationBuilder.RenameColumn(
                name: "processing_error",
                table: "uploaded_files",
                newName: "ProcessingError");

            migrationBuilder.RenameColumn(
                name: "processed_at",
                table: "uploaded_files",
                newName: "ProcessedAt");

            migrationBuilder.RenameColumn(
                name: "page_count",
                table: "uploaded_files",
                newName: "PageCount");

            migrationBuilder.RenameColumn(
                name: "original_file_name",
                table: "uploaded_files",
                newName: "OriginalFileName");

            migrationBuilder.RenameColumn(
                name: "extracted_char_count",
                table: "uploaded_files",
                newName: "ExtractedCharCount");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "uploaded_files",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "content_type",
                table: "uploaded_files",
                newName: "ContentType");

            migrationBuilder.RenameColumn(
                name: "chunk_count",
                table: "uploaded_files",
                newName: "ChunkCount");

            migrationBuilder.RenameIndex(
                name: "i_x_uploaded_files_user_id",
                table: "uploaded_files",
                newName: "IX_uploaded_files_UserId");

            migrationBuilder.RenameIndex(
                name: "i_x_uploaded_files_processing_status",
                table: "uploaded_files",
                newName: "IX_uploaded_files_ProcessingStatus");

            migrationBuilder.RenameColumn(
                name: "value",
                table: "system_settings",
                newName: "Value");

            migrationBuilder.RenameColumn(
                name: "key",
                table: "system_settings",
                newName: "Key");

            migrationBuilder.RenameColumn(
                name: "description",
                table: "system_settings",
                newName: "Description");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "system_settings",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "value_type",
                table: "system_settings",
                newName: "ValueType");

            migrationBuilder.RenameColumn(
                name: "updated_at",
                table: "system_settings",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "is_secret",
                table: "system_settings",
                newName: "IsSecret");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "system_settings",
                newName: "CreatedAt");

            migrationBuilder.RenameIndex(
                name: "i_x_system_settings_key",
                table: "system_settings",
                newName: "IX_system_settings_Key");

            migrationBuilder.RenameColumn(
                name: "description",
                table: "subscription_plans",
                newName: "Description");

            migrationBuilder.RenameColumn(
                name: "currency",
                table: "subscription_plans",
                newName: "Currency");

            migrationBuilder.RenameColumn(
                name: "code",
                table: "subscription_plans",
                newName: "Code");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "subscription_plans",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "yearly_price",
                table: "subscription_plans",
                newName: "YearlyPrice");

            migrationBuilder.RenameColumn(
                name: "updated_at",
                table: "subscription_plans",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "stripe_yearly_price_id",
                table: "subscription_plans",
                newName: "StripeYearlyPriceId");

            migrationBuilder.RenameColumn(
                name: "stripe_monthly_price_id",
                table: "subscription_plans",
                newName: "StripeMonthlyPriceId");

            migrationBuilder.RenameColumn(
                name: "sort_order",
                table: "subscription_plans",
                newName: "SortOrder");

            migrationBuilder.RenameColumn(
                name: "paystack_yearly_plan_code",
                table: "subscription_plans",
                newName: "PaystackYearlyPlanCode");

            migrationBuilder.RenameColumn(
                name: "paystack_monthly_plan_code",
                table: "subscription_plans",
                newName: "PaystackMonthlyPlanCode");

            migrationBuilder.RenameColumn(
                name: "monthly_price",
                table: "subscription_plans",
                newName: "MonthlyPrice");

            migrationBuilder.RenameColumn(
                name: "is_default",
                table: "subscription_plans",
                newName: "IsDefault");

            migrationBuilder.RenameColumn(
                name: "is_active",
                table: "subscription_plans",
                newName: "IsActive");

            migrationBuilder.RenameColumn(
                name: "google_play_yearly_product_id",
                table: "subscription_plans",
                newName: "GooglePlayYearlyProductId");

            migrationBuilder.RenameColumn(
                name: "google_play_monthly_product_id",
                table: "subscription_plans",
                newName: "GooglePlayMonthlyProductId");

            migrationBuilder.RenameColumn(
                name: "display_name",
                table: "subscription_plans",
                newName: "DisplayName");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "subscription_plans",
                newName: "CreatedAt");

            migrationBuilder.RenameIndex(
                name: "i_x_subscription_plans_code",
                table: "subscription_plans",
                newName: "IX_subscription_plans_Code");

            migrationBuilder.RenameColumn(
                name: "status",
                table: "reports",
                newName: "Status");

            migrationBuilder.RenameColumn(
                name: "resolution",
                table: "reports",
                newName: "Resolution");

            migrationBuilder.RenameColumn(
                name: "reason",
                table: "reports",
                newName: "Reason");

            migrationBuilder.RenameColumn(
                name: "details",
                table: "reports",
                newName: "Details");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "reports",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "updated_at",
                table: "reports",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "resolved_by_admin_id",
                table: "reports",
                newName: "ResolvedByAdminId");

            migrationBuilder.RenameColumn(
                name: "resolved_at",
                table: "reports",
                newName: "ResolvedAt");

            migrationBuilder.RenameColumn(
                name: "reporter_user_id",
                table: "reports",
                newName: "ReporterUserId");

            migrationBuilder.RenameColumn(
                name: "message_id",
                table: "reports",
                newName: "MessageId");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "reports",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "conversation_id",
                table: "reports",
                newName: "ConversationId");

            migrationBuilder.RenameIndex(
                name: "i_x_reports_status",
                table: "reports",
                newName: "IX_reports_Status");

            migrationBuilder.RenameIndex(
                name: "i_x_reports_reporter_user_id",
                table: "reports",
                newName: "IX_reports_ReporterUserId");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "refresh_tokens",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "user_id",
                table: "refresh_tokens",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "updated_at",
                table: "refresh_tokens",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "token_hash",
                table: "refresh_tokens",
                newName: "TokenHash");

            migrationBuilder.RenameColumn(
                name: "revoked_at",
                table: "refresh_tokens",
                newName: "RevokedAt");

            migrationBuilder.RenameColumn(
                name: "replaced_by_token_hash",
                table: "refresh_tokens",
                newName: "ReplacedByTokenHash");

            migrationBuilder.RenameColumn(
                name: "ip_address",
                table: "refresh_tokens",
                newName: "IpAddress");

            migrationBuilder.RenameColumn(
                name: "expires_at",
                table: "refresh_tokens",
                newName: "ExpiresAt");

            migrationBuilder.RenameColumn(
                name: "device_info",
                table: "refresh_tokens",
                newName: "DeviceInfo");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "refresh_tokens",
                newName: "CreatedAt");

            migrationBuilder.RenameIndex(
                name: "i_x_refresh_tokens_user_id",
                table: "refresh_tokens",
                newName: "IX_refresh_tokens_UserId");

            migrationBuilder.RenameIndex(
                name: "i_x_refresh_tokens_token_hash",
                table: "refresh_tokens",
                newName: "IX_refresh_tokens_TokenHash");

            migrationBuilder.RenameIndex(
                name: "i_x_refresh_tokens_expires_at",
                table: "refresh_tokens",
                newName: "IX_refresh_tokens_ExpiresAt");

            migrationBuilder.RenameColumn(
                name: "status",
                table: "payments",
                newName: "Status");

            migrationBuilder.RenameColumn(
                name: "provider",
                table: "payments",
                newName: "Provider");

            migrationBuilder.RenameColumn(
                name: "currency",
                table: "payments",
                newName: "Currency");

            migrationBuilder.RenameColumn(
                name: "amount",
                table: "payments",
                newName: "Amount");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "payments",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "user_id",
                table: "payments",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "updated_at",
                table: "payments",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "subscription_id",
                table: "payments",
                newName: "SubscriptionId");

            migrationBuilder.RenameColumn(
                name: "refunded_at",
                table: "payments",
                newName: "RefundedAt");

            migrationBuilder.RenameColumn(
                name: "receipt_url",
                table: "payments",
                newName: "ReceiptUrl");

            migrationBuilder.RenameColumn(
                name: "raw_event_json",
                table: "payments",
                newName: "RawEventJson");

            migrationBuilder.RenameColumn(
                name: "paid_at",
                table: "payments",
                newName: "PaidAt");

            migrationBuilder.RenameColumn(
                name: "external_payment_id",
                table: "payments",
                newName: "ExternalPaymentId");

            migrationBuilder.RenameColumn(
                name: "external_order_id",
                table: "payments",
                newName: "ExternalOrderId");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "payments",
                newName: "CreatedAt");

            migrationBuilder.RenameIndex(
                name: "i_x_payments_user_id",
                table: "payments",
                newName: "IX_payments_UserId");

            migrationBuilder.RenameIndex(
                name: "i_x_payments_subscription_id",
                table: "payments",
                newName: "IX_payments_SubscriptionId");

            migrationBuilder.RenameIndex(
                name: "i_x_payments_status",
                table: "payments",
                newName: "IX_payments_Status");

            migrationBuilder.RenameIndex(
                name: "i_x_payments_external_payment_id",
                table: "payments",
                newName: "IX_payments_ExternalPaymentId");

            migrationBuilder.RenameColumn(
                name: "type",
                table: "notifications",
                newName: "Type");

            migrationBuilder.RenameColumn(
                name: "title",
                table: "notifications",
                newName: "Title");

            migrationBuilder.RenameColumn(
                name: "body",
                table: "notifications",
                newName: "Body");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "notifications",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "user_id",
                table: "notifications",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "updated_at",
                table: "notifications",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "sent_at",
                table: "notifications",
                newName: "SentAt");

            migrationBuilder.RenameColumn(
                name: "read_at",
                table: "notifications",
                newName: "ReadAt");

            migrationBuilder.RenameColumn(
                name: "metadata_json",
                table: "notifications",
                newName: "MetadataJson");

            migrationBuilder.RenameColumn(
                name: "is_read",
                table: "notifications",
                newName: "IsRead");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "notifications",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "action_url",
                table: "notifications",
                newName: "ActionUrl");

            migrationBuilder.RenameIndex(
                name: "i_x_notifications_user_id_is_read",
                table: "notifications",
                newName: "IX_notifications_UserId_IsRead");

            migrationBuilder.RenameIndex(
                name: "i_x_notifications_created_at",
                table: "notifications",
                newName: "IX_notifications_CreatedAt");

            migrationBuilder.RenameColumn(
                name: "status",
                table: "messages",
                newName: "Status");

            migrationBuilder.RenameColumn(
                name: "role",
                table: "messages",
                newName: "Role");

            migrationBuilder.RenameColumn(
                name: "content",
                table: "messages",
                newName: "Content");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "messages",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "updated_at",
                table: "messages",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "parent_message_id",
                table: "messages",
                newName: "ParentMessageId");

            migrationBuilder.RenameColumn(
                name: "output_tokens",
                table: "messages",
                newName: "OutputTokens");

            migrationBuilder.RenameColumn(
                name: "model_id",
                table: "messages",
                newName: "ModelId");

            migrationBuilder.RenameColumn(
                name: "input_tokens",
                table: "messages",
                newName: "InputTokens");

            migrationBuilder.RenameColumn(
                name: "estimated_cost",
                table: "messages",
                newName: "EstimatedCost");

            migrationBuilder.RenameColumn(
                name: "error_message",
                table: "messages",
                newName: "ErrorMessage");

            migrationBuilder.RenameColumn(
                name: "error_code",
                table: "messages",
                newName: "ErrorCode");

            migrationBuilder.RenameColumn(
                name: "duration_ms",
                table: "messages",
                newName: "DurationMs");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "messages",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "conversation_id",
                table: "messages",
                newName: "ConversationId");

            migrationBuilder.RenameColumn(
                name: "branch_root_id",
                table: "messages",
                newName: "BranchRootId");

            migrationBuilder.RenameIndex(
                name: "i_x_messages_status",
                table: "messages",
                newName: "IX_messages_Status");

            migrationBuilder.RenameIndex(
                name: "i_x_messages_parent_message_id",
                table: "messages",
                newName: "IX_messages_ParentMessageId");

            migrationBuilder.RenameIndex(
                name: "i_x_messages_model_id",
                table: "messages",
                newName: "IX_messages_ModelId");

            migrationBuilder.RenameIndex(
                name: "i_x_messages_conversation_id_created_at",
                table: "messages",
                newName: "IX_messages_ConversationId_CreatedAt");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "message_attachments",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "uploaded_file_id",
                table: "message_attachments",
                newName: "UploadedFileId");

            migrationBuilder.RenameColumn(
                name: "updated_at",
                table: "message_attachments",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "message_id",
                table: "message_attachments",
                newName: "MessageId");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "message_attachments",
                newName: "CreatedAt");

            migrationBuilder.RenameIndex(
                name: "i_x_message_attachments_uploaded_file_id",
                table: "message_attachments",
                newName: "IX_message_attachments_UploadedFileId");

            migrationBuilder.RenameIndex(
                name: "i_x_message_attachments_message_id",
                table: "message_attachments",
                newName: "IX_message_attachments_MessageId");

            migrationBuilder.RenameColumn(
                name: "embedding",
                table: "file_chunks",
                newName: "Embedding");

            migrationBuilder.RenameColumn(
                name: "content",
                table: "file_chunks",
                newName: "Content");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "file_chunks",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "updated_at",
                table: "file_chunks",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "token_count",
                table: "file_chunks",
                newName: "TokenCount");

            migrationBuilder.RenameColumn(
                name: "page_number",
                table: "file_chunks",
                newName: "PageNumber");

            migrationBuilder.RenameColumn(
                name: "file_id",
                table: "file_chunks",
                newName: "FileId");

            migrationBuilder.RenameColumn(
                name: "embedding_model",
                table: "file_chunks",
                newName: "EmbeddingModel");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "file_chunks",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "chunk_index",
                table: "file_chunks",
                newName: "ChunkIndex");

            migrationBuilder.RenameIndex(
                name: "i_x_file_chunks_file_id_chunk_index",
                table: "file_chunks",
                newName: "IX_file_chunks_FileId_ChunkIndex");

            migrationBuilder.RenameColumn(
                name: "rating",
                table: "feedback",
                newName: "Rating");

            migrationBuilder.RenameColumn(
                name: "category",
                table: "feedback",
                newName: "Category");

            migrationBuilder.RenameColumn(
                name: "body",
                table: "feedback",
                newName: "Body");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "feedback",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "user_id",
                table: "feedback",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "updated_at",
                table: "feedback",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "metadata_json",
                table: "feedback",
                newName: "MetadataJson");

            migrationBuilder.RenameColumn(
                name: "is_reviewed",
                table: "feedback",
                newName: "IsReviewed");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "feedback",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "contact_email",
                table: "feedback",
                newName: "ContactEmail");

            migrationBuilder.RenameIndex(
                name: "i_x_feedback_created_at",
                table: "feedback",
                newName: "IX_feedback_CreatedAt");

            migrationBuilder.RenameColumn(
                name: "key",
                table: "feature_flags",
                newName: "Key");

            migrationBuilder.RenameColumn(
                name: "description",
                table: "feature_flags",
                newName: "Description");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "feature_flags",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "updated_at",
                table: "feature_flags",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "rollout_percent",
                table: "feature_flags",
                newName: "RolloutPercent");

            migrationBuilder.RenameColumn(
                name: "is_enabled",
                table: "feature_flags",
                newName: "IsEnabled");

            migrationBuilder.RenameColumn(
                name: "enabled_for_plans_csv",
                table: "feature_flags",
                newName: "EnabledForPlansCsv");

            migrationBuilder.RenameColumn(
                name: "display_name",
                table: "feature_flags",
                newName: "DisplayName");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "feature_flags",
                newName: "CreatedAt");

            migrationBuilder.RenameIndex(
                name: "i_x_feature_flags_key",
                table: "feature_flags",
                newName: "IX_feature_flags_Key");

            migrationBuilder.RenameColumn(
                name: "token",
                table: "device_tokens",
                newName: "Token");

            migrationBuilder.RenameColumn(
                name: "platform",
                table: "device_tokens",
                newName: "Platform");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "device_tokens",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "user_id",
                table: "device_tokens",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "updated_at",
                table: "device_tokens",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "last_used_at",
                table: "device_tokens",
                newName: "LastUsedAt");

            migrationBuilder.RenameColumn(
                name: "is_active",
                table: "device_tokens",
                newName: "IsActive");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "device_tokens",
                newName: "CreatedAt");

            migrationBuilder.RenameIndex(
                name: "i_x_device_tokens_user_id",
                table: "device_tokens",
                newName: "IX_device_tokens_UserId");

            migrationBuilder.RenameIndex(
                name: "i_x_device_tokens_token",
                table: "device_tokens",
                newName: "IX_device_tokens_Token");

            migrationBuilder.RenameColumn(
                name: "type",
                table: "credit_transactions",
                newName: "Type");

            migrationBuilder.RenameColumn(
                name: "reason",
                table: "credit_transactions",
                newName: "Reason");

            migrationBuilder.RenameColumn(
                name: "amount",
                table: "credit_transactions",
                newName: "Amount");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "credit_transactions",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "user_id",
                table: "credit_transactions",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "updated_at",
                table: "credit_transactions",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "reference_id",
                table: "credit_transactions",
                newName: "ReferenceId");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "credit_transactions",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "balance_after",
                table: "credit_transactions",
                newName: "BalanceAfter");

            migrationBuilder.RenameIndex(
                name: "i_x_credit_transactions_user_id",
                table: "credit_transactions",
                newName: "IX_credit_transactions_UserId");

            migrationBuilder.RenameIndex(
                name: "i_x_credit_transactions_created_at",
                table: "credit_transactions",
                newName: "IX_credit_transactions_CreatedAt");

            migrationBuilder.RenameColumn(
                name: "title",
                table: "conversations",
                newName: "Title");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "conversations",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "user_id",
                table: "conversations",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "updated_at",
                table: "conversations",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "last_model_id",
                table: "conversations",
                newName: "LastModelId");

            migrationBuilder.RenameColumn(
                name: "is_pinned",
                table: "conversations",
                newName: "IsPinned");

            migrationBuilder.RenameColumn(
                name: "is_archived",
                table: "conversations",
                newName: "IsArchived");

            migrationBuilder.RenameColumn(
                name: "deleted_at",
                table: "conversations",
                newName: "DeletedAt");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "conversations",
                newName: "CreatedAt");

            migrationBuilder.RenameIndex(
                name: "i_x_conversations_user_id_updated_at",
                table: "conversations",
                newName: "IX_conversations_UserId_UpdatedAt");

            migrationBuilder.RenameIndex(
                name: "i_x_conversations_user_id_deleted_at",
                table: "conversations",
                newName: "IX_conversations_UserId_DeletedAt");

            migrationBuilder.RenameIndex(
                name: "i_x_conversations_last_model_id",
                table: "conversations",
                newName: "IX_conversations_LastModelId");

            migrationBuilder.RenameIndex(
                name: "i_x_conversations_is_pinned",
                table: "conversations",
                newName: "IX_conversations_IsPinned");

            migrationBuilder.RenameColumn(
                name: "action",
                table: "audit_logs",
                newName: "Action");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "audit_logs",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "user_agent",
                table: "audit_logs",
                newName: "UserAgent");

            migrationBuilder.RenameColumn(
                name: "updated_at",
                table: "audit_logs",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "metadata_json",
                table: "audit_logs",
                newName: "MetadataJson");

            migrationBuilder.RenameColumn(
                name: "ip_address",
                table: "audit_logs",
                newName: "IpAddress");

            migrationBuilder.RenameColumn(
                name: "entity_type",
                table: "audit_logs",
                newName: "EntityType");

            migrationBuilder.RenameColumn(
                name: "entity_id",
                table: "audit_logs",
                newName: "EntityId");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "audit_logs",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "actor_user_id",
                table: "audit_logs",
                newName: "ActorUserId");

            migrationBuilder.RenameColumn(
                name: "actor_email",
                table: "audit_logs",
                newName: "ActorEmail");

            migrationBuilder.RenameIndex(
                name: "i_x_audit_logs_created_at",
                table: "audit_logs",
                newName: "IX_audit_logs_CreatedAt");

            migrationBuilder.RenameIndex(
                name: "i_x_audit_logs_actor_user_id",
                table: "audit_logs",
                newName: "IX_audit_logs_ActorUserId");

            migrationBuilder.RenameColumn(
                name: "name",
                table: "ai_providers",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "ai_providers",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "updated_at",
                table: "ai_providers",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "is_active",
                table: "ai_providers",
                newName: "IsActive");

            migrationBuilder.RenameColumn(
                name: "display_name",
                table: "ai_providers",
                newName: "DisplayName");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "ai_providers",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "base_url",
                table: "ai_providers",
                newName: "BaseUrl");

            migrationBuilder.RenameColumn(
                name: "api_key_secret_name",
                table: "ai_providers",
                newName: "ApiKeySecretName");

            migrationBuilder.RenameIndex(
                name: "i_x_ai_providers_name",
                table: "ai_providers",
                newName: "IX_ai_providers_Name");

            migrationBuilder.RenameColumn(
                name: "name",
                table: "ai_models",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "ai_models",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "updated_at",
                table: "ai_models",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "provider_id",
                table: "ai_models",
                newName: "ProviderId");

            migrationBuilder.RenameColumn(
                name: "output_price_per1_m",
                table: "ai_models",
                newName: "OutputPricePer1M");

            migrationBuilder.RenameColumn(
                name: "model_identifier",
                table: "ai_models",
                newName: "ModelIdentifier");

            migrationBuilder.RenameColumn(
                name: "max_output_tokens",
                table: "ai_models",
                newName: "MaxOutputTokens");

            migrationBuilder.RenameColumn(
                name: "is_premium",
                table: "ai_models",
                newName: "IsPremium");

            migrationBuilder.RenameColumn(
                name: "is_default",
                table: "ai_models",
                newName: "IsDefault");

            migrationBuilder.RenameColumn(
                name: "is_active",
                table: "ai_models",
                newName: "IsActive");

            migrationBuilder.RenameColumn(
                name: "input_price_per1_m",
                table: "ai_models",
                newName: "InputPricePer1M");

            migrationBuilder.RenameColumn(
                name: "display_name",
                table: "ai_models",
                newName: "DisplayName");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "ai_models",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "context_window",
                table: "ai_models",
                newName: "ContextWindow");

            migrationBuilder.RenameColumn(
                name: "capabilities_csv",
                table: "ai_models",
                newName: "CapabilitiesCsv");

            migrationBuilder.RenameIndex(
                name: "i_x_ai_models_provider_id",
                table: "ai_models",
                newName: "IX_ai_models_ProviderId");

            migrationBuilder.RenameIndex(
                name: "i_x_ai_models_name",
                table: "ai_models",
                newName: "IX_ai_models_Name");

            migrationBuilder.RenameIndex(
                name: "i_x_ai_models_is_default",
                table: "ai_models",
                newName: "IX_ai_models_IsDefault");

            migrationBuilder.RenameIndex(
                name: "i_x_ai_models_is_active",
                table: "ai_models",
                newName: "IX_ai_models_IsActive");

            migrationBuilder.RenameColumn(
                name: "role",
                table: "admin_users",
                newName: "Role");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "admin_users",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "user_id",
                table: "admin_users",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "updated_at",
                table: "admin_users",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "last_action_at",
                table: "admin_users",
                newName: "LastActionAt");

            migrationBuilder.RenameColumn(
                name: "is_active",
                table: "admin_users",
                newName: "IsActive");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "admin_users",
                newName: "CreatedAt");

            migrationBuilder.RenameIndex(
                name: "i_x_admin_users_user_id",
                table: "admin_users",
                newName: "IX_admin_users_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_users",
                table: "users",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_user_subscriptions",
                table: "user_subscriptions",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_user_profiles",
                table: "user_profiles",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_usage_records",
                table: "usage_records",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_usage_limits",
                table: "usage_limits",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_uploaded_files",
                table: "uploaded_files",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_system_settings",
                table: "system_settings",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_subscription_plans",
                table: "subscription_plans",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_reports",
                table: "reports",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_refresh_tokens",
                table: "refresh_tokens",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_payments",
                table: "payments",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_notifications",
                table: "notifications",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_messages",
                table: "messages",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_message_attachments",
                table: "message_attachments",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_file_chunks",
                table: "file_chunks",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_feedback",
                table: "feedback",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_feature_flags",
                table: "feature_flags",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_device_tokens",
                table: "device_tokens",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_credit_transactions",
                table: "credit_transactions",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_conversations",
                table: "conversations",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_audit_logs",
                table: "audit_logs",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ai_providers",
                table: "ai_providers",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ai_models",
                table: "ai_models",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_admin_users",
                table: "admin_users",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_admin_users_users_UserId",
                table: "admin_users",
                column: "UserId",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ai_models_ai_providers_ProviderId",
                table: "ai_models",
                column: "ProviderId",
                principalTable: "ai_providers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_conversations_ai_models_LastModelId",
                table: "conversations",
                column: "LastModelId",
                principalTable: "ai_models",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_conversations_users_UserId",
                table: "conversations",
                column: "UserId",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_credit_transactions_users_UserId",
                table: "credit_transactions",
                column: "UserId",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_device_tokens_users_UserId",
                table: "device_tokens",
                column: "UserId",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_file_chunks_uploaded_files_FileId",
                table: "file_chunks",
                column: "FileId",
                principalTable: "uploaded_files",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_message_attachments_messages_MessageId",
                table: "message_attachments",
                column: "MessageId",
                principalTable: "messages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_message_attachments_uploaded_files_UploadedFileId",
                table: "message_attachments",
                column: "UploadedFileId",
                principalTable: "uploaded_files",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_messages_ai_models_ModelId",
                table: "messages",
                column: "ModelId",
                principalTable: "ai_models",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_messages_conversations_ConversationId",
                table: "messages",
                column: "ConversationId",
                principalTable: "conversations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_messages_messages_ParentMessageId",
                table: "messages",
                column: "ParentMessageId",
                principalTable: "messages",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_notifications_users_UserId",
                table: "notifications",
                column: "UserId",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_payments_user_subscriptions_SubscriptionId",
                table: "payments",
                column: "SubscriptionId",
                principalTable: "user_subscriptions",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_payments_users_UserId",
                table: "payments",
                column: "UserId",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_refresh_tokens_users_UserId",
                table: "refresh_tokens",
                column: "UserId",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_uploaded_files_users_UserId",
                table: "uploaded_files",
                column: "UserId",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_usage_limits_subscription_plans_PlanId",
                table: "usage_limits",
                column: "PlanId",
                principalTable: "subscription_plans",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_usage_records_ai_models_ModelId",
                table: "usage_records",
                column: "ModelId",
                principalTable: "ai_models",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_usage_records_users_UserId",
                table: "usage_records",
                column: "UserId",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_user_profiles_users_UserId",
                table: "user_profiles",
                column: "UserId",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_user_subscriptions_subscription_plans_PlanId",
                table: "user_subscriptions",
                column: "PlanId",
                principalTable: "subscription_plans",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_user_subscriptions_users_UserId",
                table: "user_subscriptions",
                column: "UserId",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
