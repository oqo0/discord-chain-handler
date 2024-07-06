using Discord.Addons.ChainHandlers.Common;
using Discord.Addons.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Discord.Addons.ChainHandlers.Configuration.Options;

public class InteractionHandlerOptions
{
    private readonly IServiceCollection _serviceCollection;

    public InteractionHandlerOptions(IServiceCollection serviceCollection)
    {
        _serviceCollection = serviceCollection;
    }
    
    public InteractionHandlerOptions UseChainHandler(ConfigureChainHandler configureChainHandler)
    {
        var chainHandlerOptions = new ChainHandlerOptions(_serviceCollection);
        configureChainHandler.Invoke(chainHandlerOptions);
        return this;
    }

    public InteractionHandlerOptions UseFinalHandler(ConfigureFinalHandler configureFinalHandler)
    {
        _serviceCollection.AddSingleton(configureFinalHandler);
        return this;
    }
    
    public InteractionHandlerOptions ConfigureInteractionService<T>(
        ConfigureInteractionService<T> configureInteractionService,
        T value)
    {
        _serviceCollection.AddHostedService(provider =>
            new InteractionHandler<T>(
                provider.GetRequiredService<DiscordSocketClient>(),
                provider.GetRequiredService<ILogger<DiscordClientService>>(),
                provider.GetRequiredService<InteractionService>(),
                provider.GetRequiredService<IServiceProvider>(),
                provider.GetRequiredService<ChainHandlerBuilder>(),
                provider.GetRequiredService<ConfigureInteractionService<T>>(),
                provider.GetRequiredService<ConfigureFinalHandler>(),
                value));
        
        _serviceCollection.AddSingleton(configureInteractionService);
        return this;
    }
}