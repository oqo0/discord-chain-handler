using System.Reflection;
using Discord.Addons.ChainHandlers.ChainHandlers;
using Discord.Addons.ChainHandlers.Common;
using Discord.Addons.ChainHandlers.Configuration;
using Discord.Addons.Hosting;
using Discord.Addons.Hosting.Util;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Discord.Addons.ChainHandlers;

public class InteractionHandler<T> : DiscordClientService
{
    private readonly InteractionService _interactionService;
    private readonly IServiceProvider _serviceProvider;
    private readonly ChainHandlerBuilder _chainHandlerBuilder;
    private readonly ConfigureInteractionServiceBeforeStart<T> _configureInteractionServiceBeforeStart;
    private readonly ConfigureInteractionService<T> _configureInteractionService;
    private readonly ConfigureFinalHandler _configureFinalHandler;
    private readonly T _value;

    public InteractionHandler(
        DiscordSocketClient client,
        ILogger<DiscordClientService> logger,
        InteractionService interactionService,
        IServiceProvider serviceProvider,
        ChainHandlerBuilder chainHandlerBuilder,
        ConfigureInteractionServiceBeforeStart<T> configureInteractionServiceBeforeStart,
        ConfigureInteractionService<T> configureInteractionService,
        ConfigureFinalHandler configureFinalHandler,
        T value) : base(client, logger)
    {
        _interactionService = interactionService;
        _serviceProvider = serviceProvider;
        _chainHandlerBuilder = chainHandlerBuilder;
        _configureInteractionService = configureInteractionService;
        _configureFinalHandler = configureFinalHandler;
        _value = value;
        _configureInteractionServiceBeforeStart = configureInteractionServiceBeforeStart;
    }
    
    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        _configureInteractionServiceBeforeStart.Invoke(_interactionService, _value);
        
        var chainHandlers = _serviceProvider.GetServices<ChainHandler>();

        foreach (var chainHandler in chainHandlers)
        {
            _chainHandlerBuilder.Add(chainHandler);
        }
        
        Client.InteractionCreated += _chainHandlerBuilder.Build().Handle;
        _interactionService.InteractionExecuted += HandleInteractionExecute;
        
        await _interactionService.AddModulesAsync(Assembly.GetEntryAssembly(), _serviceProvider);
        await Client.WaitForReadyAsync(cancellationToken);

        _configureInteractionService.Invoke(_interactionService, _value);
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