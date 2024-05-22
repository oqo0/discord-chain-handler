#pragma warning disable CS0618 // Type or member is obsolete

using Discord;
using Discord.Addons.ChainHandlers.Configuration;
using Discord.Addons.ChainHandlers.Default;
using Discord.Addons.Hosting;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

var host = Host.CreateDefaultBuilder(args);

host.ConfigureDiscordHost((context, config) =>
{
    config.SocketConfig = new DiscordSocketConfig
    {
        LogLevel = LogSeverity.Verbose,
        AlwaysDownloadUsers = true,
        MessageCacheSize = 200
    };
    config.Token = context.Configuration["Token"];
})
.UseCommandService((_, config) =>
{
    config.DefaultRunMode = RunMode.Async;
    config.CaseSensitiveCommands = false;
})
.UseInteractionService((_, config) =>
{
    config.LogLevel = LogSeverity.Info;
    config.UseCompiledLambda = true;
})
.ConfigureServices((context, services) =>
{
    services.AddInteractionHandler(options =>
    {
        options.UseChainHandler(handlerOptions =>
        {
            handlerOptions.Add<ErrorChainHandler>();
            handlerOptions.Add<ProblemChainHandler>();
        });

        options.UseFinalHandler(async interactionContext =>
        {
            await interactionContext.Interaction.RespondAsync(
                "Something bad happened in final!", ephemeral: true);
        });
        
        options.ConfigureInteractionService(async interactionService =>
        {
            var stagingGuildId = context.Configuration.GetValue<ulong>("GuildId");

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
});

await host
    .Build()
    .RunAsync();