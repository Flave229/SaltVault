using System.Collections.Generic;
using SaltVault.Core.ToDo.Models;

namespace SaltVault.WebApp.Models.ToDo
{
    public class GetToDoResponse : CommunicationResponse
    {
        public List<ToDoTask> ToDoTasks { get; set; }
    }
}