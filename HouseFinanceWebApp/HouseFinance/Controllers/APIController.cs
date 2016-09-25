using HouseFinance.Api.Communication;
using Newtonsoft.Json;
using System;
using System.Web.Mvc;

namespace HouseFinance.Controllers
{
    public class APIController : Controller
    {
        [Route("Api/{authToken}/{requestType}/{id?}")]
        public string ApiRequest(string authToken, string requestType, string id = "")
        {
            if (!AuthenticatedUsers.CheckAuthentication(authToken))
                return "The authentication key provided is invalid.";

            switch (requestType)
            {
                case "RequestBillList":
                    return RequestBillList();
                case "RequestBillDetails":
                    return RequestBillDetails(id);
                case "RequestShoppingList":
                    return RequestShoppingList();
                default:
                    return "Request type was invalid.";
            }
        }
        
        public string RequestBillList()
        {
            try
            {
                var response = Api.Builders.BillListBuilder.BuildBillList();
                return JsonConvert.SerializeObject(response);
            }
            catch
            {
                return "An Error occured while requesting bill details!";
            }
        }
        
        public string RequestBillDetails(string billId)
        {
            try
            {
                var id = new Guid();

                if (!Guid.TryParse(billId, out id))
                    return "Bill Id was not valid, bill details could not be built!";

                var response = Api.Builders.BillDetailsBuilder.BuildBillDetails(id);
                return JsonConvert.SerializeObject(response);
            }
            catch
            {
                return "An Error occured while requesting bill details!";
            }
        }
        
        public string RequestShoppingList()
        {
            try
            {
                var response = Api.Builders.ShoppingListBuilder.BuildShoppingList();
                return JsonConvert.SerializeObject(response);
            }
            catch
            {
                return "An Error occured while requesting shopping list details!";
            }
        }
    }
}