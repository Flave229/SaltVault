﻿using HouseFinance.Api.Communication;
using Newtonsoft.Json;
using System;
using System.Web.Mvc;

namespace HouseFinance.Controllers
{
    public class APIController : Controller
    {
        public string RequestBillList()
        {
            return RequestBillList("");
        }

        public string RequestShoppingList()
        {
            return RequestShoppingList("");
        }
        
        [Route("Api/{authToken}/RequestBillList")]
        public string RequestBillList(string authToken)
        {
            if (AuthenticatedUsers.CheckAuthentication(authToken))
            {
                var response = Api.Builders.BillListBuilder.BuildBillList();

                var jsonResponse = JsonConvert.SerializeObject(response);

                return jsonResponse;
            }

            return "An Error occured while requesting bill details!";
        }

        [Route("Api/{authToken}/RequestBillDetails/{billId}")]
        public string RequestBillDetails(string authToken, string billId)
        {
            if (AuthenticatedUsers.CheckAuthentication(authToken))
            {
                var id = new Guid();

                if (!Guid.TryParse(billId, out id))
                    return "Bill Id was not valid, bill details could not be built!";

                var response = Api.Builders.BillDetailsBuilder.BuildBillDetails(id);

                var jsonResponse = JsonConvert.SerializeObject(response);

                return jsonResponse;
            }

            return "An Error occured while requesting bill details!";
        }

        [Route("Api/RequestShoppingList/{authToken}")]
        public string RequestShoppingList(string authToken)
        {
            if (AuthenticatedUsers.CheckAuthentication(authToken))
            {
                var response = Api.Builders.ShoppingListBuilder.BuildShoppingList();

                var jsonResponse = JsonConvert.SerializeObject(response);

                return jsonResponse;
            }

            return "An Error occured while requesting shopping list details!";
        }
    }
}