# ⛓️ Discord.Net ChainHandlers

![C#](https://img.shields.io/badge/c%23-%23239120.svg?style=Flat&logo=csharp&logoColor=white)
![Stars](https://img.shields.io/github/stars/oqo0/discord-chain-handler.png)
![License](https://img.shields.io/github/license/oqo0/discord-chain-handler)
![Rease](https://img.shields.io/github/v/release/oqo0/discord-chain-handler)

A powerful and extensible middleware framework for Discord.Net interactions that implements the Chain of Responsibility pattern. This library provides a structured way to handle Discord interactions through a chain of handlers, making your bot code more modular, maintainable, and robust.

### Features

- **Chain of Responsibility Pattern**: Process interactions through a series of handlers
- **Modular Architecture**: Separate concerns into independent handlers
- **Error Handling**: Built-in error handling with custom exception management
- **Rate Limiting**: Prevent abuse with configurable rate limiting
- **Extensible**: Easy to create custom handlers for any functionality
- **Discord.Net Integration**: Seamlessly works with Discord.Net's InteractionService
- **Dependency Injection**: Full support for .NET's dependency injection

## 📦 Installation

```bash
dotnet add package Discord.Addons.ChainHandlers
```

## 🔧 Quick Start

### 1. Basic Setup

```csharp
var host = Host.CreateDefaultBuilder(args);

host.ConfigureDiscordHost((context, config) =>
{
    config.Token = context.Configuration["Token"];
})
.UseInteractionService()
.ConfigureServices((context, services) =>
{
    services.AddInteractionHandler(options =>
    {
        options.UseChainHandler(handlerOptions =>
        {
            handlerOptions
                .Add<ErrorChainHandler>()
                .Add<ProblemChainHandler>();
        });
    });
});

await host.Build().RunAsync();
```

> [!IMPORTANT]  
> **Order Matters**: Place handlers in logical order (logging → validation → business logic → error handling)

### 2. Create Your Interaction Modules

```csharp
public class ExampleModule : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("ping", "Get the bot's latency")]
    public async Task PingAsync()
    {
        await RespondAsync($"Pong! Latency: {Context.Client.Latency}ms");
    }

    [SlashCommand("echo", "Echo a message")]
    public async Task EchoAsync([Summary("message")] string message)
    {
        await RespondAsync($"You said: {message}");
    }
}
```

## 🛠️ Creating Custom Chain Handlers

This extension allows you to create any custom chain handler which is going to act as a middleware between interaction and a usecase.

> [!TIP]
> **Keep Handlers Focused**: Each handler should have a single responsibility

### Example: Permission Handler

```csharp
public class PermissionChainHandler : ChainHandler
{
    public PermissionChainHandler(
        IServiceProvider provider,
        InteractionService interactionService,
        DiscordSocketClient client) 
        : base(provider, interactionService, client) { }

    public override async Task<IResult> Handle(SocketInteraction interaction)
    {
        // Check if user has required permissions
        if (interaction.User is SocketGuildUser guildUser)
        {
            if (!guildUser.GuildPermissions.Administrator)
            {
                await interaction.RespondAsync(
                    "You need administrator permissions to use this command!", 
                    ephemeral: true);
                return InteractionResult.UnhandledException;
            }
        }

        return await base.Handle(interaction);
    }
}
```

### Example: Logging Handler

```csharp
public class LoggingChainHandler : ChainHandler
{
    private readonly ILogger<LoggingChainHandler> _logger;

    public LoggingChainHandler(
        IServiceProvider provider,
        InteractionService interactionService,
        DiscordSocketClient client,
        ILogger<LoggingChainHandler> logger) 
        : base(provider, interactionService, client)
    {
        _logger = logger;
    }

    public override async Task<IResult> Handle(SocketInteraction interaction)
    {
        _logger.LogInformation(
            "Interaction received: {Type} from {User} in {Channel}",
            interaction.Type,
            interaction.User.Username,
            (interaction.Channel as SocketGuildChannel)?.Guild.Name ?? "DM");

        var result = await base.Handle(interaction);

        _logger.LogInformation(
            "Interaction completed: {Type} with result {Success}",
            interaction.Type,
            result.IsSuccess);

        return result;
    }
}
```

### Example: Advanced Rate Limiter

```csharp
public class AdvancedRateLimiter : ChainHandler
{
    private readonly Dictionary<ulong, UserRateLimitInfo> _userRequests = new();
    private readonly TimeSpan _timeWindow = TimeSpan.FromMinutes(1);
    private readonly int _maxRequests = 10;

    public AdvancedRateLimiter(
        IServiceProvider provider,
        InteractionService interactionService,
        DiscordSocketClient client) 
        : base(provider, interactionService, client) { }

    public override async Task<IResult> Handle(SocketInteraction interaction)
    {
        var userId = interaction.User.Id;
        var now = DateTime.UtcNow;

        if (!_userRequests.ContainsKey(userId))
        {
            _userRequests[userId] = new UserRateLimitInfo();
        }

        var userInfo = _userRequests[userId];

        // Clean old requests
        userInfo.Requests.RemoveAll(time => now - time > _timeWindow);

        if (userInfo.Requests.Count >= _maxRequests)
        {
            var timeLeft = _timeWindow - (now - userInfo.Requests.First());
            await interaction.RespondAsync(
                $"Rate limit exceeded! Please wait {timeLeft:mm\\:ss} minutes.",
                ephemeral: true);
            return InteractionResult.UnhandledException;
        }

        userInfo.Requests.Add(now);
        return await base.Handle(interaction);
    }

    private class UserRateLimitInfo
    {
        public List<DateTime> Requests { get; } = new List<DateTime>();
    }
}
```

After creating a chain handler it needs to be injected into the DI container:
```csharp
handlerOptions
    .Add<ErrorChainHandler>()
    .Add<ProblemChainHandler>()
    // ...
```

## ⚙️ Configuration Options

### Chain Handler Configuration

```csharp
services.AddInteractionHandler(options =>
{
    // Add chain handlers in execution order
    options.UseChainHandler(handlerOptions =>
    {
        handlerOptions
            .Add<LoggingChainHandler>()      // First: Log all interactions
            .Add<PermissionChainHandler>()   // Second: Check permissions
            .Add<RateLimiterChainHandler>()  // Third: Rate limiting
            .Add<ErrorChainHandler>();       // Fourth: Error handling
    });

    // Final handler for unhandled errors
    options.UseFinalHandler(async interactionContext =>
    {
        await interactionContext.Interaction.RespondAsync(
            "An unexpected error occurred. Please try again later.",
            ephemeral: true);
    });

    // Configure the interaction service
    options.ConfigureInteractionService(async (interactionService, config) =>
    {
        var guildId = config.GetValue<ulong>("GuildId");
        await interactionService.AddModulesAsync(Assembly.GetEntryAssembly(), services.BuildServiceProvider());
        await interactionService.RegisterCommandsToGuildAsync(guildId);
    }, context.Configuration);
});
```

## 🎯 Advanced Usage

### Conditional Chain Execution

```csharp
public class ConditionalChainHandler : ChainHandler
{
    public override async Task<IResult> Handle(SocketInteraction interaction)
    {
        // Only process specific interaction types
        if (interaction.Type == InteractionType.ApplicationCommand)
        {
            // Special processing for application commands
            var commandInteraction = (SocketSlashCommand)interaction;
            
            if (commandInteraction.CommandName == "admin")
            {
                // Additional validation for admin commands
                return await ValidateAdminCommand(interaction);
            }
        }

        return await base.Handle(interaction);
    }

    private async Task<IResult> ValidateAdminCommand(SocketInteraction interaction)
    {
        // Admin command validation logic
        return await base.Handle(interaction);
    }
}
```

### Database Integration

```csharp
public class DatabaseChainHandler : ChainHandler
{
    private readonly ApplicationDbContext _dbContext;

    public DatabaseChainHandler(
        IServiceProvider provider,
        InteractionService interactionService,
        DiscordSocketClient client,
        ApplicationDbContext dbContext) 
        : base(provider, interactionService, client)
    {
        _dbContext = dbContext;
    }

    public override async Task<IResult> Handle(SocketInteraction interaction)
    {
        // Log interaction to database
        var interactionLog = new InteractionLog
        {
            InteractionId = interaction.Id.ToString(),
            UserId = interaction.User.Id,
            Type = interaction.Type.ToString(),
            ChannelId = interaction.Channel.Id,
            CreatedAt = DateTime.UtcNow
        };

        _dbContext.InteractionLogs.Add(interactionLog);
        await _dbContext.SaveChangesAsync();

        return await base.Handle(interaction);
    }
}
```

## 🔍 Error Handling

The library includes a built-in error handler, but you can create custom error handling:

```csharp
public class CustomErrorHandler : ChainHandler
{
    private readonly ILogger<CustomErrorHandler> _logger;

    public CustomErrorHandler(
        IServiceProvider provider,
        InteractionService interactionService,
        DiscordSocketClient client,
        ILogger<CustomErrorHandler> logger) 
        : base(provider, interactionService, client)
    {
        _logger = logger;
    }

    public override async Task<IResult> Handle(SocketInteraction interaction)
    {
        try
        {
            return await base.Handle(interaction);
        }
        catch (TimeoutException ex)
        {
            _logger.LogWarning(ex, "Timeout processing interaction {InteractionId}", interaction.Id);
            await interaction.RespondAsync("Request timed out. Please try again.", ephemeral: true);
            return InteractionResult.UnhandledException;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error processing interaction {InteractionId}", interaction.Id);
            await interaction.RespondAsync("An unexpected error occurred.", ephemeral: true);
            return InteractionResult.UnhandledException;
        }
    }
}
```

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🐛 Troubleshooting

### Common Issues

1. **Handlers not executing**: Ensure handlers are registered in the correct order
2. **Dependency injection issues**: Verify all handler dependencies are registered
3. **Interaction timeouts**: Check for long-running operations in handlers (Sometimes initial DB connection can last longer than 2 seconds causing an interaction timeout).

### Getting Help

- Check the [examples directory](/src/Samples/)
- Open an [issue](https://github.com/oqo0/discord-chain-handler/issues)

---

For more examples and advanced usage, check out the [Samples directory](/src/Samples/) in the repository.
