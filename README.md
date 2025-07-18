﻿# Discord.Net ChainHandler

Chain handlers are meant to make using of Discord.Net interaction easier.
Chain handler that will act as a layer between your commands and the user.

### Example

```csharp
serviceCollection.AddInteractionHandler(options =>
{
    options.UseChainHandler(handlerOptions =>
    {
        handlerOptions
            .Add<ErrorChainHandler>()
            .Add<ProblemChainHandler>();
    });
    options.UseFinalHandler(async interactionContext =>
    {
        await interactionContext.Interaction.RespondAsync(
            "Something bad happened!", ephemeral: true);
    });
    options.ConfigureInteractionService(async (interactionService, _) =>
    {
        var stagingGuildId = configuration.GetValue<ulong>("GuildId");

        await interactionService.AddModulesGloballyAsync(
            true,
            Array.Empty<Discord.Interactions.ModuleInfo>());
        await interactionService.AddModulesToGuildAsync(
            stagingGuildId, 
            true,
            Array.Empty<Discord.Interactions.ModuleInfo>());
    
        await interactionService.RegisterCommandsToGuildAsync(stagingGuildId);
    }, configuration);
});
```

Full sample code can be found at [./src/Samples/](/src/Samples)
