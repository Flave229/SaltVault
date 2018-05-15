using System;
using System.Collections.Generic;
using Npgsql;
using System.Linq;
using SaltVault.Core.People;
using SaltVault.Core.ToDo.Models;

namespace SaltVault.Core.ToDo
{
    public interface IToDoRepository
    {
        List<ToDoTask> GetToDoList();
    }

    public class ToDoRepository : IToDoRepository
    {
        private readonly NpgsqlConnection _connection;

        public ToDoRepository(NpgsqlConnection connection)
        {
            _connection = connection;
        }

        public List<ToDoTask> GetToDoList()
        {
            _connection.Open();

            var toDoTasks = new List<ToDoTask>();
            try
            {
                var command = new NpgsqlCommand("SELECT ToDo.\"Id\", ToDo.\"Title\", ToDo.\"Due\", ToDo.\"Complete\", Person.\"Id\", Person.\"Image\", Person.\"FirstName\", Person.\"LastName\" " +
                                                "FROM public.\"ToDo\" AS ToDo " +
                                                "LEFT OUTER JOIN \"PeopleForToDo\" AS PeopleForToDo ON PeopleForToDo.\"ToDoId\" = ToDo.\"Id\" " +
                                                "LEFT OUTER JOIN \"Person\" AS Person ON Person.\"Id\" = PeopleForToDo.\"PersonId\" " +
                                                "ORDER BY ToDo.\"Due\" DESC, Person.\"Id\" ASC", _connection);
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
            catch (Exception exception)
            {
                throw new Exception("An Error occured while getting the to do items", exception);
            }
            finally
            {
                _connection.Close();
            }
        }
    }
}