using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using SaltVault.IntegrationTests.TestingHelpers;
using SaltVault.WebApp.Models.Bills;

namespace SaltVault.IntegrationTests.Bill.GivenARequestToGetBills
{
    [TestClass]
    public class WhenTheUserHasOverdueBillsOlderThanCompletedBills
    {
        private FakeAccountHelper _fakeAccountHelper;
        private GetBillListResponse _getBillListResponse;
        private EndpointHelper _endpointHelper;

        [TestInitialize]
        public void Initialize()
        {
            _fakeAccountHelper = new FakeAccountHelper();
            Guid validSessionId = _fakeAccountHelper.GenerateValidCredentials();
            _endpointHelper = new EndpointHelper();
            _endpointHelper.Setup()
                .SetAuthenticationToken(validSessionId.ToString())
                .AddOverdueBill()
                .AddCompletedBill();

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
            _fakeAccountHelper.CleanUp();
        }
    }
}
