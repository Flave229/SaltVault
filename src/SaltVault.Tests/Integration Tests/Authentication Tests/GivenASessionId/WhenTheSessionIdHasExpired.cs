using System;
using System.Net.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using SaltVault.Core.Exception;
using SaltVault.Tests.TestingHelpers;
using SaltVault.WebApp.Models.Bills;

namespace SaltVault.Tests.Integration_Tests.Authentication_Tests.GivenASessionId
{
    [TestClass]
    public class WhenTheSessionIdHasExpired
    {
        private Guid _expiredSessionId;
        private FakeTestingAccountHelper _fakeTestingAccountHelper;

        [TestInitialize]
        public void Initialize()
        {
            _fakeTestingAccountHelper = new FakeTestingAccountHelper();
            _expiredSessionId = _fakeTestingAccountHelper.GenerateValidExpiredFakeCredentials();
            EndpointHelper.CreateFakeServer();
            EndpointHelper.SetAuthenticationToken(_expiredSessionId.ToString());
        }

        [TestMethod]
        public void ThenTheResponseContainsAnErrorWithExpiredCode()
        {
            string responseContent = EndpointHelper.GetBills();
            GetBillListResponse billListResponse = JsonConvert.DeserializeObject<GetBillListResponse>(responseContent);

            Assert.IsTrue(billListResponse.HasError);
            Assert.IsTrue(billListResponse.Error.ErrorCode == ErrorCode.USER_SESSION_EXPIRED);
        }

        [TestCleanup]
        public void CleanUp()
        {
            _fakeTestingAccountHelper.CleanUp(_expiredSessionId);
        }
    }
}