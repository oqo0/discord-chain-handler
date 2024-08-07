using Discord.Addons.ChainHandlers.Configuration.Options;

namespace Discord.Addons.ChainHandlers.Configuration;

public delegate void ConfigureInteractionHandlerOptions(InteractionHandlerOptions interactionHandlerOptions);
public delegate void ConfigureChainHandler(ChainHandlerOptions chainHandlerOptions);
public delegate void ConfigureFinalHandler(IInteractionContext interactionContext);
public delegate void ConfigureInteractionServiceBeforeStart<in TValue>(InteractionService interactionService, TValue value);
public delegate void ConfigureInteractionService<in TValue>(InteractionService interactionService, TValue value);