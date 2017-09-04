using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;
using SaltVault.Core.Services.Discord.Models;

namespace SaltVault.Core.Services.Discord
{
    public interface IDiscordService
    {
        void AddBillNotification(string name, DateTime date, decimal amount);
    }

    public class DiscordService : IDiscordService
    {
        private readonly HttpClient _discordHttpClient;

        public DiscordService(HttpClient discordHttpClient)
        {
            _discordHttpClient = discordHttpClient;
            Connect();
        }

        private void Connect()
        {
            var filePath = "Data/Discord/bot_token.txt";
            if (!System.IO.File.Exists(filePath))
                Console.WriteLine("Failed to fetch the bot token to log in to Discord");

            var fileContents = System.IO.File.ReadAllLines(filePath);
            var botToken = string.Join(",", fileContents);

            _discordHttpClient.BaseAddress = new Uri("https://discordapp.com/api/v6/");
            _discordHttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bot", botToken);
        }

        public void AddBillNotification(string name, DateTime date, decimal amount)
        {
            var discordMessage = new DiscordMessage
            {
                content = $"A new bill has been added! The new bill, '{name}', is £{amount:###0.00} and is due on {date:dd/MM/yyyy}."
            };
            var postContent = new StringContent(JsonConvert.SerializeObject(discordMessage), Encoding.UTF8, "application/json");
            var result = _discordHttpClient.PostAsync("channels/340434832310009856/messages", postContent).Result;
        }
    }
}
