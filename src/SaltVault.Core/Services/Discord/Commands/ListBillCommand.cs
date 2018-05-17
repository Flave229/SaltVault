using System;
using System.Collections.Generic;
using System.Linq;
using SaltVault.Core.Bills;
using SaltVault.Core.People;
using SaltVault.Core.Services.Discord.Models;

namespace SaltVault.Core.Services.Discord.Commands
{
    class ListBillCommand : IDiscordCommand
    {
        private readonly IDiscordService _discordService;
        private readonly IBillRepository _billRepository;
        private readonly IPeopleRepository _peopleRepository;

        public ListBillCommand(IDiscordService discordService, IBillRepository billRepository, IPeopleRepository peopleRepository)
        {
            _discordService = discordService;
            _billRepository = billRepository;
            _peopleRepository = peopleRepository;
        }

        public void ExecuteCommand(List<string> subCommands)
        {
            var allBills = _billRepository.GetAllBasicBillDetails(new Pagination
            {
                Page = 0,
                ResultsPerPage = int.MaxValue
            });
            var outstandingBills = allBills.Where(x => x.AmountPaid < x.TotalAmount).ToList();
            outstandingBills.Reverse();

            if (subCommands.Count > 0)
            {
                var discordUserId = subCommands[0].Replace("<", "").Replace("@", "").Replace(">", "");
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
                    return;
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
                            value = $"`[{x.FullDateDue:MMM dd}]` £{x.TotalAmount}"
                        }).ToList()
                    }
                };

                _discordService.SendMessage(discordMessage);
                return;
            }

            if (outstandingBills.Count == 0)
            {
                _discordService.SendMessage(new DiscordMessage { content = "You have no outstanding bills!" });
                return;
            }
            
            var discordMessage2 = new DiscordMessage
            {
                embed = new DiscordMessageEmbed
                {
                    author = new DiscordMessageAuthor
                    {
                        icon_url = "https://127xwr2qcfsvmn8a91nbd428-wpengine.netdna-ssl.com/wp-content/uploads/2013/01/Pile-of-salt.jpg",
                        name = "Salt Automaton"
                    },
                    title = "Outstanding Bills For All Users",
                    fields = outstandingBills.Select(x => new DiscordMessageField
                    {
                        name = $"{x.Name} - {x.FullDateDue:dd MMMM}",
                        value = $"£{x.TotalAmount}"
                    }).ToList()
                }
            };

            _discordService.SendMessage(discordMessage2);
        }
    }
}
