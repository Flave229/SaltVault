using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Services.Models.FinanceModels;

namespace Services.FileIO
{
    public class GenericFileHelper : IFileHelper
    {
        private string _filePath;

        public GenericFileHelper(FilePath filePath)
        {
            _filePath = FilePathToString.ToString(filePath);
        }

        public List<IPersistedData> Open()
        {
            try
            {
                if (!System.IO.File.Exists(_filePath)) return new List<IPersistedData>();

                var existingFileAsJson = System.IO.File.ReadAllLines(_filePath);
                var existingFileAsString = "";

                for (var i = 0; i < existingFileAsJson.Length; i++)
                {
                    existingFileAsString = existingFileAsString + existingFileAsJson.ElementAt(i);
                }

                return existingFileAsString.Equals("") ? new List<IPersistedData>() : JsonConvert.DeserializeObject<dynamic>(existingFileAsString).Cast<IPersistedData>().ToList();
            }
            catch (Exception exception)
            {
                throw new Exception("Error: An Error occured while trying to retrieve data at: " + _filePath
                    + ".\n Exception: " + exception.Message, exception);
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

        public void Delete(Guid Id)
        {
            try
            {
                var objList = GetAll();

                for (var i = 0; i < objList.Count; i++)
                {
                    if (objList[i].Id != Id) continue;

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

        public void AddOrUpdate(IPersistedData obj)
        {
            var objs = Open();

            objs = objs.Any(existingObj => existingObj.Id.Equals(obj.Id)) ? Update(objs, obj) : Add(objs, obj);

            Save(objs);
        }

        public void AddOrUpdate(List<IPersistedData> obj)
        {
            for (var i = 0; i < obj.Count; i++)
            {
                AddOrUpdate(obj.ElementAt(i));
            }
        }

        public IPersistedData Get(Guid id)
        {
            var objs = Open();

            return objs.FirstOrDefault(obj => obj.Id.Equals(id));
        }

        public List<IPersistedData> GetAll()
        {
            return Open();
        }
    }
}