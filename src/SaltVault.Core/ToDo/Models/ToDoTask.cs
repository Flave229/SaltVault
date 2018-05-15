using System;
using System.Collections.Generic;
using SaltVault.Core.People;

namespace SaltVault.Core.ToDo.Models
{
    public class ToDoTask
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public DateTime? Due { get; set; }
        public bool Complete { get; set; }
        public List<Person> PeopleForTask { get; set; }
    }
}
