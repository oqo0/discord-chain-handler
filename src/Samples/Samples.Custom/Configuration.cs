using Discord.Addons.ChainHandlers;
using Discord.Addons.ChainHandlers.Default;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Samples.Custom.ChainHandlers;

namespace Samples.Custom;

internal static class Configuration
{
    internal static IServiceCollection AddInteraction(
        this IServiceCollection serviceCollection,
        IConfiguration configuration)
    {
        serviceCollection.AddInteractionHandler(options =>
        {
            options.UseChainHandler(handlerOptions =>
            {
                handlerOptions
                    .Add<ErrorChainHandler>()
                    .Add<ProblemChainHandler>()
                    .Add<RateLimiter>();
            });

            options.UseFinalHandler(async interactionContext =>
            {
                await interactionContext.Interaction.RespondAsync(
                    "Something bad happened in final!", ephemeral: true);
            });
        
            options.ConfigureInteractionService(async interactionService =>
            {
                var stagingGuildId = configuration.GetValue<ulong>("GuildId");

                await interactionService.AddModulesGloballyAsync(
                    true,
                    Array.Empty<Discord.Interactions.ModuleInfo>());
                await interactionService.AddModulesToGuildAsync(
                    stagingGuildId, 
                    true,
                    Array.Empty<Discord.Interactions.ModuleInfo>());
            
                await interactionService.RegisterCommandsToGuildAsync(stagingGuildId);
            });
        });

        return serviceCollection;
    }
}