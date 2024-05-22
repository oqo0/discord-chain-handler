using Discord.Addons.ChainHandlers.Configuration;
using Discord.Addons.ChainHandlers.Configuration.Options;
using Microsoft.Extensions.DependencyInjection;

namespace Discord.Addons.ChainHandlers;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInteractionHandler(
        this IServiceCollection serviceCollection,
        ConfigureInteractionHandlerOptions configureInteractionHandlerOptions)
    {
        var interactionHandlerOptions = new InteractionHandlerOptions(serviceCollection);
        configureInteractionHandlerOptions.Invoke(interactionHandlerOptions);
        return serviceCollection;
    }
}