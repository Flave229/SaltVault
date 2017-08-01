using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using HouseFinance.Core.Services.Discord.Models;
using Newtonsoft.Json;

namespace HouseFinance.Core.Services.Discord
{
    public class DiscordService
    {
        private readonly DiscordSocketClient _discordClient;
        private SocketGuildChannel _channel;
        private HttpClient _discordHttpClient;
        private string _botToken;

        public DiscordService(DiscordSocketClient discordClient)
        {
            _discordClient = discordClient;
            Connect();
            _discordHttpClient = new HttpClient { BaseAddress = new Uri("https://discordapp.com/api/v6/") };
            _discordHttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bot", _botToken);
        }

        public async void Connect()
        {
            var filePath = "Data/Discord/bot_token.txt";
            if (!System.IO.File.Exists(filePath))
                Console.WriteLine("Failed to fetch the bot token to log in to Discord");

            var fileContents = System.IO.File.ReadAllLines(filePath);
            _botToken = string.Join(",", fileContents);

            _discordClient.MessageReceived += MessageReceived;

            await _discordClient.LoginAsync(TokenType.Bot, _botToken);
            await _discordClient.StartAsync();
            
            await Task.Delay(-1);
        }

        public void AddBillNotification(string name, DateTime date, decimal amount)
        {
            var discordMessage = new DiscordMessage
            {
                content = $"A new bill has been added! The new bill, '{name}', is £{amount:###0.00} and is due on {date:d}."
            };
            var postContent = new StringContent(JsonConvert.SerializeObject(discordMessage), Encoding.UTF8, "application/json");
            var result = _discordHttpClient.PostAsync("channels/340434832310009856/messages", postContent).Result;
        }

        private async Task MessageReceived(SocketMessage message)
        {
            _channel = message.Channel as SocketGuildChannel;
            if (message.Content == "!ping")
                await message.Channel.SendMessageAsync("Pong!");
        }
    }
}
