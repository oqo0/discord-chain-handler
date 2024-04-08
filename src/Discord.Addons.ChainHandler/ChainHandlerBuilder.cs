using Discord.Addons.ChainHandler.ChainHandlers;
using Microsoft.Extensions.DependencyInjection;

namespace Discord.Addons.ChainHandler;

public class ChainHandlerBuilder
{
    private readonly IServiceProvider _serviceProvider;
    private IChainHandler? _chainHandler;
    private IChainHandler? _lastChainHandler;

    public ChainHandlerBuilder(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public ChainHandlerBuilder Add<T>() where T : IChainHandler
    {
        var chainHandlerToAdd = _serviceProvider.GetRequiredService<T>();

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