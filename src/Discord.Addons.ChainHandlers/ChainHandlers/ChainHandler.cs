namespace Discord.Addons.ChainHandlers.ChainHandlers;

public abstract class ChainHandler : IChainHandler
{
    private IChainHandler? _nextChainHandler;
    private readonly IServiceProvider _provider;
    private readonly InteractionService _interactionService;
    private readonly DiscordSocketClient _socketClient;

    protected ChainHandler(
        IServiceProvider provider,
        InteractionService interactionService,
        DiscordSocketClient socketClient)
    {
        _provider = provider;
        _interactionService = interactionService;
        _socketClient = socketClient;
    }
    
    public IChainHandler SetNext(IChainHandler chainHandler)
    {
        _nextChainHandler = chainHandler;
        return _nextChainHandler;
    }

    public virtual async Task<IResult> Handle(SocketInteraction interaction)
    {
        if (_nextChainHandler is not null)
        {
            return await _nextChainHandler.Handle(interaction);
        }
    
        var context = new SocketInteractionContext(_socketClient, interaction);
        return await _interactionService.ExecuteCommandAsync(context, _provider);
    }
}