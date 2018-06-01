using System;
using System.Collections.Generic;
using System.IO;
using Npgsql;
using System.Linq;
using SaltVault.Core.People;
using SaltVault.Core.ToDo.Models;

namespace SaltVault.Core.ToDo
{
    public interface IToDoRepository
    {
        List<ToDoTask> GetToDoList(int userHouseId);
        int AddToDoTask(AddToDoTaskRequest toDoTask, int userHouseId);
        void UpdateToDoTask(UpdateToDoRequest toDoRequest);
        bool DeleteToDoTask(int toDoId);
    }

    public class ToDoRepository : IToDoRepository
    {
        private readonly NpgsqlConnection _connection;

        public ToDoRepository()
        {
            var connectionString = File.ReadAllText("./Data/Config/LIVEConnectionString.config");
            _connection = new NpgsqlConnection(connectionString);
        }

        public List<ToDoTask> GetToDoList(int userHouseId)
        {
            _connection.Open();

            var toDoTasks = new List<ToDoTask>();
            try
            {
                var command = new NpgsqlCommand("SELECT ToDo.\"Id\", ToDo.\"Title\", ToDo.\"Due\", ToDo.\"Complete\", Person.\"Id\", Person.\"Image\", Person.\"FirstName\", Person.\"LastName\" " +
                                                "FROM public.\"ToDo\" AS ToDo " +
                                                "LEFT OUTER JOIN \"PeopleForToDo\" AS PeopleForToDo ON PeopleForToDo.\"ToDoId\" = ToDo.\"Id\" " +
                                                "LEFT OUTER JOIN \"Person\" AS Person ON Person.\"Id\" = PeopleForToDo.\"PersonId\" " +
                                                $"WHERE ToDo.\"HouseId\" = {userHouseId} " +
                                                "ORDER BY ToDo.\"Complete\", ToDo.\"Due\" DESC, Person.\"Id\" ASC", _connection);
                var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    var toDoId = Convert.ToInt32(reader[0]);
                    ToDoTask toDoTask;

                    if (toDoTasks.Any(x => x.Id == toDoId))
                        toDoTask = toDoTasks.First(x => x.Id == toDoId);
                    else
                    {
                        toDoTask = new ToDoTask
                        {
                            Id = toDoId,
                            Title = Convert.ToString(reader[1]),
                            Due = (reader[2] == DBNull.Value) ? null : (DateTime?)Convert.ToDateTime(reader[2]),
                            Complete = Convert.ToBoolean(reader[3])
                        };
                    }

                    var personId = Convert.ToInt32(reader[4]);
                    var personImage = Convert.ToString(reader[5]);
                    if (toDoTask.PeopleForTask.Any(x => x.Id == personId) == false)
                    {
                        toDoTask.PeopleForTask.Add(new Person
                        {
                            Id = personId,
                            Image = personImage,
                            FirstName = Convert.ToString(reader[6]),
                            LastName = Convert.ToString(reader[7])
                        });
                    }
                    
                    if (toDoTasks.Any(x => x.Id == toDoId) == false)
                        toDoTasks.Add(toDoTask);
                }

                reader.Close();
                return toDoTasks;
            }
            catch (System.Exception exception)
            {
                throw new System.Exception("An Error occured while getting the to do items", exception);
            }
            finally
            {
                _connection.Close();
            }
        }

        public int AddToDoTask(AddToDoTaskRequest toDoTask, int userHouseId)
        {
            _connection.Open();

            try
            {
                var dueValue = toDoTask.Due == null ? "NULL" : $"'{toDoTask.Due:yyyy-MM-dd}'";
                var command = new NpgsqlCommand($"INSERT INTO public.\"ToDo\" (\"Title\", \"Due\", \"Complete\", \"HouseId\") " +
                                                $"VALUES ('{toDoTask.Title}', {dueValue}, false, {userHouseId}) " +
                                                "RETURNING \"Id\"", _connection);
                Int64 toDoTaskId = -1;
                var reader = command.ExecuteReader();
                while (reader.Read())
                    toDoTaskId = Convert.ToInt64(reader[0]);

                reader.Close();

                foreach (var peopleId in toDoTask.PeopleIds)
                {
                    command = new NpgsqlCommand("INSERT INTO public.\"PeopleForToDo\" (\"ToDoId\", \"PersonId\") " +
                                                $"VALUES ({toDoTaskId}, {peopleId})", _connection);
                    reader = command.ExecuteReader();
                    while (reader.Read())
                    { }
                    reader.Close();
                }

                return Convert.ToInt32(toDoTaskId);
            }
            catch (System.Exception exception)
            {
                throw new System.Exception($"An Error occured while adding the To Do Task '{toDoTask.Title}'", exception);
            }
            finally
            {
                _connection.Close();
            }
        }

        public void UpdateToDoTask(UpdateToDoRequest toDoRequest)
        {
            _connection.Open();

            try
            {
                var setValues = new List<string>();

                if (toDoRequest.Title != null)
                    setValues.Add($"\"Title\"='{toDoRequest.Title}'");
                if (toDoRequest.Due != null)
                    setValues.Add($"\"Due\"='{toDoRequest.Due}'");
                if (toDoRequest.Complete != null)
                    setValues.Add($"\"Complete\"={toDoRequest.Complete}");

                NpgsqlCommand command;
                NpgsqlDataReader reader;

                if (setValues.Count > 0)
                {
                    command = new NpgsqlCommand("UPDATE public.\"ToDo\" " +
                                                $"SET {string.Join(", ", setValues)} " +
                                                $"WHERE \"Id\" = {toDoRequest.Id}", _connection);
                    reader = command.ExecuteReader();
                    while (reader.Read())
                    { }
                    reader.Close();
                }

                if (toDoRequest.PeopleIds == null || toDoRequest.PeopleIds.Count == 0)
                    return;

                command = new NpgsqlCommand("DELETE FROM public.\"PeopleForToDo\" " +
                                            $"WHERE \"ToDoId\" = {toDoRequest.Id}", _connection);
                reader = command.ExecuteReader();
                while (reader.Read())
                {}
                reader.Close();

                foreach (var personId in toDoRequest.PeopleIds)
                {
                    command = new NpgsqlCommand("INSERT INTO public.\"PeopleForToDo\" (\"ToDoId\", \"PersonId\") " +
                                                $"VALUES ({toDoRequest.Id}, {personId})", _connection);
                    reader = command.ExecuteReader();
                    while (reader.Read())
                    {}
                    reader.Close();
                }
            }
            catch (System.Exception exception)
            {
                throw new System.Exception($"An Error occured while updating the To Do Task '{toDoRequest.Title}'", exception);
            }
            finally
            {
                _connection.Close();
            }
        }

        public bool DeleteToDoTask(int toDoId)
        {
            _connection.Open();

            try
            {
                var command = new NpgsqlCommand("DELETE FROM public.\"PeopleForToDo\" " +
                                            $"WHERE \"ToDoId\" = {toDoId}", _connection);
                var reader = command.ExecuteReader();
                while (reader.Read())
                { }
                reader.Close();

                command = new NpgsqlCommand("DELETE FROM public.\"ToDo\" " +
                                            $"WHERE \"Id\" = {toDoId} " +
                                            "RETURNING \"Id\"", _connection);

                var deleted = false;
                reader = command.ExecuteReader();
                while (reader.Read())
                {
                    deleted = true;
                }
                reader.Close();

                return deleted;
            }
            catch (System.Exception exception)
            {
                throw new System.Exception($"An Error occured while deleting the To Do Task (ID: {toDoId})", exception);
            }
            finally
            {
                _connection.Close();
            }
        }
    }
}