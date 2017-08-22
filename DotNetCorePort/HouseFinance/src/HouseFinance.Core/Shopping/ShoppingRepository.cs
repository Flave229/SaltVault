﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using HouseFinance.Core.People;
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
        
        public ShoppingListResponse GetAllItems()
        {
            _connection.Open();

            try
            {
                var shoppingItems = new List<Item>();
                var command = new NpgsqlCommand("SELECT Item.\"Id\", Item.\"Name\", Item.\"AddedOn\", Item.\"Purchased\", AddedBy.\"Id\", AddedBy.\"FirstName\", AddedBy.\"LastName\", AddedBy.\"Image\", " +
                                                "ShoppingItemFor.\"Id\", ShoppingItemFor.\"FirstName\", ShoppingItemFor.\"LastName\", ShoppingItemFor.\"Image\" " +
                                                "FROM public.\"ShoppingItem\" AS Item " +
                                                "LEFT OUTER JOIN \"Person\" AS AddedBy ON AddedBy.\"Id\" = Item.\"AddedBy\" " +
                                                "LEFT OUTER JOIN \"ShoppingItemFor\" AS ItemPersonLinker ON ItemPersonLinker.\"ShoppingItemId\" = Item.\"Id\" " +
                                                "LEFT OUTER JOIN \"Person\" AS ShoppingItemFor ON ItemPersonLinker.\"PersonId\" = ShoppingItemFor.\"Id\" " +
                                                "ORDER BY Item.\"Purchased\", Item.\"AddedOn\" DESC", _connection);
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
                            },
                            AddedByImage = (string)reader[7]
                        };
                    }

                    shoppingItem.AddedForImages.Add((string)reader[11]);
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
                _connection.Close();
                return new ShoppingListResponse
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
                                                $"VALUES ('{shoppingRequest.Name}', '{shoppingRequest.Added:yyyy-MM-dd}', {shoppingRequest.AddedBy}, FALSE) " +
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

        public void UpdateItem(UpdateShoppingItemRequestV2 shoppingRequest)
        {
            _connection.Open();

            try
            {
                var setValues = new List<string>();

                if (shoppingRequest.Name != null)
                    setValues.Add($"\"Name\"='{shoppingRequest.Name}'");
                if (shoppingRequest.Purchased != null)
                    setValues.Add($"\"Purchased\"={shoppingRequest.Purchased.ToString().ToUpper()}");

                var command = new NpgsqlCommand("UPDATE public.\"ShoppingItem\" " +
                                                $"SET {string.Join(", ", setValues)} " +
                                                $"WHERE \"Id\" = {shoppingRequest.Id}", _connection);
                
                var reader = command.ExecuteReader();
                while (reader.Read())
                { }
                reader.Close();

                if (shoppingRequest.ItemFor == null || shoppingRequest.ItemFor.Count == 0)
                    return;

                command = new NpgsqlCommand("DELETE FROM public.\"ShoppingItemFor\" " +
                                            $"WHERE \"ShoppingItemId\" = {shoppingRequest.Id}", _connection);
                reader = command.ExecuteReader();
                while (reader.Read())
                { }
                reader.Close();

                foreach (var peopleId in shoppingRequest.ItemFor)
                {
                    command = new NpgsqlCommand("INSERT INTO public.\"ShoppingItemFor\" (\"ShoppingItemId\", \"PersonId\") " +
                                                $"VALUES ({shoppingRequest.Id}, {peopleId})", _connection);
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
                throw new Exception($"An Error occured while updating the payment (ID: {shoppingRequest.Id})", exception);
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
            catch (Exception exception)
            {
                _connection.Close();
                throw new Exception($"An Error occured while deleting the shopping item (ID: {shoppingItemId})", exception);
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

    public class UpdateShoppingItemRequestV2
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool? Purchased { get; set; }
        public List<int> ItemFor { get; set; }
    }
}