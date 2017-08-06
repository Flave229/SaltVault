using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace HouseFinance.Core.FileManagement
{
    public class GenericFileHelper
    {
        private readonly string _filePath;

        public GenericFileHelper(string filePath)
        {
            _filePath = filePath;
        }

        public List<T> Open<T>()
        {
            try
            {
                if (!System.IO.File.Exists(_filePath)) return new List<T>();

                var existingFileAsJson = System.IO.File.ReadAllLines(_filePath);
                var existingFileAsString = "";

                for (var i = 0; i < existingFileAsJson.Length; i++)
                {
                    existingFileAsString = existingFileAsString + existingFileAsJson.ElementAt(i);
                }

                if (existingFileAsString.Equals(""))
                {
                    return new List<T>();
                }

                return JsonConvert.DeserializeObject<List<T>>(existingFileAsString);
            }
            catch (Exception exception)
            {
                var message = "Error: An Error occured while trying to retrieve data at: " + _filePath + ".\n Exception: " + exception.Message;
                throw new Exception(message, exception);
            }
        }

        public void Save(List<IPersistedData> obj)
        {
            try
            {
                var jsonResponse = JsonConvert.SerializeObject(obj);

                var directoryInfo = new System.IO.FileInfo(_filePath);
                directoryInfo.Directory?.Create();

                System.IO.File.WriteAllText(directoryInfo.FullName, jsonResponse);
            }
            catch (Exception exception)
            {
                throw new Exception("An Error occured while trying to save data at: " + _filePath
                    + ".\n Exception: " + exception.Message, exception);
            }
        }

        public List<IPersistedData> Add(List<IPersistedData> obj, IPersistedData objToAdd)
        {
            obj.Add(objToAdd);

            return obj;
        }

        public List<IPersistedData> Update(List<IPersistedData> objs, IPersistedData updatedObj)
        {
            var index = objs.FindIndex(obj => obj.Id.Equals(updatedObj.Id));
            objs[index] = updatedObj;

            return objs;
        }

        public void Delete<T>(Guid id)
        {
            try
            {
                var objList = GetAll<T>().Cast<IPersistedData>().ToList();

                for (var i = 0; i < objList.Count; i++)
                {
                    if (objList[i].Id != id) continue;

                    objList.RemoveAt(i);
                    break;
                }

                Save(objList);
            }
            catch (Exception exception)
            {
                throw new Exception("An Error occured while trying to delete the object.\n Exception: " + exception.Message, exception);
            }
        }

        public void AddOrUpdate<T>(IPersistedData obj)
        {
            var objs = Open<T>().Cast<IPersistedData>().ToList();

            if (obj.Id == Guid.Empty)
            {
                obj.Id = Guid.NewGuid();
                objs = Add(objs, obj);
            }
            else
                objs = objs.Any(existingObj => existingObj.Id.Equals(obj.Id))
                    ? Update(objs, obj)
                    : Add(objs, obj);

            Save(objs);
        }

        public void AddOrUpdate<T>(List<IPersistedData> obj)
        {
            for (var i = 0; i < obj.Count; i++)
            {
                AddOrUpdate<T>(obj.ElementAt(i));
            }
        }

        public T Get<T>(Guid id)
        {
            var objs = Open<T>().Cast<IPersistedData>().ToList();

            return (T)objs.FirstOrDefault(obj => obj.Id.Equals(id));
        }

        public List<T> Get<T>(List<Guid> ids)
        {
            var objs = Open<T>().Cast<IPersistedData>().ToList();

            return objs.Where(obj => ids.Contains(obj.Id)).Cast<T>().ToList();
        }

        public List<T> GetAll<T>()
        {
            return Open<T>().ToList();
        }
    }
}
