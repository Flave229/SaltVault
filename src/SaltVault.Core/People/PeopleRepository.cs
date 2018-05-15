using System;
using System.Collections.Generic;
using System.IO;
using Npgsql;

namespace SaltVault.Core.People
{
    public interface IPeopleRepository
    {
        List<Person> GetAllPeople();
        List<Person> GetPeople(List<int> peopleIds);
        Person GetPersonFromDiscordId(string discordId);
    }

    public class PeopleRepository : IPeopleRepository
    {
        private readonly NpgsqlConnection _connection;

        public PeopleRepository(NpgsqlConnection connection)
        {
            _connection = connection;
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
            catch (Exception exception)
            {
                throw new Exception("An Error occured while getting the list of people", exception);
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
            catch (Exception exception)
            {
                throw new Exception("An Error occured while getting the list of people", exception);
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
            catch (Exception exception)
            {
                throw new Exception("An Error occured while getting the person from the discord Id", exception);
            }
            finally
            {
                _connection.Close();
            }
        }
    }
}
