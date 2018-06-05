using System;
using System.IO;
using Npgsql;
using SaltVault.Core.Household.Model;

namespace SaltVault.Core.Household
{
    public interface IHouseholdRepository
    {
        House GetHouseholdForUser(int personId);
    }

    public class HouseholdRepository : IHouseholdRepository
    {
        private readonly string _connectionString;

        public HouseholdRepository()
        {
            _connectionString = File.ReadAllText("./Data/Config/LIVEConnectionString.config");
        }

        public House GetHouseholdForUser(int personId)
        {
            NpgsqlConnection connection = new NpgsqlConnection(_connectionString);
            connection.Open();

            var house = new House();
            try
            {
                var command = new NpgsqlCommand("SELECT House.\"Id\", House.\"Name\" " +
                                                "FROM public.\"PeopleForHouse\" AS PeopleForHouse " +
                                                "LEFT OUTER JOIN \"House\" AS House ON House.\"Id\" = PeopleForHouse.\"HouseId\" " +
                                                $"WHERE PeopleForHouse.\"PersonId\" = {personId} " +
                                                "LIMIT 1", connection);
                var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    house = new House
                    {
                        Id = Convert.ToInt32(reader[0]),
                        Name = Convert.ToString(reader[1])
                    };
                }

                reader.Close();
                return house;
            }
            catch (System.Exception exception)
            {
                throw new System.Exception("An Error occured while getting the household", exception);
            }
            finally
            {
                connection.Close();
            }
        }
    }
}