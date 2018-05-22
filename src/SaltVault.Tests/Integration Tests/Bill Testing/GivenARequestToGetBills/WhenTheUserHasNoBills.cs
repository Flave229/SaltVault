using System;
using System.Net.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using SaltVault.Tests.TestingHelpers;
using SaltVault.WebApp.Models.Bills;

namespace SaltVault.Tests.Integration_Tests.Bill_Testing.GivenARequestToGetBills
{
    [TestClass]
    public class WhenTheUserHasNoBills
    {
        private FakeTestingAccountHelper _fakeTestingAccountHelper;
        private Guid _validSessionId;
        private HttpClient _client;
        private GetBillListResponse _getBillListResponse;

        [TestInitialize]
        public void Initialize()
        {
            _fakeTestingAccountHelper = new FakeTestingAccountHelper();
            _validSessionId = _fakeTestingAccountHelper.GenerateValidFakeCredentials();
            _client = EndpointHelper.CreateFakeServer();
            EndpointHelper.SetAuthenticationToken(_validSessionId.ToString());

            var response = _client.GetAsync("/Api/v2/Bills").Result;
            string responseContent = response.Content.ReadAsStringAsync().Result;
            _getBillListResponse = JsonConvert.DeserializeObject<GetBillListResponse>(responseContent);
        }

        [TestMethod]
        public void ThenTheBillListIsEmpty()
        {
            Assert.AreEqual(_getBillListResponse.Bills.Count, 0);
        }

        [TestMethod]
        public void ThenTheResponseContainsNoErrors()
        {
            Assert.IsFalse(_getBillListResponse.HasError);
        }
    }
}
