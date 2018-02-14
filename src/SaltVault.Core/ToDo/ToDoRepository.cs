using System.Collections.Generic;
using SaltVault.Core.ToDo.Models;

namespace SaltVault.Core.ToDo
{
    public interface IToDoRepository
    {
        List<ToDoTask> GetToDoList();
    }

    public class ToDoRepository : IToDoRepository
    {
        public List<ToDoTask> GetToDoList()
        {
            return new List<ToDoTask>();
        }
    }
}