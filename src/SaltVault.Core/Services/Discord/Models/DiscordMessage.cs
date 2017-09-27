using System.Diagnostics.CodeAnalysis;

namespace SaltVault.Core.Services.Discord.Models
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class DiscordMessage
    {
        public string id;
        public string content { get; set; }
    }
}