using Discord.Addons.ChainHandlers.ChainHandlers;
using Discord.Addons.ChainHandlers.Common;
using Microsoft.Extensions.DependencyInjection;

namespace Discord.Addons.ChainHandlers.Configuration;

public delegate void ConfigureInteractionHandlerOptions(InteractionHandlerOptions interactionHandlerOptions);

public delegate void ConfigureChainHandler(ChainHandlerOptions chainHandlerOptions);
public delegate void ConfigureFinalHandler(IInteractionContext interactionContext);
public delegate void ConfigureInteractionService(InteractionService interactionService);

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
    
    public InteractionHandlerOptions ConfigureInteractionService(
        ConfigureInteractionService configureInteractionService)
    {
        _serviceCollection.AddSingleton(configureInteractionService);
        return this;
    }
}

public class ChainHandlerOptions
{
    private readonly IServiceCollection _serviceCollection;

    public ChainHandlerOptions(IServiceCollection serviceCollection)
    {
        _serviceCollection = serviceCollection;
    }

    public ChainHandlerOptions Add<TChainHandler>() where TChainHandler : ChainHandler
    {
        if (typeof(TChainHandler) == typeof(ChainHandler))
        {
            throw new InvalidOperationException(
                "Cannot add ChainHandler directly. Please add a derived class.");
        }
        
        _serviceCollection.AddSingleton<ChainHandler, TChainHandler>();
        _serviceCollection.AddSingleton<ChainHandlerBuilder>();
        _serviceCollection.AddHostedService<InteractionHandler>();
        
        return this;
    }
}