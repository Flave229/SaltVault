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
        private static FakeTestingAccountHelper _fakeTestingAccountHelper;
        private static Guid _validSessionId;
        private static GetBillListResponse _getBillListResponse;
        private static int _billId;
        private static EndpointHelper _endpointHelper;

        [TestInitialize]
        public void Initialize()
        {
            _fakeTestingAccountHelper = new FakeTestingAccountHelper();
            _validSessionId = _fakeTestingAccountHelper.GenerateValidFakeCredentials();
            _endpointHelper = new EndpointHelper();
            _billId = _endpointHelper.Setup()
                .SetAuthenticationToken(_validSessionId.ToString())
                .AddTestBill(typeof(WhenTheUserSuppliesAValidId).Name)
                .ReturnAddedBillIds()[0];
            string responseContent = _endpointHelper.GetBills(_billId);
            _getBillListResponse = JsonConvert.DeserializeObject<GetBillListResponse>(responseContent);
        }

        [TestMethod]
        public void ThenTheBillIdMatchesTheRequestedId()
        {
            Console.WriteLine(JsonConvert.SerializeObject(_getBillListResponse));
            Assert.AreEqual(_billId, _getBillListResponse.Bills[0].Id);
        }

        [TestMethod]
        public void ThenTheBillNameMatchesTheAddedBillName()
        {
            Console.WriteLine(JsonConvert.SerializeObject(_getBillListResponse));
            Assert.AreEqual(this.GetType().Name, _getBillListResponse.Bills[0].Name);
        }

        [TestMethod]
        public void ThenTheResponseContainsNoErrors()
        {
            Console.WriteLine(JsonConvert.SerializeObject(_getBillListResponse));
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
