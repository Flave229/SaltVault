using System.Collections.Generic;

namespace SaltVault.Core.Services.Discord.Models
{
    public class DiscordMessage
    {
        public string id { get; set; }
        public string content { get; set; }
        public DiscordMessageEmbed embed { get; set; }
    }

    public class DiscordMessageEmbed
    {
        public DiscordMessageAuthor author { get; set; }
        public string title { get; set; }
        public List<DiscordMessageField> fields { get; set; }
    }

    public class DiscordMessageField
    {
        public string name { get; set; }
        public string value { get; set; }
    }

    public class DiscordMessageAuthor
    {
        public string name { get; set; }
        public string icon_url { get; set; }
    }
}