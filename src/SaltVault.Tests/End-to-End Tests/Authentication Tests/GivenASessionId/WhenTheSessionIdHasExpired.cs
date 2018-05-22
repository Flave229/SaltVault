using System;
using System.Net.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using SaltVault.Core.Exception;
using SaltVault.Tests.TestingHelpers;
using SaltVault.WebApp.Models.Bills;

namespace SaltVault.Tests.Authentication_Tests.GivenASessionId
{
    [TestClass]
    public class WhenTheSessionIdHasExpired
    {
        private Guid _expiredSessionId;
        private HttpClient _client;
        private FakeTestingAccountHelper _fakeTestingAccountHelper;

        [TestInitialize]
        public void Initialize()
        {
            _fakeTestingAccountHelper = new FakeTestingAccountHelper();
            _expiredSessionId = _fakeTestingAccountHelper.GenerateValidExpiredFakeCredentials();
            _client = EndpointHelper.CreateFakeServer();
            EndpointHelper.SetAuthenticationToken(_expiredSessionId.ToString());
        }

        [TestMethod]
        public void ThenTheResponseContainsAnErrorWithExpiredCode()
        {
            var response = _client.GetAsync("/Api/v2/Bills").Result;
            string responseContent = response.Content.ReadAsStringAsync().Result;
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