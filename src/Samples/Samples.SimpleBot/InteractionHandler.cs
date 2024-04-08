using System.Reflection;
using Discord;
using Discord.Addons.ChainHandler;
using Discord.Addons.ChainHandler.Common;
using Discord.Addons.ChainHandler.Default;
using Discord.Addons.Hosting;
using Discord.Addons.Hosting.Util;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Samples.SimpleBot;

public class InteractionHandler : DiscordClientService
{
    private readonly InteractionService _interactionService;
    private readonly IServiceProvider _serviceProvider;
    private readonly ChainHandlerBuilder _chainHandlerBuilder;
    private readonly IConfiguration _configuration;

    public InteractionHandler(
        DiscordSocketClient client,
        ILogger<DiscordClientService> logger,
        InteractionService interactionService,
        IServiceProvider serviceProvider,
        ChainHandlerBuilder chainHandlerBuilder, 
        IConfiguration configuration) : base(client, logger)
    {
        _interactionService = interactionService;
        _serviceProvider = serviceProvider;
        _chainHandlerBuilder = chainHandlerBuilder;
        _configuration = configuration;
    }
    
    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        var chainHandler = _chainHandlerBuilder
            .Add<ErrorChainHandler>()
            .Add<ProblemChainHandler>()
            .Build();

        Client.InteractionCreated += chainHandler.Handle;
        _interactionService.InteractionExecuted += HandleInteractionExecute;
        
        await _interactionService.AddModulesAsync(Assembly.GetEntryAssembly(), _serviceProvider);
        await Client.WaitForReadyAsync(cancellationToken);

        var stagingGuildId = _configuration.GetValue<ulong>("GuildId");

        await _interactionService.AddModulesGloballyAsync(
            true, Array.Empty<ModuleInfo>());
        await _interactionService.AddModulesToGuildAsync(
            stagingGuildId, true, Array.Empty<ModuleInfo>());
        
        await _interactionService.RegisterCommandsToGuildAsync(stagingGuildId);
    }

    private async Task HandleInteractionExecute(
        ICommandInfo commandInfo, IInteractionContext interactionContext, IResult result)
    {
        if (!result.IsSuccess)
        {
            await interactionContext.Interaction.RespondAsync("Something bad happened!", ephemeral: true);
        }
    }
}