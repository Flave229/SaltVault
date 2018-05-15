using System;
using System.Collections.Generic;

namespace SaltVault.Core.ToDo.Models
{
    public class UpdateToDoRequest
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public DateTime? Due { get; set; }
        public bool? Complete { get; set; }
        public List<int> PeopleIds { get; set; }

        public UpdateToDoRequest()
        {
            PeopleIds = new List<int>();
        }
    }
}