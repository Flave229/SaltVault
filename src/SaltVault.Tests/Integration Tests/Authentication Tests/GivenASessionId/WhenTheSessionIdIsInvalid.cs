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
    public class WhenTheSessionIdIsInvalid
    {
        private Guid _invalidSessionId;
        private HttpClient _client;

        [TestInitialize]
        public void Initialize()
        {
            _invalidSessionId = Guid.NewGuid();
            _client = EndpointHelper.CreateFakeServer();
            EndpointHelper.SetAuthenticationToken(_invalidSessionId.ToString());
        }

        [TestMethod]
        public void ThenTheResponseContainsAnErrorWithInvalidCredentialsCode()
        {
            var response = _client.GetAsync("/Api/v2/Bills").Result;
            string responseContent = response.Content.ReadAsStringAsync().Result;
            GetBillListResponse billListResponse = JsonConvert.DeserializeObject<GetBillListResponse>(responseContent);
            
            Assert.IsTrue(billListResponse.HasError);
            Assert.IsTrue(billListResponse.Error.ErrorCode == ErrorCode.SESSION_INVALID);
        }
    }
}
