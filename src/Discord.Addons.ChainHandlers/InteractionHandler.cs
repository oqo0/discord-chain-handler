using System.Reflection;
using Discord.Addons.ChainHandlers.ChainHandlers;
using Discord.Addons.ChainHandlers.Common;
using Discord.Addons.ChainHandlers.Configuration;
using Discord.Addons.Hosting;
using Discord.Addons.Hosting.Util;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Discord.Addons.ChainHandlers;

public class InteractionHandler : DiscordClientService
{
    private readonly InteractionService _interactionService;
    private readonly IServiceProvider _serviceProvider;
    private readonly ChainHandlerBuilder _chainHandlerBuilder;
    private readonly ConfigureInteractionService _configureInteractionService;
    private readonly ConfigureFinalHandler _configureFinalHandler;
    private readonly IConfiguration _configuration;

    public InteractionHandler(
        DiscordSocketClient client,
        ILogger<DiscordClientService> logger,
        InteractionService interactionService,
        IServiceProvider serviceProvider,
        ChainHandlerBuilder chainHandlerBuilder, 
        ConfigureInteractionService configureInteractionService,
        ConfigureFinalHandler configureFinalHandler,
        IConfiguration configuration) : base(client, logger)
    {
        _interactionService = interactionService;
        _serviceProvider = serviceProvider;
        _chainHandlerBuilder = chainHandlerBuilder;
        _configureInteractionService = configureInteractionService;
        _configureFinalHandler = configureFinalHandler;
        _configuration = configuration;
    }
    
    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        var chainHandlers = _serviceProvider.GetServices<ChainHandler>();

        foreach (var chainHandler in chainHandlers)
        {
            _chainHandlerBuilder.Add(chainHandler);
        }
        
        Client.InteractionCreated += _chainHandlerBuilder.Build().Handle;
        _interactionService.InteractionExecuted += HandleInteractionExecute;
        
        await _interactionService.AddModulesAsync(Assembly.GetEntryAssembly(), _serviceProvider);
        await Client.WaitForReadyAsync(cancellationToken);

        _configureInteractionService.Invoke(_interactionService, _configuration);
    }

    private async Task HandleInteractionExecute(
        ICommandInfo commandInfo, IInteractionContext interactionContext, IResult result)
    {
        if (!result.IsSuccess)
        {
            _configureFinalHandler.Invoke(interactionContext);
        }
    }
}