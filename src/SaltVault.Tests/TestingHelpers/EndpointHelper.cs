using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Newtonsoft.Json;
using SaltVault.Core.Bills;
using SaltVault.WebApp;
using SaltVault.WebApp.Models.Bills;

namespace SaltVault.Tests.TestingHelpers
{
    public class EndpointHelper
    {
        private static HttpClient _fakeSever;
        private static readonly List<int> _testBillsAdded = new List<int>();

        public static HttpClient CreateFakeServer()
        {
            if (_fakeSever != null)
                return _fakeSever;

            TestServer server = new TestServer(new WebHostBuilder().UseStartup<Startup>());
            HttpClient httpClient = server.CreateClient();
            _fakeSever = httpClient;
            return _fakeSever;
        }

        public static void SetAuthenticationToken(string sessionId)
        {
            _fakeSever.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Token", sessionId);
        }

        public static void CleanUp()
        {
            // TODO: Use the future endpoints that can clear all the data associated with a user & household
            foreach (var billId in _testBillsAdded)
            {
                DeleteTestBill(billId);
            }
        }

        public static string GetBills()
        {
            var response = _fakeSever.GetAsync("/Api/v2/Bills").Result;
            return response.Content.ReadAsStringAsync().Result;
        }

        public static void AddTestBill()
        {
            AddBillRequest request = new AddBillRequest
            {
                Name = "DEVELOPMENT TESTING BILL",
                Due = DateTime.Now,
                PeopleIds = new List<int> { 5 },
                TotalAmount = 299,
                RecurringType = RecurringType.None
            };
            var requestBody = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
            var result = _fakeSever.PostAsync("/Api/v2/Bills", requestBody).Result;

            var responseBody = result.Content.ReadAsStringAsync().Result;
            AddBillResponse billResponse = JsonConvert.DeserializeObject<AddBillResponse>(responseBody);
            _testBillsAdded.Add(billResponse.Id);
        }

        private static void DeleteTestBill(int billId)
        {
            DeleteBillRequest request = new DeleteBillRequest { BillId = billId };
            HttpRequestMessage deleteMessage = new HttpRequestMessage
            {
                Content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json"),
                Method = HttpMethod.Delete,
                RequestUri = new Uri(_fakeSever.BaseAddress, "Api/v2/Bills")
            };
            var result = _fakeSever.SendAsync(deleteMessage).Result;
            var responseBody = result.Content.ReadAsStringAsync().Result;
        }
    }
}