using System;
using System.Linq;
using System.Threading;
using SaltVault.Core.Bills;
using SaltVault.Core.Shopping;

namespace SaltVault.Core.Services.Discord
{
    public class DiscordMessageListener
    {
        private readonly IBillRepository _billRepository;
        private readonly IShoppingRepository _shoppingRepository;
        private readonly IDiscordService _discordService;
        private string _lastMessageId;

        public DiscordMessageListener(IBillRepository billRepository, IShoppingRepository shoppingRepository, IDiscordService discordService)
        {
            _billRepository = billRepository;
            _shoppingRepository = shoppingRepository;
            _discordService = discordService;
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

                var command = messageContent.Substring(2);

                if (command.Trim().StartsWith("bills"))
                {
                    var allBills = _billRepository.GetAllBasicBillDetails();
                    var outstandingBills = allBills.Where(x => x.AmountPaid < x.TotalAmount).ToList();
                    outstandingBills.Reverse();

                    if (outstandingBills.Count == 0)
                    {
                        _discordService.SendMessage("You have no outstanding bills!");
                        continue;
                    }

                    var outstandingBillMessage = "__**Outstanding Bills:**__\n\n";
                    foreach (var outstandingBill in outstandingBills)
                        outstandingBillMessage += $"`[{outstandingBill.FullDateDue:MMM dd}]` **{outstandingBill.Name}** for £{outstandingBill.TotalAmount}\n";
                    
                    _discordService.SendMessage(outstandingBillMessage);
                }
                else if (command.Trim().StartsWith("shopping"))
                {
                    var shoppingItems = _shoppingRepository.GetAllItems(true);

                    var shoppingItemMessage = "__**Shopping List:**__\n\n";
                    foreach (var shoppingItem in shoppingItems.ShoppingList)
                        shoppingItemMessage += $"**{shoppingItem.Name}** for {string.Join(", ", shoppingItem.AddedFor.Select(x => x.FirstName))}\n";

                    _discordService.SendMessage(shoppingItemMessage);
                }
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