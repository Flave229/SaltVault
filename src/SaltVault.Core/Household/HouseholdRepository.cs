using System;
using System.IO;
using Npgsql;
using SaltVault.Core.Household.Model;

namespace SaltVault.Core.Household
{
    public interface IHouseholdRepository
    {
        House GetHouseholdForUser(int personId);
        int AddHousehold(string name, int personId);
        void AddPersonToHousehold(int houseId, int personId);
        void DeleteHousehold(int houseId);
        bool UpdateHousehold(UpdateHousehold updateHousehold);
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

        public int AddHousehold(string name, int personId)
        {
            NpgsqlConnection connection = new NpgsqlConnection(_connectionString);
            connection.Open();

            var house = new House();
            try
            {
                var command = new NpgsqlCommand("INSERT INTO public.\"House\" (\"Name\") " +
                                                $"VALUES ('{name}') " +
                                                "RETURNING \"Id\"", connection);
                Int64 houseId = -1;
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    houseId = (Int64)reader[0];
                }
                reader.Close();

                command = new NpgsqlCommand("INSERT INTO public.\"PeopleForHouse\" (\"HouseId\", \"PersonId\", \"LeadTenant\") " +
                                            $"VALUES ({houseId}, {personId}, true)", connection);
                reader = command.ExecuteReader();
                while (reader.Read())
                { }
                reader.Close();

                return Convert.ToInt32(houseId);
            }
            catch (System.Exception exception)
            {
                throw new System.Exception("An Error occured while adding the new household", exception);
            }
            finally
            {
                connection.Close();
            }
        }

        public void AddPersonToHousehold(int houseId, int personId)
        {
            NpgsqlConnection connection = new NpgsqlConnection(_connectionString);
            connection.Open();

            var house = new House();
            try
            {
                var command = new NpgsqlCommand("DELETE FROM public.\"PeopleForHouse\" AS PeopleForHouse " +
                                                $"WHERE PeopleForHouse.\"PersonId\" = {personId}", connection);
                var reader = command.ExecuteReader();
                while (reader.Read())
                { }
                reader.Close();

                command = new NpgsqlCommand("INSERT INTO public.\"PeopleForHouse\" (\"HouseId\", \"PersonId\", \"LeadTenant\") " +
                                            $"VALUES ({houseId}, {personId}, false)", connection);
                reader = command.ExecuteReader();
                while (reader.Read())
                { }
                reader.Close();
            }
            catch (System.Exception exception)
            {
                throw new System.Exception("An Error occured while adding the new household", exception);
            }
            finally
            {
                connection.Close();
            }
        }

        public void DeleteHousehold(int houseId)
        {
            NpgsqlConnection connection = new NpgsqlConnection(_connectionString);
            connection.Open();

            try
            {
                var command = new NpgsqlCommand("DELETE FROM public.\"PeopleForHouse\" AS PeopleForHouse " +
                                                $"WHERE PeopleForHouse.\"HouseId\" = {houseId}", connection);
                var reader = command.ExecuteReader();
                while (reader.Read())
                { }
                reader.Close();
                
                reader = command.ExecuteReader();
                while (reader.Read())
                { }
                reader.Close();

                command = new NpgsqlCommand("DELETE FROM public.\"House\" AS House " +
                                            $"WHERE House.\"Id\" = {houseId} ", connection);

                reader = command.ExecuteReader();
                while (reader.Read())
                {
                }
                reader.Close();
            }
            catch (System.Exception exception)
            {
                throw new System.Exception($"An Error occured while deleting the household (ID: {houseId})", exception);
            }
            finally
            {
                connection.Close();
            }
        }

        public bool UpdateHousehold(UpdateHousehold request)
        {
            NpgsqlConnection connection = new NpgsqlConnection(_connectionString);
            connection.Open();

            try
            {
                var rowUpdated = false;

                NpgsqlCommand command = new NpgsqlCommand("UPDATE public.\"Bill\" " +
                                            $"SET \"Name\"='{request.Name}' " +
                                            $"WHERE \"Id\" = {request.Id} " +
                                            "RETURNING \"Id\"", connection);
                NpgsqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    rowUpdated = true;
                }
                reader.Close();

                return rowUpdated;
            }
            catch (System.Exception exception)
            {
                throw new System.Exception($"An Error occured while updating the household '{request.Name}'", exception);
            }
            finally
            {
                connection.Close();
            }
        }
    }
}