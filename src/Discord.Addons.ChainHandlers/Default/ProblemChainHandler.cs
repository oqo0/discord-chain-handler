using Microsoft.Extensions.Logging;

namespace Discord.Addons.ChainHandlers.Default;

public class ProblemChainHandler : ChainHandlers.ChainHandler
{
    private readonly ILogger<ProblemChainHandler> _logger;

    public ProblemChainHandler(IServiceProvider provider,
        InteractionService interactionService,
        DiscordSocketClient client,
        ILogger<ProblemChainHandler> logger) : base(provider, interactionService, client)
    {
        _logger = logger;
    }
    
    public override async Task<IResult> Handle(SocketInteraction interaction)
    {
        var result = await base.Handle(interaction);

        if (!result.IsSuccess)
        {
            _logger.LogWarning(
                "Problem ({Error}) occured while {User} ({UserId}) used interaction {InteractionId}: " +
                "{Problem}", result.Error, interaction.User.Username, interaction.User.Id, interaction.Id,
                result.ErrorReason);
        }

        return result;
    }
}