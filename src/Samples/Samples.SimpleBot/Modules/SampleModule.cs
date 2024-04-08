using Discord.Interactions;

namespace Samples.SimpleBot.Modules;

public class SampleModule : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("sample", "Sample command")]
    public async Task SampleCommand(string message, string test)
    {
        throw new NotImplementedException();
    }
}