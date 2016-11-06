using Newtonsoft.Json;
using System;

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
                case "RequestShoppingList":
                    return RequestShoppingList();
                default:
                    return "Request type was invalid.";
            }
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
                    return "Bill Id was not valid, bill details could not be built!";

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
    }
}
