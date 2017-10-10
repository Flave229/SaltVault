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

            var shoppingItemMessage = "__**Shopping List:**__\n\n";
            foreach (var shoppingItem in shoppingItems.ShoppingList)
                shoppingItemMessage += $"**{shoppingItem.Name}** for {string.Join(", ", shoppingItem.AddedFor.Select(x => x.FirstName))}\n";

            _discordService.SendMessage(new DiscordMessage { content = shoppingItemMessage });
        }
    }
}
