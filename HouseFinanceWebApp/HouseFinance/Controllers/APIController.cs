using Newtonsoft.Json;
using System;
using System.Web.Mvc;

namespace HouseFinance.Controllers
{
    public class APIController : Controller
    {
        [Route("Api/RequestBillList")]
        public string RequestBillList()
        {
            var response = Api.Builders.BillListBuilder.BuildBillList();
            
            var jsonResponse = JsonConvert.SerializeObject(response);

            return jsonResponse;
        }
    }
}