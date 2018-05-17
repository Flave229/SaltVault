using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SaltVault.Core.People;
using SaltVault.Core.Services.Discord.Models;
using SaltVault.Core.Shopping;

namespace SaltVault.Core.Services.Discord.Commands
{
    public class ListShoppingCommand : IDiscordCommand
    {
        private readonly IDiscordService _discordService;
        private readonly IShoppingRepository _shoppingRepository;
        private readonly IPeopleRepository _peopleRepository;

        public ListShoppingCommand(IDiscordService discordService, IShoppingRepository shoppingRepository, IPeopleRepository peopleRepository)
        {
            _discordService = discordService;
            _shoppingRepository = shoppingRepository;
            _peopleRepository = peopleRepository;
        }

        public void ExecuteCommand(List<string> subCommands)
        {
            var shoppingItems = _shoppingRepository.GetAllItems(new Pagination
            {
                Page = 0,
                ResultsPerPage = int.MaxValue
            }, true);
            string title;

            if (subCommands.Count > 0)
            {
                var discordUserId = subCommands[0].Replace("<", "").Replace("@", "").Replace(">", "");
                var discordUser = _peopleRepository.GetPersonFromDiscordId(discordUserId);
                shoppingItems.ShoppingList = shoppingItems.ShoppingList.Where(x => x.AddedFor.Any(y => y.Id == discordUser.Id)).ToList();

                if (shoppingItems.ShoppingList.Count == 0)
                {
                    _discordService.SendMessage(new DiscordMessage { content = "You have no shopping items!" });
                    return;
                }
                title = $"Shopping List for {discordUser.FirstName} {discordUser.LastName}";
            }
            else
            {
                title = "Shopping List For All Users";
            }

            var discordMessage = new DiscordMessage
            {
                embed = new DiscordMessageEmbed
                {
                    author = new DiscordMessageAuthor
                    {
                        icon_url = "https://127xwr2qcfsvmn8a91nbd428-wpengine.netdna-ssl.com/wp-content/uploads/2013/01/Pile-of-salt.jpg",
                        name = title
                    },
                    fields = shoppingItems.ShoppingList.Select(shoppingItem => new DiscordMessageField
                    {
                        name = $"{shoppingItem.Name}",
                        value = $"For {string.Join(", ", shoppingItem.AddedFor.Select(x => x.FirstName))}"
                    }).ToList()
                }
            };
            _discordService.SendMessage(discordMessage);
        }
    }
}
