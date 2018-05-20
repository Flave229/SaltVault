using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Npgsql;
using SaltVault.Core.People;
using SaltVault.Core.Shopping.Models;

namespace SaltVault.Core.Shopping
{
    public interface IShoppingRepository
    {
        List<Item> GetAllItems(Pagination pagination, int userHouseId, bool onlyUnpurchasedItems = false);
        void AddItem(AddShoppingItemRequest shoppingRequest, int userHouseId);
        void UpdateItem(UpdateShoppingItemRequest shoppingRequest);
        void DeleteItem(int shoppingItemId);
    }

    public class ShoppingRepository : IShoppingRepository
    {
        private readonly NpgsqlConnection _connection;

        public ShoppingRepository()
        {
            var connectionString = File.ReadAllText("./Data/Config/LIVEConnectionString.config");
            _connection = new NpgsqlConnection(connectionString);
        }

        public List<Item> GetAllItems(Pagination pagination, int userHouseId, bool onlyUnpurchasedItems = false)
        {
            _connection.Open();

            try
            {
                var shoppingItems = new List<Item>();
                var whereClause = $"WHERE Item.\"HouseId\" = {userHouseId} ";

                if (onlyUnpurchasedItems)
                    whereClause += "AND Item.\"Purchased\" = false ";

                var command = new NpgsqlCommand("SELECT Item.\"Id\", Item.\"Name\", Item.\"AddedOn\", Item.\"Purchased\", AddedBy.\"Id\", AddedBy.\"FirstName\", AddedBy.\"LastName\", AddedBy.\"Image\", " +
                                                "ShoppingItemFor.\"Id\", ShoppingItemFor.\"FirstName\", ShoppingItemFor.\"LastName\", ShoppingItemFor.\"Image\" " +
                                                "FROM public.\"ShoppingItem\" AS Item " +
                                                "LEFT OUTER JOIN \"Person\" AS AddedBy ON AddedBy.\"Id\" = Item.\"AddedBy\" " +
                                                "LEFT OUTER JOIN \"ShoppingItemFor\" AS ItemPersonLinker ON ItemPersonLinker.\"ShoppingItemId\" = Item.\"Id\" " +
                                                "LEFT OUTER JOIN \"Person\" AS ShoppingItemFor ON ItemPersonLinker.\"PersonId\" = ShoppingItemFor.\"Id\" " +
                                                whereClause +
                                                $"ORDER BY Item.\"Purchased\", Item.\"AddedOn\" DESC OFFSET {pagination.Page * pagination.ResultsPerPage} FETCH NEXT {pagination.ResultsPerPage} ROWS ONLY", _connection);
                var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    var shoppingItemId = Convert.ToInt32(reader[0]);
                    Item shoppingItem;

                    if (shoppingItems.Any(x => x.Id == shoppingItemId))
                        shoppingItem = shoppingItems.First(x => x.Id == shoppingItemId);
                    else
                    {
                        shoppingItem = new Item
                        {
                            Id = shoppingItemId,
                            Name = (string)reader[1],
                            DateAdded = (DateTime)reader[2],
                            Purchased = (bool)reader[3],
                            AddedBy = new Person
                            {
                                Id = Convert.ToInt32(reader[4]),
                                FirstName = (string)reader[5],
                                LastName = (string)reader[6],
                                Image = (string)reader[7]
                            }
                        };
                    }
                    
                    shoppingItem.AddedFor.Add(new Person
                    {
                        Id = Convert.ToInt32(reader[8]),
                        FirstName = (string)reader[9],
                        LastName = (string)reader[10],
                        Image = (string)reader[11]
                    });

                    if (shoppingItems.Any(x => x.Id == shoppingItemId) == false)
                        shoppingItems.Add(shoppingItem);
                }

                reader.Close();
                return shoppingItems;
            }
            catch (System.Exception exception)
            {
                throw new System.Exception("An Error occured while getting the bills", exception);
            }
            finally
            {
                _connection.Close();
            }
        }

        public void AddItem(AddShoppingItemRequest shoppingRequest, int userHouseId)
        {
            _connection.Open();

            try
            {
                var command = new NpgsqlCommand("INSERT INTO public.\"ShoppingItem\" (\"Name\", \"AddedOn\", \"AddedBy\", \"Purchased\", \"HouseId\") " +
                                                $"VALUES ('{shoppingRequest.Name}', '{shoppingRequest.Added:yyyy-MM-dd}', {shoppingRequest.AddedBy}, FALSE, {userHouseId}) " +
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
            }
            catch (System.Exception exception)
            {
                throw new System.Exception($"An Error occured while adding the shopping item '{shoppingRequest.Name}'", exception);
            }
            finally
            {
                _connection.Close();
            }
        }

        public void UpdateItem(UpdateShoppingItemRequest shoppingRequest)
        {
            _connection.Open();

            try
            {
                var setValues = new List<string>();

                if (shoppingRequest.Name != null)
                    setValues.Add($"\"Name\"='{shoppingRequest.Name}'");
                if (shoppingRequest.Purchased != null)
                    setValues.Add($"\"Purchased\"={shoppingRequest.Purchased.ToString().ToUpper()}");

                NpgsqlCommand command;
                NpgsqlDataReader reader;
                if (setValues.Count > 0)
                {
                    command = new NpgsqlCommand("UPDATE public.\"ShoppingItem\" " +
                                                $"SET {string.Join(", ", setValues)} " +
                                                $"WHERE \"Id\" = {shoppingRequest.Id}", _connection);

                    reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                    }
                    reader.Close();
                }

                if (shoppingRequest.ItemFor == null || shoppingRequest.ItemFor.Count == 0)
                    return;

                command = new NpgsqlCommand("DELETE FROM public.\"ShoppingItemFor\" " +
                                            $"WHERE \"ShoppingItemId\" = {shoppingRequest.Id}", _connection);
                reader = command.ExecuteReader();
                while (reader.Read())
                {
                }
                reader.Close();

                foreach (var peopleId in shoppingRequest.ItemFor)
                {
                    command = new NpgsqlCommand(
                        "INSERT INTO public.\"ShoppingItemFor\" (\"ShoppingItemId\", \"PersonId\") " +
                        $"VALUES ({shoppingRequest.Id}, {peopleId})", _connection);
                    reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                    }
                    reader.Close();
                }
            }
            catch (System.Exception exception)
            {
                throw new System.Exception($"An Error occured while updating the payment (ID: {shoppingRequest.Id})",
                    exception);
            }
            finally
            {
                _connection.Close();
            }
        }

        public void DeleteItem(int shoppingItemId)
        {
            _connection.Open();

            try
            {
                var command = new NpgsqlCommand("DELETE FROM public.\"ShoppingItemFor\" " +
                                                $"WHERE \"ShoppingItemId\" = {shoppingItemId}", _connection);
                var reader = command.ExecuteReader();
                while (reader.Read())
                { }
                reader.Close();

                command = new NpgsqlCommand("DELETE FROM public.\"ShoppingItem\" " +
                                            $"WHERE \"Id\" = {shoppingItemId}", _connection);
                
                reader = command.ExecuteReader();
                while (reader.Read())
                { }
                reader.Close();
            }
            catch (System.Exception exception)
            {
                throw new System.Exception($"An Error occured while deleting the shopping item (ID: {shoppingItemId})", exception);
            }
            finally
            {
                _connection.Close();
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