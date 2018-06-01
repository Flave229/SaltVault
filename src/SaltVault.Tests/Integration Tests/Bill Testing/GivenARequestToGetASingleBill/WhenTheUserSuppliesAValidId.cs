using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using SaltVault.Tests.TestingHelpers;
using SaltVault.WebApp.Models.Bills;

namespace SaltVault.Tests.Integration_Tests.Bill_Testing.GivenARequestToGetASingleBill
{
    [TestClass]
    public class WhenTheUserSuppliesAValidId
    {
        private FakeTestingAccountHelper _fakeTestingAccountHelper;
        private Guid _validSessionId;
        private GetBillListResponse _getBillListResponse;
        private int _billId;

        [TestInitialize]
        public void Initialize()
        {
            _fakeTestingAccountHelper = new FakeTestingAccountHelper();
            _validSessionId = _fakeTestingAccountHelper.GenerateValidFakeCredentials();
            EndpointHelper.CreateFakeServer();
            EndpointHelper.SetAuthenticationToken(_validSessionId.ToString());

            _billId = EndpointHelper.AddTestBill(this.GetType().Name);
            string responseContent = EndpointHelper.GetBills(_billId);
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
            EndpointHelper.CleanUp();
            _fakeTestingAccountHelper.CleanUp(_validSessionId);
        }
    }
}
