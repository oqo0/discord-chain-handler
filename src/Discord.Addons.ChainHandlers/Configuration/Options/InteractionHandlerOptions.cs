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
            GetInteractionHandler(provider, (_, _) => {}, configureInteractionService, value));
        
        _serviceCollection.AddSingleton(configureInteractionService);
        return this;
    }
    
    public InteractionHandlerOptions ConfigureInteractionService<T>(
        ConfigureInteractionServiceBeforeStart<T> configureInteractionServiceBeforeStart,
        ConfigureInteractionService<T> configureInteractionService,
        T value)
    {
        _serviceCollection.AddHostedService(provider =>
            GetInteractionHandler(provider, configureInteractionServiceBeforeStart, configureInteractionService, value));
        
        _serviceCollection.AddSingleton(configureInteractionService);
        return this;
    }
    
    private static InteractionHandler<T> GetInteractionHandler<T>(
        IServiceProvider serviceProvider,
        ConfigureInteractionServiceBeforeStart<T> configureInteractionServiceBeforeStart,
        ConfigureInteractionService<T> configureInteractionService,
        T value)
    {
        return new InteractionHandler<T>(
            serviceProvider.GetRequiredService<DiscordSocketClient>(),
            serviceProvider.GetRequiredService<ILogger<DiscordClientService>>(),
            serviceProvider.GetRequiredService<InteractionService>(),
            serviceProvider.GetRequiredService<IServiceProvider>(),
            serviceProvider.GetRequiredService<ChainHandlerBuilder>(),
            configureInteractionServiceBeforeStart,
            configureInteractionService,
            serviceProvider.GetRequiredService<ConfigureFinalHandler>(),
            value);
    }
}