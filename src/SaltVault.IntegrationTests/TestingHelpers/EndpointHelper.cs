using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Castle.Core.Internal;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Newtonsoft.Json;
using SaltVault.Core.Bills;
using SaltVault.Core.ToDo.Models;
using SaltVault.WebApp;
using SaltVault.WebApp.Models.Bills;
using SaltVault.WebApp.Models.Household;
using SaltVault.WebApp.Models.Shopping;
using SaltVault.WebApp.Models.Users;

namespace SaltVault.IntegrationTests.TestingHelpers
{
    public interface IEndpointHelperSetup
    {
        IEndpointHelperSetup SetAuthenticationToken(string sessionId);
        IEndpointHelperSetup AddTestBill(string name = null);
        IEndpointHelperSetup AddShoppingItem(string name = null);
        IEndpointHelperSetup AddToDoTask(string name = null);
        IEndpointHelperSetup AddHousehold(string name = null);
        IEndpointHelperSetup CreateHouseholdInviteLink();
        CreateHouseholdInviteLinkResponse ReturnHouseholdLink();
        List<int> ReturnAddedBillIds();
        void CleanUp(bool keepHousehold);
    }

    public class EndpointHelper
    {
        private HttpClient _fakeServer;
        private readonly IEndpointHelperSetup _endpointHelperSetup;

        public EndpointHelper()
        {
            CreateFakeServer();
            _endpointHelperSetup = new EndpointHelperSetup(_fakeServer);
        }

        private void CreateFakeServer()
        {
            TestServer server = new TestServer(new WebHostBuilder().UseStartup<Startup>());
            HttpClient httpClient = server.CreateClient();
            _fakeServer = httpClient;
        }

        public IEndpointHelperSetup Setup()
        {
            return _endpointHelperSetup;
        }

        public void CleanUp(bool keepHousehold = true)
        {
            _endpointHelperSetup.CleanUp(keepHousehold);
        }

        public string GetBills()
        {
            var response = _fakeServer.GetAsync("/Api/v2/Bills").Result;
            return response.Content.ReadAsStringAsync().Result;
        }

        public string GetBills(int id)
        {
            var response = _fakeServer.GetAsync($"/Api/v2/Bills?id={id}").Result;
            return response.Content.ReadAsStringAsync().Result;
        }

        public string GetShoppingItems()
        {
            var response = _fakeServer.GetAsync("/Api/v2/Shopping").Result;
            return response.Content.ReadAsStringAsync().Result;
        }

        public string GetToDoItems()
        {
            var response = _fakeServer.GetAsync("/Api/v2/ToDo").Result;
            return response.Content.ReadAsStringAsync().Result;
        }

        public string GetHousehold()
        {
            var response = _fakeServer.GetAsync("/Api/v2/Household").Result;
            return response.Content.ReadAsStringAsync().Result;
        }

        private class EndpointHelperSetup : IEndpointHelperSetup
        {
            private readonly HttpClient _fakeSever;
            private readonly List<int> _testBillsAdded = new List<int>();
            private CreateHouseholdInviteLinkResponse _inviteLinkResponse;

            public EndpointHelperSetup(HttpClient fakeSever)
            {
                _fakeSever = fakeSever;
            }

            public IEndpointHelperSetup SetAuthenticationToken(string sessionId)
            {
                _fakeSever.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Token", sessionId);
                return this;
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

            public IEndpointHelperSetup AddShoppingItem(string name = null)
            {
                AddShoppingItemRequest request = new AddShoppingItemRequest
                {
                    Name = name ?? "DEVELOPMENT TESTING SHOPPING ITEM",
                    ItemFor = new List<int> { 5 }
                };
                var requestBody = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
                var result = _fakeSever.PostAsync("/Api/v2/Shopping", requestBody).Result;

                var responseBody = result.Content.ReadAsStringAsync().Result;
                return this;
            }

            public IEndpointHelperSetup AddToDoTask(string name = null)
            {
                AddToDoTaskRequest request = new AddToDoTaskRequest
                {
                    Title = name ?? "DEVELOPMENT TESTING TO DO TASK",
                    PeopleIds = new List<int> { 5 }
                };
                var requestBody = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
                var result = _fakeSever.PostAsync("/Api/v2/ToDo", requestBody).Result;

                var responseBody = result.Content.ReadAsStringAsync().Result;
                return this;
            }

            public IEndpointHelperSetup AddHousehold(string name = null)
            {
                AddHouseholdRequest request = new AddHouseholdRequest
                {
                    Name = name ?? "DEVELOPMENT TESTING HOUSEHOLD"
                };
                var requestBody = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
                var result = _fakeSever.PostAsync("/Api/v2/Household", requestBody).Result;

                var responseBody = result.Content.ReadAsStringAsync().Result;
                return this;
            }

            public IEndpointHelperSetup CreateHouseholdInviteLink()
            {
                var requestBody = new StringContent("", Encoding.UTF8, "application/json");
                var result = _fakeSever.PostAsync("/Api/v2/Household/InviteLink", requestBody).Result;

                var responseBody = result.Content.ReadAsStringAsync().Result;
                _inviteLinkResponse = JsonConvert.DeserializeObject<CreateHouseholdInviteLinkResponse>(responseBody);
                return this;
            }

            public List<int> ReturnAddedBillIds()
            {
                return _testBillsAdded;
            }

            public CreateHouseholdInviteLinkResponse ReturnHouseholdLink()
            {
                if (_inviteLinkResponse == null || _inviteLinkResponse.HasError)
                    throw new Exception("You need to create an invite link before one can be returned. If you have set this correctly in the setup, there is an issue with the endpoint.");

                return _inviteLinkResponse;
            }

            public void CleanUp(bool keepHousehold)
            {
                DeleteHouseholdRequest request = new DeleteHouseholdRequest
                {
                    KeepHousehold = keepHousehold
                };
                HttpRequestMessage requestMessage = new HttpRequestMessage
                {
                    Content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json"),
                    Method = HttpMethod.Delete,
                    RequestUri = new Uri(_fakeSever.BaseAddress, "Api/v2/Household")
                };
                var result = _fakeSever.SendAsync(requestMessage).Result;
                var responseBody = result.Content.ReadAsStringAsync().Result;
            }
        }
    }
}