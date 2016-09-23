using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Services.Models.ShoppingModels;

namespace Services.FileIO
{
    public class ShoppingFileHelper
    {
        private static readonly string FilePath = AppDomain.CurrentDomain.BaseDirectory + @"Data\Shopping\ShoppingItems.txt";

        private static List<ShoppingItem> Open()
        {
            try
            {
                if (!System.IO.File.Exists(FilePath)) return new List<ShoppingItem>();

                var existingShoppingItemsAsJson = System.IO.File.ReadAllLines(FilePath);
                var existingShoppingItemAsString = "";

                for (var i = 0; i < existingShoppingItemsAsJson.Length; i++)
                {
                    existingShoppingItemAsString = existingShoppingItemAsString + existingShoppingItemsAsJson.ElementAt(i);
                }

                return existingShoppingItemAsString.Equals("") ? new List<ShoppingItem>() : JsonConvert.DeserializeObject<List<ShoppingItem>>(existingShoppingItemAsString);
            }
            catch (Exception exception)
            {
                throw new Exception("Error: An Error occured while trying to retrieve ShoppingItem data at: " + FilePath
                    + ".\n Exception: " + exception.Message, exception);
            }
        }

        private static void Save(List<ShoppingItem> shoppingItems)
        {
            try
            {
                var jsonResponse = JsonConvert.SerializeObject(shoppingItems);

                var directoryInfo = new System.IO.FileInfo(FilePath);
                directoryInfo.Directory?.Create();

                System.IO.File.WriteAllText(directoryInfo.FullName, jsonResponse);
            }
            catch (Exception exception)
            {
                throw new Exception("An Error occured while trying to save ShoppingItem data at: " + FilePath
                    + ".\n Exception: " + exception.Message, exception);
            }
        }

        public static List<ShoppingItem> Add(List<ShoppingItem> shoppingItems, ShoppingItem shoppingItemToAdd)
        {
            shoppingItems.Add(shoppingItemToAdd);

            return shoppingItems;
        }

        public static List<ShoppingItem> Update(List<ShoppingItem> shoppingItems, ShoppingItem updatedShoppingItem)
        {
            var index = shoppingItems.FindIndex(ShoppingItem => ShoppingItem.Id.Equals(updatedShoppingItem.Id));
            shoppingItems[index] = updatedShoppingItem;

            return shoppingItems;
        }

        public static void Delete(Guid shoppingItemId)
        {
            try
            {
                var shoppingItemList = GetShoppingItems();

                for (var i = 0; i < shoppingItemList.Count; i++)
                {
                    if (shoppingItemList[i].Id != shoppingItemId) continue;

                    shoppingItemList.RemoveAt(i);
                    break;
                }

                Save(shoppingItemList);
            }
            catch (Exception exception)
            {
                throw new Exception("An Error occured while trying to delete the ShoppingItem.\n Exception: " + exception.Message, exception);
            }
        }

        public static void AddOrUpdate(ShoppingItem shoppingItem)
        {
            var shoppingItems = Open();

            shoppingItems = shoppingItems.Any(existingShoppingItem => existingShoppingItem.Id.Equals(shoppingItem.Id)) ? Update(shoppingItems, shoppingItem) : Add(shoppingItems, shoppingItem);

            Save(shoppingItems);
        }

        public static void AddOrUpdate(List<ShoppingItem> shoppingItem)
        {
            for (var i = 0; i < shoppingItem.Count; i++)
            {
                AddOrUpdate(shoppingItem.ElementAt(i));
            }
        }

        public static ShoppingItem GetShoppingItem(Guid shoppingItemId)
        {
            var shoppingItems = Open();

            return shoppingItems.FirstOrDefault(shoppingItem => shoppingItem.Id.Equals(shoppingItemId));
        }

        public static ShoppingItem GetShoppingItem(string name)
        {
            var shoppingItems = Open();

            return shoppingItems.FirstOrDefault(shoppingItem => shoppingItem.Name.Equals(name));
        }

        public static List<ShoppingItem> GetShoppingItems()
        {
            return Open();
        }
    }
}
