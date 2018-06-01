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
    public interface IEndpointHelperSetup
    {
        IEndpointHelperSetup AddTestBill(string name = null);
        IEndpointHelperSetup SetAuthenticationToken(string sessionId);
        List<int> ReturnAddedBillIds();
        void CleanUp();
    }

    public class EndpointHelper
    {
        private HttpClient _fakeSever;
        private readonly IEndpointHelperSetup _endpointHelperSetup;

        public EndpointHelper()
        {
            CreateFakeServer();
            _endpointHelperSetup = new EndpointHelperSetup(_fakeSever);
        }

        private void CreateFakeServer()
        {
            TestServer server = new TestServer(new WebHostBuilder().UseStartup<Startup>());
            HttpClient httpClient = server.CreateClient();
            _fakeSever = httpClient;
        }

        public IEndpointHelperSetup Setup()
        {
            return _endpointHelperSetup;
        }

        public void CleanUp()
        {
            _endpointHelperSetup.CleanUp();
        }

        public string GetBills()
        {
            var response = _fakeSever.GetAsync("/Api/v2/Bills").Result;
            return response.Content.ReadAsStringAsync().Result;
        }

        public string GetBills(int id)
        {
            var response = _fakeSever.GetAsync($"/Api/v2/Bills?id={id}").Result;
            return response.Content.ReadAsStringAsync().Result;
        }

        private class EndpointHelperSetup : IEndpointHelperSetup
        {
            private readonly HttpClient _fakeSever;
            private readonly List<int> _testBillsAdded = new List<int>();

            public EndpointHelperSetup(HttpClient fakeSever)
            {
                _fakeSever = fakeSever;
            }

            public IEndpointHelperSetup AddTestBill(string name = null)
            {
                AddBillRequest request = new AddBillRequest
                {
                    Name = name ?? "DEVELOPMENT TESTING BILL",
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
                return this;
            }

            public IEndpointHelperSetup SetAuthenticationToken(string sessionId)
            {
                _fakeSever.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Token", sessionId);
                return this;
            }

            public List<int> ReturnAddedBillIds()
            {
                return _testBillsAdded;
            }

            public void CleanUp()
            {
                // TODO: Use the future endpoints that can clear all the data associated with a user & household
                foreach (var billId in _testBillsAdded)
                {
                    DeleteTestBill(billId);
                }
            }

            private void DeleteTestBill(int billId)
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
}