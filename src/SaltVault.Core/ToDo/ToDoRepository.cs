using System.Collections.Generic;
using Npgsql;
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
            return new List<ToDoTask>();
        }
    }
}