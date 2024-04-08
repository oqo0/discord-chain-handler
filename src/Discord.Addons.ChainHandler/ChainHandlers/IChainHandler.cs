namespace Discord.Addons.ChainHandler.ChainHandlers;

public interface IChainHandler
{
    public IChainHandler SetNext(IChainHandler chainHandler);
    public Task<IResult> Handle(SocketInteraction interaction);
}