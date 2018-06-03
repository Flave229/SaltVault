using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using SaltVault.IntegrationTests.TestingHelpers;
using SaltVault.WebApp.Models.Bills;

namespace SaltVault.IntegrationTests.Bill.GivenARequestToGetBills
{
    [TestClass]
    public class WhenTheUserHasBills
    {
        private FakeTestingAccountHelper _fakeTestingAccountHelper;
        private Guid _validSessionId;
        private GetBillListResponse _getBillListResponse;
        private EndpointHelper _endpointHelper;

        [TestInitialize]
        public void Initialize()
        {
            _fakeTestingAccountHelper = new FakeTestingAccountHelper();
            _validSessionId = _fakeTestingAccountHelper.GenerateValidFakeCredentials();
            _endpointHelper = new EndpointHelper();
            _endpointHelper.Setup()
                .SetAuthenticationToken(_validSessionId.ToString())
                .AddTestBill();
            
            string responseContent = _endpointHelper.GetBills();
            _getBillListResponse = JsonConvert.DeserializeObject<GetBillListResponse>(responseContent);
        }

        [TestMethod]
        public void ThenTheBillListContainsBills()
        {
            Assert.AreEqual(1, _getBillListResponse.Bills.Count);
        }

        [TestMethod]
        public void ThenTheResponseContainsNoErrors()
        {
            Assert.IsFalse(_getBillListResponse.HasError);
        }

        [TestCleanup]
        public void CleanUp()
        {
            _endpointHelper.CleanUp();
            _fakeTestingAccountHelper.CleanUp(_validSessionId);
        }
    }
}
