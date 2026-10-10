namespace Chima.Application.DTOs.Admin;

public class DashboardMetricsDto
{
    public int TotalUsers { get; set; }
    public int NewUsersInPeriod { get; set; }
    public int ActiveUsersInPeriod { get; set; }
    public int PaidUsers { get; set; }
    public int FreeUsers { get; set; }

    public decimal RevenueInPeriod { get; set; }
    public decimal MonthlyRecurringRevenue { get; set; }

    public int AiRequestsInPeriod { get; set; }
    public decimal EstimatedAiCostInPeriod { get; set; }
    public decimal GrossMarginInPeriod { get; set; }
    public double GrossMarginPercent { get; set; }

    public int FailedRequestsInPeriod { get; set; }
    public int TotalConversations { get; set; }
    public int TotalMessages { get; set; }
    public long TotalStorageBytes { get; set; }

    public List<TimeSeriesPoint> NewUsersSeries { get; set; } = new();
    public List<TimeSeriesPoint> RevenueSeries { get; set; } = new();
    public List<TimeSeriesPoint> AiRequestsSeries { get; set; } = new();
}

public class TimeSeriesPoint
{
    public DateTime Date { get; set; }
    public decimal Value { get; set; }
}

public class RevenueMetricsDto
{
    public decimal TotalRevenue { get; set; }
    public decimal RevenueInPeriod { get; set; }
    public decimal Mrr { get; set; }
    public decimal Arr { get; set; }
    public int ActiveSubscriptions { get; set; }
    public int CancelledInPeriod { get; set; }
    public int NewSubscriptionsInPeriod { get; set; }
    public double ChurnPercent { get; set; }
    public List<PlanRevenueDto> ByPlan { get; set; } = new();
}

public class PlanRevenueDto
{
    public string PlanCode { get; set; } = string.Empty;
    public string PlanName { get; set; } = string.Empty;
    public int Subscribers { get; set; }
    public decimal Mrr { get; set; }
}

public class AiCostMetricsDto
{
    public int RequestsInPeriod { get; set; }
    public int InputTokensInPeriod { get; set; }
    public int OutputTokensInPeriod { get; set; }
    public decimal EstimatedCostInPeriod { get; set; }
    public decimal RevenueInPeriod { get; set; }
    public decimal GrossMarginInPeriod { get; set; }
    public double GrossMarginPercent { get; set; }

    public List<ModelCostDto> ByModel { get; set; } = new();
    public List<TimeSeriesPoint> DailyCost { get; set; } = new();
}

public class ModelCostDto
{
    public Guid ModelId { get; set; }
    public string ModelName { get; set; } = string.Empty;
    public string ProviderName { get; set; } = string.Empty;
    public int Requests { get; set; }
    public int InputTokens { get; set; }
    public int OutputTokens { get; set; }
    public decimal EstimatedCost { get; set; }
}