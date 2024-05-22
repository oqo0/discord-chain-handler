using Discord.Addons.ChainHandlers.ChainHandlers;

namespace Discord.Addons.ChainHandlers.Common;

public class ChainHandlerBuilder
{
    private IChainHandler? _chainHandler;
    private IChainHandler? _lastChainHandler;

    public ChainHandlerBuilder Add(ChainHandler chainHandlerToAdd)
    {
        if (_lastChainHandler is null)
        {
            _chainHandler = chainHandlerToAdd;
            _lastChainHandler = chainHandlerToAdd;
        }
        else
        {
            _lastChainHandler.SetNext(chainHandlerToAdd);
            _lastChainHandler = chainHandlerToAdd;
        }
        
        return this;
    }

    public IChainHandler Build()
    {
        if (_chainHandler is null)
        {
            throw new InvalidOperationException();
        }
        
        return _chainHandler;
    }
    
    private void Clear()
    {
        _chainHandler = null;
        _lastChainHandler = null;
    }
}