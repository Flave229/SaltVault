using System;
using System.Net.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using SaltVault.Tests.TestingHelpers;
using SaltVault.WebApp.Models.Bills;

namespace SaltVault.Tests.Authentication_Tests.GivenASessionId
{
    [TestClass]
    public class WhenTheSessionIdIsValid
    {
        private Guid _validSessionId;
        private HttpClient _client;

        [TestInitialize]
        public void Initialize()
        {
            FakeTestingAccountHelper accountHelper = new FakeTestingAccountHelper();
            _validSessionId = accountHelper.GenerateValidFakeCredentials();
            _client = EndpointHelper.CreateFakeServer(_validSessionId.ToString());
        }

        [TestMethod]
        public void ThenTheResponseContainsNoErrors()
        {
            var response = _client.GetAsync("/Api/v2/Bills").Result;
            string responseContent = response.Content.ReadAsStringAsync().Result;
            GetBillListResponse billListResponse = JsonConvert.DeserializeObject<GetBillListResponse>(responseContent);

            Assert.IsFalse(billListResponse.HasError);
        }
    }
}