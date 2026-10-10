using Chima.Domain.Common;

namespace Chima.Domain.Entities;

public class AIProvider : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string BaseUrl { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public string ApiKeySecretName { get; set; } = string.Empty;

    public ICollection<AIModel> Models { get; set; } = new List<AIModel>();
}
