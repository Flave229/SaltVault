using Newtonsoft.Json;
using System;
using Services.FileIO;
using Services.Models.ShoppingModels;

namespace HouseFinance.Api.Communication
{
    public static class Communication
    {
        public static string Call(ICommunicationRequest request)
        {
            if (!AuthenticatedUsers.CheckAuthentication(request.AuthToken))
                return "The authentication key provided is invalid.";

            switch (request.RequestType)
            {
                case "RequestBillList":
                    return RequestBillList();
                case "RequestBillDetails":
                    return RequestBillDetails(request.Id);
                case "AddBillItem":
                    return AddBill(request.PostBody);
                case "RequestShoppingList":
                    return RequestShoppingList();
                case "AddShoppingItem":
                    return AddShoppingItem(request.PostBody);
                default:
                    return "Request type was invalid.";
            }
        }

        private static string AddBill(string requestPostBody)
        {
            return "ping";
        }

        public static string RequestBillList()
        {
            try
            {
                var response = Builders.BillListBuilder.BuildBillList();
                return JsonConvert.SerializeObject(response);
            }
            catch
            {
                return "An Error occured while requesting bill details!";
            }
        }

        public static string RequestBillDetails(string billId)
        {
            try
            {
                var id = new Guid();

                if (!Guid.TryParse(billId, out id))
                    return "Bill PostBody was not valid, bill details could not be built!";

                var response = Builders.BillDetailsBuilder.BuildBillDetails(id);
                return JsonConvert.SerializeObject(response);
            }
            catch
            {
                return "An Error occured while requesting bill details!";
            }
        }

        public static string RequestShoppingList()
        {
            try
            {
                var response = Builders.ShoppingListBuilder.BuildShoppingList();
                return JsonConvert.SerializeObject(response);
            }
            catch
            {
                return "An Error occured while requesting shopping list details!";
            }
        }

        public static string AddShoppingItem(string postBody)
        {
            try
            {
                var item = JsonConvert.DeserializeObject<ShoppingItem>(postBody);
                new GenericFileHelper(FilePath.Shopping).AddOrUpdate<ShoppingItem>(item);

                return "Item Added";
            }
            catch
            {
                return "An Error occured while adding the shopping item!";
            }
        }
    }
}