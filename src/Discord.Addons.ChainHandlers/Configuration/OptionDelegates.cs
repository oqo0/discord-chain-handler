using Discord.Addons.ChainHandlers.Configuration.Options;
using Microsoft.Extensions.Configuration;

namespace Discord.Addons.ChainHandlers.Configuration;

public delegate void ConfigureInteractionHandlerOptions(InteractionHandlerOptions interactionHandlerOptions);
public delegate void ConfigureChainHandler(ChainHandlerOptions chainHandlerOptions);
public delegate void ConfigureFinalHandler(IInteractionContext interactionContext);
public delegate void ConfigureInteractionService<in TValue>(InteractionService interactionService, TValue value);