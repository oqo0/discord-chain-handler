namespace Discord.Addons.ChainHandler;

public abstract class InteractionChainHandler : IChainHandler
{
    private IChainHandler? _nextChainHandler;
    private readonly IServiceProvider _provider;
    private readonly InteractionService _interactionService;
    private readonly DiscordSocketClient _socketClient;

    protected InteractionChainHandler(
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

    public virtual Task<IResult> Handle(SocketInteraction interaction)
    {
        if (_nextChainHandler is not null)
        {
            return _nextChainHandler.Handle(interaction);
        }
        
        var context = new SocketInteractionContext(_socketClient, interaction);
        return _interactionService.ExecuteCommandAsync(context, _provider);
    }
}