using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SaltVault.Core.Services.Discord.Models;
using SaltVault.Core.Shopping;

namespace SaltVault.Core.Services.Discord.Commands
{
    public class ListShoppingCommand : IDiscordCommand
    {
        private readonly IDiscordService _discordService;
        private readonly IShoppingRepository _shoppingRepository;

        public ListShoppingCommand(IDiscordService discordService, IShoppingRepository shoppingRepository)
        {
            _discordService = discordService;
            _shoppingRepository = shoppingRepository;
        }

        public void ExecuteCommand(List<string> subCommands)
        {
            var shoppingItems = _shoppingRepository.GetAllItems(true);

            var discordMessage = new DiscordMessage
            {
                embed = new DiscordMessageEmbed
                {
                    author = new DiscordMessageAuthor
                    {
                        icon_url = "https://127xwr2qcfsvmn8a91nbd428-wpengine.netdna-ssl.com/wp-content/uploads/2013/01/Pile-of-salt.jpg",
                        name = "Shopping List For All Users"
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
