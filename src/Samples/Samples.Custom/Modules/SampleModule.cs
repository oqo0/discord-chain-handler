using Discord.Interactions;

namespace Samples.Custom.Modules;

public class SampleModule : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("sample", "Sample command")]
    public async Task SampleCommand(string message, string test)
    {
        throw new InvalidOperationException("This is a sample bot");
    }
}