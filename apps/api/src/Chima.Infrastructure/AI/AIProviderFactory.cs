using Chima.Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Chima.Application.Settings;

namespace Chima.Infrastructure.AI;

public class AIProviderFactory
{
    private readonly IServiceProvider _sp;
    private readonly AISettings _settings;

    public AIProviderFactory(IServiceProvider sp, IOptions<AISettings> settings)
    {
        _sp = sp;
        _settings = settings.Value;
    }

    public IAIProvider Get()
    {
        return _settings.Provider.ToLowerInvariant() switch
        {
            "openai" => _sp.GetRequiredService<OpenAIProvider>(),
            "compatible" => _sp.GetRequiredService<CompatibleAIProvider>(),
            _ => _sp.GetRequiredService<OpenAIProvider>()
        };
    }
}