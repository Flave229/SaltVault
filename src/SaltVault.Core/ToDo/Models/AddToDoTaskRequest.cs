using System;
using System.Collections.Generic;

namespace SaltVault.Core.ToDo.Models
{
    public class AddToDoTaskRequest
    {
        public string Title { get; set; }
        public DateTime? Due { get; set; }
        public List<int> PeopleIds { get; set; }

        public AddToDoTaskRequest()
        {
            PeopleIds = new List<int>();
        }
    }
}
