using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Services.Models.FinanceModels;
using Services.Models;

namespace Services.FileIO
{
    public class BillFileHelper
    {
        private static readonly string FilePath = AppDomain.CurrentDomain.BaseDirectory + @"Data\Bills\bills.txt";

        public List<IPersistedData> Open()
        {
            try
            {
                if (!System.IO.File.Exists(FilePath)) return new List<IPersistedData>();

                var existingBillsAsJson = System.IO.File.ReadAllLines(FilePath);
                var existingBillAsString = "";

                for (var i = 0; i < existingBillsAsJson.Length; i++)
                {
                    existingBillAsString = existingBillAsString + existingBillsAsJson.ElementAt(i);
                }

                return existingBillAsString.Equals("") ? new List<IPersistedData>() : JsonConvert.DeserializeObject<List<Bill>>(existingBillAsString).Cast<IPersistedData>().ToList();
            }
            catch (Exception exception)
            {
                throw new Exception("Error: An Error occured while trying to retrieve bill data at: " + FilePath
                    + ".\n Exception: " + exception.Message, exception);
            }
        }

        public void Save(List<IPersistedData> bills)
        {
            try
            {
                var jsonResponse = JsonConvert.SerializeObject(bills);

                var directoryInfo = new System.IO.FileInfo(FilePath);
                directoryInfo.Directory?.Create();

                System.IO.File.WriteAllText(directoryInfo.FullName, jsonResponse);
            }
            catch (Exception exception)
            {
                throw new Exception("An Error occured while trying to save bill data at: " + FilePath
                    + ".\n Exception: " + exception.Message, exception);
            }
        }

        public List<IPersistedData> Add(List<IPersistedData> bills, IPersistedData billToAdd)
        {
            bills.Add(billToAdd);

            return bills;
        }

        public List<IPersistedData> Update(List<IPersistedData> bills, IPersistedData updatedBill)
        {
            var index = bills.FindIndex(bill => bill.Id.Equals(updatedBill.Id));
            bills[index] = updatedBill;

            return bills;
        }

        public void Delete(Guid billId)
        {
            try
            {
                var billList = GetAll();

                for (var i = 0; i < billList.Count; i++)
                {
                    if (billList[i].Id != billId) continue;

                    billList.RemoveAt(i);
                    break;
                }

                Save(billList);
            }
            catch (Exception exception)
            {
                throw new Exception("An Error occured while trying to delete the bill.\n Exception: " + exception.Message, exception);
            }
        }

        public void AddOrUpdate(IPersistedData bill)
        {
            var bills = Open();

            bills = bills.Any(existingBill => existingBill.Id.Equals(bill.Id)) ? Update(bills, bill) : Add(bills, bill);

            Save(bills);
        }

        public void AddOrUpdate(List<IPersistedData> bill)
        {
            for (var i = 0; i < bill.Count; i++)
            {
                AddOrUpdate(bill.ElementAt(i));
            }
        }

        public IPersistedData Get(Guid id)
        {
            var bills = Open();

            return bills.FirstOrDefault(bill => bill.Id.Equals(id));
        }

        public IPersistedData Get(string name)
        {
            var bills = Open();

            return bills.Cast<Bill>().FirstOrDefault(bill => bill.Name.Equals(name));
        }

        public List<IPersistedData> GetAll()
        {
            return Open();
        }
    }
}