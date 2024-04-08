using Discord.Addons.ChainHandler.Common;
using Microsoft.Extensions.Logging;

namespace Discord.Addons.ChainHandler.Default;

public class ErrorChainHandler : ChainHandlers.ChainHandler
{
    private readonly ILogger<ErrorChainHandler> _logger;

    public ErrorChainHandler(IServiceProvider provider,
        InteractionService interactionService,
        DiscordSocketClient client,
        ILogger<ErrorChainHandler> logger) : base(provider, interactionService, client)
    {
        _logger = logger;
    }

    public override async Task<IResult> Handle(SocketInteraction interaction)
    {
        try
        {
            return await base.Handle(interaction);
        }
        catch (Exception exception)
        {
            await HandleInteractionError(interaction, exception);
            return InteractionResult.UnhandledException;
        }
    }
    
    private async Task HandleInteractionError(SocketInteraction interaction, Exception exception)
    {
        _logger.LogError(
            "Interaction {InteractionId} error occured: {Exception}", 
            interaction.Id, exception.ToString());
        
        if (interaction.Type is InteractionType.ApplicationCommand)
        {
            await interaction.GetOriginalResponseAsync()
                .ContinueWith(msg=> msg.Result.DeleteAsync());
        }
    }
}