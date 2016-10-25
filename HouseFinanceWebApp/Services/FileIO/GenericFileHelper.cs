using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Services.Models;

namespace Services.FileIO
{
    public class GenericFileHelper : IFileHelper
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

                return JsonConvert.DeserializeObject<List<T>>(existingFileAsString) as List<T>;
            }
            catch (Exception exception)
            {
                throw new Exception("Error: An Error occured while trying to retrieve data at: " + _filePath
                    + ".\n Exception: " + exception.Message, exception);
            }
        }

        public void Save<T>(List<IPersistedData> obj)
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

        public List<IPersistedData> Add<T>(List<IPersistedData> obj, IPersistedData objToAdd)
        {
            obj.Add(objToAdd);

            return obj;
        }

        public List<IPersistedData> Update<T>(List<IPersistedData> objs, IPersistedData updatedObj)
        {
            var index = objs.FindIndex(obj => obj.Id.Equals(updatedObj.Id));
            objs[index] = updatedObj;

            return objs;
        }

        public void Delete<T>(Guid Id)
        {
            try
            {
                var objList = GetAll<T>().Cast<IPersistedData>().ToList();

                for (var i = 0; i < objList.Count; i++)
                {
                    if ((objList[i] as IPersistedData).Id != Id) continue;

                    objList.RemoveAt(i);
                    break;
                }

                Save<T>(objList);
            }
            catch (Exception exception)
            {
                throw new Exception("An Error occured while trying to delete the object.\n Exception: " + exception.Message, exception);
            }
        }

        public void AddOrUpdate<T>(IPersistedData obj)
        {
            var objs = Open<T>().Cast<IPersistedData>().ToList();

            objs = objs.Any(existingObj => (existingObj as IPersistedData).Id.Equals(obj.Id)) ? Update<T>(objs as List<IPersistedData>, obj) : Add<T>(objs as List<IPersistedData>, obj);

            Save<T>(objs);
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

        public List<T> GetAll<T>()
        {
            return Open<T>().ToList();
        }
    }
}