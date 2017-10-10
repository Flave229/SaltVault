using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using SaltVault.Core.Bills;
using SaltVault.Core.People;
using SaltVault.Core.Services.Discord.Commands;
using SaltVault.Core.Services.Discord.Models;
using SaltVault.Core.Shopping;

namespace SaltVault.Core.Services.Discord
{
    public class DiscordMessageListener
    {
        private readonly IDiscordService _discordService;
        private string _lastMessageId;
        private readonly Dictionary<string, IDiscordCommand> _discordCommands;

        public DiscordMessageListener(IBillRepository billRepository, IShoppingRepository shoppingRepository, IPeopleRepository peopleRepository, IDiscordService discordService)
        {
            _discordService = discordService;
            _discordCommands = new Dictionary<string, IDiscordCommand>
            {
                { "bills", new ListBillCommand(discordService, billRepository, peopleRepository) },
                { "shopping", new ListShoppingCommand(discordService, shoppingRepository, peopleRepository) }
            };
        }

        public void ProcessNewMessages()
        {
            var discordMessages = _discordService.GetRecentDiscordMessages(_lastMessageId);

            if (_lastMessageId == null)
            {
                _lastMessageId = discordMessages[0].id;
                discordMessages = _discordService.GetRecentDiscordMessages(_lastMessageId);
            }
            discordMessages.Reverse();

            foreach (var message in discordMessages)
            {
                _lastMessageId = message.id;
                var messageContent = message.content;
                if (messageContent.StartsWith("//") == false)
                    continue;

                messageContent = messageContent.Substring(2);
                var commandWords = messageContent.Split(' ');

                if (_discordCommands.ContainsKey(commandWords[0]))
                    _discordCommands[commandWords[0]].ExecuteCommand(commandWords.Skip(1).ToList());
            }
        }

        public void StartWorker()
        {
            while (true)
            {
                try
                {
                    ProcessNewMessages();
                    Thread.Sleep(2000);
                }
                catch (Exception exception)
                {
                    Console.WriteLine("The DiscordMessageListener Worker encountered a problem and restarted: " + exception);
                    Thread.Sleep(2000);
                }
            }
        }
    }
}