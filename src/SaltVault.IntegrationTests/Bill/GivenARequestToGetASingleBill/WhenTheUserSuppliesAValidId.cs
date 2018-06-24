using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using SaltVault.IntegrationTests.TestingHelpers;
using SaltVault.WebApp.Models.Bills;

namespace SaltVault.IntegrationTests.Bill.GivenARequestToGetASingleBill
{
    [TestClass]
    public class WhenTheUserSuppliesAValidId
    {
        private static FakeAccountHelper _fakeAccountHelper;
        private static GetBillListResponse _getBillListResponse;
        private static int _billId;
        private static EndpointHelper _endpointHelper;

        [TestInitialize]
        public void Initialize()
        {
            _fakeAccountHelper = new FakeAccountHelper();
            Guid validSessionId = _fakeAccountHelper.GenerateValidCredentials();
            _endpointHelper = new EndpointHelper();
            _billId = _endpointHelper.Setup()
                .SetAuthenticationToken(validSessionId.ToString())
                .AddBill(typeof(WhenTheUserSuppliesAValidId).Name)
                .ReturnAddedBillIds()[0];
            string responseContent = _endpointHelper.GetBills(_billId);
            _getBillListResponse = JsonConvert.DeserializeObject<GetBillListResponse>(responseContent);
        }

        [TestMethod]
        public void ThenTheBillIdMatchesTheRequestedId()
        {
            Assert.AreEqual(_billId, _getBillListResponse.Bills[0].Id);
        }

        [TestMethod]
        public void ThenTheBillNameMatchesTheAddedBillName()
        {
            Assert.AreEqual(this.GetType().Name, _getBillListResponse.Bills[0].Name);
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
