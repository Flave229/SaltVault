using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using SaltVault.Core.Bills;
using SaltVault.Core.People;
using SaltVault.Core.Services.Discord.Models;
using SaltVault.Core.Shopping;

namespace SaltVault.Core.Services.Discord
{
    public class DiscordMessageListener
    {
        private readonly IBillRepository _billRepository;
        private readonly IShoppingRepository _shoppingRepository;
        private readonly IPeopleRepository _peopleRepository;
        private readonly IDiscordService _discordService;
        private string _lastMessageId;

        public DiscordMessageListener(IBillRepository billRepository, IShoppingRepository shoppingRepository, IPeopleRepository peopleRepository, IDiscordService discordService)
        {
            _billRepository = billRepository;
            _shoppingRepository = shoppingRepository;
            _peopleRepository = peopleRepository;
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

                messageContent = messageContent.Substring(2);
                var commandWords = messageContent.Split(' ');

                if (commandWords[0].StartsWith("bills"))
                {
                    var allBills = _billRepository.GetAllBasicBillDetails();
                    var outstandingBills = allBills.Where(x => x.AmountPaid < x.TotalAmount).ToList();
                    outstandingBills.Reverse();

                    if (commandWords.Length > 1)
                    {
                        var discordUserId = commandWords[1].Replace("<", "").Replace("@", "").Replace(">", "");
                        var discordUser = _peopleRepository.GetPersonFromDiscordId(discordUserId);
                        outstandingBills = outstandingBills.Where(x => x.People.Any(y => y.Id == discordUser.Id)).ToList();
                        var billsForDiscordUser = new List<Bill>();
                        foreach (var bill in outstandingBills)
                        {
                            var paymentsForUser = bill.Payments.Where(x => x.Person.Id == discordUser.Id);
                            var totalPaid = paymentsForUser.Sum(x => x.Amount);

                            if (totalPaid + 0.01m < bill.TotalAmount / bill.People.Count)
                                billsForDiscordUser.Add(bill);
                        }

                        if (billsForDiscordUser.Count == 0)
                        {
                            _discordService.SendMessage(new DiscordMessage { content = "You have no outstanding bills!" });
                            continue;
                        }

                        var discordMessage = new DiscordMessage
                        {
                            embed = new DiscordMessageEmbed
                            {
                                author = new DiscordMessageAuthor
                                {
                                    icon_url = discordUser.Image,
                                    name = discordUser.FirstName + " " + discordUser.LastName
                                },
                                title = "Outstanding Bills For " + discordUser.FirstName + " " + discordUser.LastName,
                                fields = billsForDiscordUser.Select(x => new DiscordMessageField
                                {
                                    name = x.Name,
                                    value = $"£{x.TotalAmount} `[{x.FullDateDue:MMM dd}]`"
                                }).ToList()
                            }
                        };

                        _discordService.SendMessage(discordMessage);
                        continue;
                    }

                    if (outstandingBills.Count == 0)
                    {
                        _discordService.SendMessage(new DiscordMessage { content = "You have no outstanding bills!" });
                        continue;
                    }

                    var outstandingBillMessage = "__**Outstanding Bills:**__\n\n";
                    foreach (var outstandingBill in outstandingBills)
                        outstandingBillMessage += $"`[{outstandingBill.FullDateDue:MMM dd}]` **{outstandingBill.Name}** for £{outstandingBill.TotalAmount}\n";
                    
                    _discordService.SendMessage(new DiscordMessage { content = outstandingBillMessage });
                }
                else if (commandWords[0].StartsWith("shopping"))
                {
                    var shoppingItems = _shoppingRepository.GetAllItems(true);

                    var shoppingItemMessage = "__**Shopping List:**__\n\n";
                    foreach (var shoppingItem in shoppingItems.ShoppingList)
                        shoppingItemMessage += $"**{shoppingItem.Name}** for {string.Join(", ", shoppingItem.AddedFor.Select(x => x.FirstName))}\n";

                    _discordService.SendMessage(new DiscordMessage { content = shoppingItemMessage });
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