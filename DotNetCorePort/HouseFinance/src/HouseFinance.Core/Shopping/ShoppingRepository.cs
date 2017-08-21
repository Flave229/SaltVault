using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Npgsql;

namespace HouseFinance.Core.Shopping
{
    public class ShoppingRepository
    {
        private readonly NpgsqlConnection _connection;

        public ShoppingRepository()
        {
            var connectionString = File.ReadAllText("./Data/Config/LIVEConnectionString.config");
            _connection = new NpgsqlConnection(connectionString);
        }

        public void EnterAllIntoDatabase(List<ShoppingItem> shoppingItems)
        {
            _connection.Open();

            foreach (var shoppingItem in shoppingItems)
            {
                var addedBy = -1;
                if (shoppingItem.AddedBy == new Guid("e9636bbb-8b54-49b9-9fa2-9477c303032f"))
                    addedBy = 1;
                else if (shoppingItem.AddedBy == new Guid("25c15fb4-b5d5-47d9-917b-c572b1119e65"))
                    addedBy = 2;
                else if (shoppingItem.AddedBy == new Guid("f97a50c9-8451-4537-bccb-e89ba5ade95a"))
                    addedBy = 3;

                var command = new NpgsqlCommand("INSERT INTO public.\"ShoppingItem\" (\"Name\", \"Purchased\", \"AddedBy\", \"AddedOn\") " +
                                                $"VALUES ('{shoppingItem.Name}', {shoppingItem.Purchased.ToString().ToUpper()}, {addedBy}, '{shoppingItem.Added}') " +
                                                "RETURNING \"Id\"", _connection);
                var reader = command.ExecuteReader();
                Int64 rowId = -1;
                while (reader.Read())
                {
                    rowId = (Int64)reader[0];
                }
                reader.Close();

                foreach (var person in shoppingItem.ItemFor)
                {
                    var personId = -1;

                    if (person == new Guid("e9636bbb-8b54-49b9-9fa2-9477c303032f"))
                        personId = 1;
                    else if (person == new Guid("25c15fb4-b5d5-47d9-917b-c572b1119e65"))
                        personId = 2;
                    else if (person == new Guid("f97a50c9-8451-4537-bccb-e89ba5ade95a"))
                        personId = 3;

                    var shoppingItemForCommand = new NpgsqlCommand("INSERT INTO public.\"ShoppingItemFor\" (\"ShoppingItemId\", \"PersonId\") " +
                                                                   $"VALUES ({rowId}, {personId})", _connection);
                    reader = shoppingItemForCommand.ExecuteReader();
                    while (reader.Read())
                    {
                    }
                    reader.Close();
                }
            }

            _connection.Close();
        }

        public ShoppingListResponseV2 GetAllItems()
        {
            _connection.Open();

            try
            {
                var shoppingItems = new List<ItemV2>();
                var command = new NpgsqlCommand("SELECT Item.\"Id\", Item.\"Name\", Item.\"AddedOn\", Item.\"Purchased\", AddedBy.\"Image\", ShoppingItemFor.\"Image\" " +
                                                "FROM public.\"ShoppingItem\" AS Item " +
                                                "LEFT OUTER JOIN \"Person\" AS AddedBy ON AddedBy.\"Id\" = Item.\"AddedBy\" " +
                                                "LEFT OUTER JOIN \"ShoppingItemFor\" AS ItemPersonLinker ON ItemPersonLinker.\"ShoppingItemId\" = Item.\"Id\" " +
                                                "LEFT OUTER JOIN \"Person\" AS ShoppingItemFor ON ItemPersonLinker.\"PersonId\" = ShoppingItemFor.\"Id\" " +
                                                "ORDER BY Item.\"Purchased\", Item.\"AddedOn\" DESC", _connection);
                var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    var shoppingItemId = Convert.ToInt32(reader[0]);
                    ItemV2 shoppingItem;

                    if (shoppingItems.Any(x => x.Id == shoppingItemId))
                        shoppingItem = shoppingItems.First(x => x.Id == shoppingItemId);
                    else
                    {
                        shoppingItem = new ItemV2
                        {
                            Id = shoppingItemId,
                            Name = (string)reader[1],
                            DateAdded = (DateTime)reader[2],
                            Purchased = (bool)reader[3],
                            AddedByImage = (string)reader[4]
                        };
                    }

                    shoppingItem.AddedForImages.Add((string)reader[5]);

                    if (shoppingItems.Any(x => x.Id == shoppingItemId) == false)
                        shoppingItems.Add(shoppingItem);
                }

                reader.Close();
                _connection.Close();
                return new ShoppingListResponseV2
                {
                    ShoppingList = shoppingItems
                };
            }
            catch (Exception exception)
            {
                _connection.Close();
                throw new Exception("An Error occured while getting the bills", exception);
            }
        }

        public void AddItem(AddShoppingItemRequest shoppingRequest)
        {
            _connection.Open();

            try
            {
                var command = new NpgsqlCommand("INSERT INTO public.\"ShoppingItem\" (\"Name\", \"AddedOn\", \"AddedBy\", \"Purchased\") " +
                                                $"VALUES ('{shoppingRequest.Name}', {shoppingRequest.Added}, '{shoppingRequest.AddedBy}', FALSE) " +
                                                "RETURNING \"Id\"", _connection);
                Int64 itemId = -1;
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    itemId = (Int64)reader[0];
                }
                reader.Close();

                foreach (var peopleId in shoppingRequest.ItemFor)
                {
                    command = new NpgsqlCommand("INSERT INTO public.\"ShoppingItemFor\" (\"ShoppingItemId\", \"PersonId\") " +
                                                $"VALUES ({itemId}, {peopleId})", _connection);
                    reader = command.ExecuteReader();
                    while (reader.Read())
                    { }
                    reader.Close();
                }
                _connection.Close();
            }
            catch (Exception exception)
            {
                _connection.Close();
                throw new Exception($"An Error occured while adding the shopping item '{shoppingRequest.Name}'", exception);
            }
        }
    }

    public class AddShoppingItemRequest
    {
        public string Name { get; set; }
        public DateTime Added { get; set; }
        public int AddedBy { get; set; }
        public List<int> ItemFor { get; set; }

        public AddShoppingItemRequest()
        {
            ItemFor = new List<int>();
        }
    }
}