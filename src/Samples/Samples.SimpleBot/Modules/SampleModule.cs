using Discord.Interactions;

namespace Samples.SimpleBot.Modules;

internal class SampleModule : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("sample", "Sample command")]
    internal async Task SampleCommand(string message, string test)
    {
        throw new NotImplementedException();
    }
}