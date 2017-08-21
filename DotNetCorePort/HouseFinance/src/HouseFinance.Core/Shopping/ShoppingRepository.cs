using System;
using System.Collections.Generic;
using System.IO;
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

                var command = new NpgsqlCommand("INSERT INTO public.\"ShoppingItem\" (\"Name\", \"Purchased\", \"AddedBy\") " +
                                                $"VALUES ('{shoppingItem.Name}', {shoppingItem.Purchased.ToString().ToUpper()}, {addedBy}) " +
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
    }
}
