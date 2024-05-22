using Discord.Addons.ChainHandlers.ChainHandlers;
using Discord.Addons.ChainHandlers.Common;
using Discord.Addons.ChainHandlers.Default;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;

namespace Samples.Custom.ChainHandlers;

public class RateLimiter : ChainHandler
{
    private readonly Dictionary<ulong, UserRequestInfo> _userRequests = new();
    
    public RateLimiter(IServiceProvider provider,
        InteractionService interactionService,
        DiscordSocketClient client,
        ILogger<ErrorChainHandler> logger) : base(provider, interactionService, client) { }

    public override async Task<IResult> Handle(SocketInteraction interaction)
    {
        var result = _userRequests.TryGetValue(interaction.User.Id, out var value);

        if (!result)
        {
            _userRequests[interaction.User.Id] = new UserRequestInfo();
            return await base.Handle(interaction);
        }

        if (_userRequests[interaction.User.Id].IsRequestAvailable())
        {
            return await base.Handle(interaction);
        }

        await interaction.RespondAsync("You are making too many requests!");
        return InteractionResult.UnhandledException;
    }
}