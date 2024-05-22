#pragma warning disable CS0618 // Type or member is obsolete

using Discord;
using Discord.Addons.Hosting;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Hosting;
using Samples.SimpleBot;

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
    services.AddInteraction(context.Configuration);
});

await host
    .Build()
    .RunAsync();