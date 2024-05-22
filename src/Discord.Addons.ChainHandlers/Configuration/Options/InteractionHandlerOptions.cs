using Microsoft.Extensions.DependencyInjection;

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
    
    public InteractionHandlerOptions ConfigureInteractionService(
        ConfigureInteractionService configureInteractionService)
    {
        _serviceCollection.AddSingleton(configureInteractionService);
        return this;
    }
}