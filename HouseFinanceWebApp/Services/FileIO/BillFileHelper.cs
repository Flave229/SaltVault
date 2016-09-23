using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Services.Models.FinanceModels;

namespace Services.FileIO
{
    public static class BillFileHelper
    {
        private static readonly string FilePath = AppDomain.CurrentDomain.BaseDirectory + @"Data\Bills\bills.txt";

        private static List<Bill> Open()
        {
            try
            {
                if (!System.IO.File.Exists(FilePath)) return new List<Bill>();

                var existingBillsAsJson = System.IO.File.ReadAllLines(FilePath);
                var existingBillAsString = "";

                for (var i = 0; i < existingBillsAsJson.Length; i++)
                {
                    existingBillAsString = existingBillAsString + existingBillsAsJson.ElementAt(i);
                }

                return existingBillAsString.Equals("") ? new List<Bill>() : JsonConvert.DeserializeObject<List<Bill>>(existingBillAsString);
            }
            catch (Exception exception)
            {
                throw new Exception("Error: An Error occured while trying to retrieve bill data at: " + FilePath
                    + ".\n Exception: " + exception.Message, exception);
            }
        }

        private static void Save(List<Bill> bills)
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

        public static List<Bill> Add(List<Bill> bills, Bill billToAdd)
        {
            bills.Add(billToAdd);

            return bills;
        }

        public static List<Bill> Update(List<Bill> bills, Bill updatedBill)
        {
            var index = bills.FindIndex(bill => bill.Id.Equals(updatedBill.Id));
            bills[index] = updatedBill;

            return bills;
        }

        public static void Delete(Guid billId)
        {
            try
            {
                var billList = GetBills();

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

        public static void AddOrUpdate(Bill bill)
        {
            var bills = Open();

            bills = bills.Any(existingBill => existingBill.Id.Equals(bill.Id)) ? Update(bills, bill) : Add(bills, bill);

            Save(bills);
        }

        public static void AddOrUpdate(List<Bill> bill)
        {
            for (var i = 0; i < bill.Count; i++)
            {
                AddOrUpdate(bill.ElementAt(i));
            }
        }

        public static Bill GetBill(Guid billId)
        {
            var bills = Open();

            return bills.FirstOrDefault(bill => bill.Id.Equals(billId));
        }

        public static Bill GetBill(string name)
        {
            var bills = Open();

            return bills.FirstOrDefault(bill => bill.Name.Equals(name));
        }

        public static List<Bill> GetBills()
        {
            return Open();
        }
    }
}