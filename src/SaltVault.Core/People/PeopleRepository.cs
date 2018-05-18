using System;
using System.Collections.Generic;
using System.IO;
using Npgsql;
using SaltVault.Core.Services.Google;
using SaltVault.Core.Users;

namespace SaltVault.Core.People
{
    public interface IPeopleRepository
    {
        List<Person> GetAllPeople();
        List<Person> GetPeople(List<int> peopleIds);
        Person GetPersonFromDiscordId(string discordId);
        ActiveUser GetPersonFromGoogleClientId(string googleClientId);
        ActiveUser AddPersonUsingGoogleInformation(GoogleTokenInformation tokenInformation);
    }

    public class PeopleRepository : IPeopleRepository
    {
        private readonly NpgsqlConnection _connection;

        public PeopleRepository()
        {
            var connectionString = File.ReadAllText("./Data/Config/LIVEConnectionString.config");
            _connection = new NpgsqlConnection(connectionString);
        }

        public List<Person> GetAllPeople()
        {
            _connection.Open();

            try
            {
                var command = new NpgsqlCommand(
                    "SELECT Person.\"Id\", Person.\"FirstName\", Person.\"LastName\", Person.\"Image\" " +
                    "FROM public.\"Person\" AS Person " +
                    "WHERE Person.\"Active\" = true", _connection);
                var reader = command.ExecuteReader();

                List<Person> people = new List<Person>();
                while (reader.Read())
                {
                    people.Add(new Person
                    {
                        Id = Convert.ToInt32(reader[0]),
                        FirstName = (string)reader[1],
                        LastName = (string)reader[2],
                        Image = (string)reader[3]
                    });
                }

                reader.Close();
                return people;
            }
            catch (System.Exception exception)
            {
                throw new System.Exception("An Error occured while getting the list of people", exception);
            }
            finally
            {
                _connection.Close();
            }
        }

        public List<Person> GetPeople(List<int> peopleIds)
        {
            _connection.Open();

            try
            {
                var command = new NpgsqlCommand(
                    "SELECT Person.\"Id\", Person.\"FirstName\", Person.\"LastName\", Person.\"Image\" " +
                    "FROM public.\"Person\" AS Person " +
                    $"WHERE Person.\"Id\" IN ({string.Join(", ", peopleIds)})", _connection);
                var reader = command.ExecuteReader();

                List<Person> people = new List<Person>();
                while (reader.Read())
                {
                    people.Add(new Person
                    {
                        Id = Convert.ToInt32(reader[0]),
                        FirstName = (string)reader[1],
                        LastName = (string)reader[2],
                        Image = (string)reader[3]
                    });
                }

                reader.Close();
                return people;
            }
            catch (System.Exception exception)
            {
                throw new System.Exception("An Error occured while getting the list of people", exception);
            }
            finally
            {
                _connection.Close();
            }
        }

        public Person GetPersonFromDiscordId(string discordId)
        {
            _connection.Open();

            try
            {
                var command = new NpgsqlCommand(
                    "SELECT Person.\"Id\", Person.\"FirstName\", Person.\"LastName\", Person.\"Image\" " +
                    "FROM public.\"Person\" AS Person " +
                    $"WHERE Person.\"DiscordUserId\" = {discordId}", _connection);
                var reader = command.ExecuteReader();

                Person person = null;
                while (reader.Read())
                {
                    person = new Person
                    {
                        Id = Convert.ToInt32(reader[0]),
                        FirstName = (string)reader[1],
                        LastName = (string)reader[2],
                        Image = (string)reader[3]
                    };
                }

                reader.Close();
                return person;
            }
            catch (System.Exception exception)
            {
                throw new System.Exception("An Error occured while getting the person from the discord Id", exception);
            }
            finally
            {
                _connection.Close();
            }
        }

        public ActiveUser GetPersonFromGoogleClientId(string googleClientId)
        {
            _connection.Open();

            try
            {
                var command = new NpgsqlCommand(
                    "SELECT Person.\"Id\", PeopleForHouse.\"HouseId\" " +
                    "FROM public.\"Person\" AS Person " +
                    "LEFT OUTER JOIN \"PeopleForHouse\" As PeopleForHouse ON PeopleForHouse.\"PersonId\" = Person.\"Id\" " +
                    $"WHERE Person.\"GoogleClientId\" = '{googleClientId}'", _connection);
                var reader = command.ExecuteReader();

                ActiveUser person = null;
                while (reader.Read())
                {
                    person = new ActiveUser
                    {
                        PersonId = Convert.ToInt32(reader[0]),
                        HouseId = (reader[1] == DBNull.Value) ? 0 : Convert.ToInt32(reader[1])
                    };
                }

                reader.Close();
                return person;
            }
            catch (System.Exception exception)
            {
                throw new System.Exception("An Error occured while getting the person from the google client Id", exception);
            }
            finally
            {
                _connection.Close();
            }
        }

        public ActiveUser AddPersonUsingGoogleInformation(GoogleTokenInformation tokenInformation)
        {
            _connection.Open();

            try
            {
                var command = new NpgsqlCommand($"INSERT INTO public.\"Person\" (\"FirstName\", \"LastName\", \"Image\", \"Active\", \"GoogleClientId\") " +
                                                $"VALUES ('{tokenInformation.given_name}', '{tokenInformation.family_name}', '{tokenInformation.picture}', true, '{tokenInformation.sub}') " +
                                                "RETURNING \"Id\"", _connection);
                Int64 userId = -1;
                var reader = command.ExecuteReader();
                while (reader.Read())
                    userId = Convert.ToInt64(reader[0]);

                reader.Close();

                return new ActiveUser
                {
                    PersonId = Convert.ToInt32(userId)
                };
            }
            catch (System.Exception exception)
            {
                throw new System.Exception($"An Error occured while adding the user after first sign in", exception);
            }
            finally
            {
                _connection.Close();
            }
        }
    }
}
