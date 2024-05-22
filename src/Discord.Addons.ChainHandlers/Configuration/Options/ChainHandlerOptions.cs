using Discord.Addons.ChainHandlers.ChainHandlers;
using Discord.Addons.ChainHandlers.Common;
using Microsoft.Extensions.DependencyInjection;

namespace Discord.Addons.ChainHandlers.Configuration.Options;

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