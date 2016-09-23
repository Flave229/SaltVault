using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Services.Models.GlobalModels;

namespace Services.FileIO
{
    public static class PersonFileHelper
    {
        private static readonly string FilePath = AppDomain.CurrentDomain.BaseDirectory + @"Data\People\People.txt";

        private static List<Person> Open()
        {
            try
            {
                if (!System.IO.File.Exists(FilePath)) return new List<Person>();

                var existingPeopleAsJson = System.IO.File.ReadAllLines(FilePath);
                var existingPersonAsString = "";

                for (var i = 0; i < existingPeopleAsJson.Length; i++)
                {
                    existingPersonAsString = existingPersonAsString + existingPeopleAsJson.ElementAt(i);
                }

                return JsonConvert.DeserializeObject<List<Person>>(existingPersonAsString);
            }
            catch (Exception exception)
            {
                throw new Exception("Error: An Error occured while trying to retrieve Person data at: " + FilePath
                    + ".\n Exception: " + exception.Message, exception);
            }
        }

        private static void Save(List<Person> people)
        {
            try
            {
                var jsonResponse = JsonConvert.SerializeObject(people);

                var directoryInfo = new System.IO.FileInfo(FilePath);
                directoryInfo.Directory?.Create();

                System.IO.File.WriteAllText(directoryInfo.FullName, jsonResponse);
            }
            catch (Exception exception)
            {
                throw new Exception("Error: An Error occured while trying to save Person data at: " + FilePath
                    + ".\n Exception: " + exception.Message, exception);
            }
        }

        public static List<Person> Add(List<Person> people, Person personToAdd)
        {
            people.Add(personToAdd);

            return people;
        }

        public static List<Person> Update(List<Person> people, Person updatedPerson)
        {
            var index = people.FindIndex(person => person.Id.Equals(updatedPerson.Id));
            people[index] = updatedPerson;

            return people;
        }

        public static void AddOrUpdate(Person person)
        {
            var People = Open();

            People = People.Any(existingPerson => existingPerson.Id.Equals(person.Id)) ? Update(People, person) : Add(People, person);

            Save(People);
        }

        public static void AddOrUpdate(List<Person> person)
        {
            for (var i = 0; i < person.Count; i++)
            {
                AddOrUpdate(person.ElementAt(i));
            }
        }

        public static Person GetPerson(Guid personId)
        {
            var people = Open();

            return people.FirstOrDefault(person => person.Id.Equals(personId));
        }

        public static Person GetPerson(string name)
        {
            var people = Open();

            return people.FirstOrDefault(person => person.FirstName.Equals(name));
        }

        public static List<Person> GetPeople()
        {
            return Open();
        }
    }
}
