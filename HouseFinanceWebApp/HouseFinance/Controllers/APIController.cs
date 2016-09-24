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

        [Route("Api/RequestBillList/{authToken}")]
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
    }
}