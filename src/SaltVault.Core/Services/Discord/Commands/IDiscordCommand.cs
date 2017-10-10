using System.Collections.Generic;

namespace SaltVault.Core.Services.Discord.Commands
{
    public interface IDiscordCommand
    {
        void ExecuteCommand(List<string> subCommands);
    }
}