using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using SaltVault.Core.Exception;
using SaltVault.IntegrationTests.TestingHelpers;
using SaltVault.WebApp.Models.Bills;

namespace SaltVault.IntegrationTests.Authentication.GivenASessionId
{
    [TestClass]
    public class WhenTheSessionIdIsInvalid
    {
        private EndpointHelper _endpointHelper;

        [TestInitialize]
        public void Initialize()
        {
            Guid invalidSessionId = Guid.NewGuid();
            _endpointHelper = new EndpointHelper();
            _endpointHelper.Setup().SetAuthenticationToken(invalidSessionId.ToString());
        }

        [TestMethod]
        public void ThenTheResponseContainsAnErrorWithInvalidCredentialsCode()
        {
            string responseContent = _endpointHelper.GetBills();
            GetBillListResponse billListResponse = JsonConvert.DeserializeObject<GetBillListResponse>(responseContent);
            
            Assert.IsTrue(billListResponse.HasError);
            Assert.IsTrue(billListResponse.Error.ErrorCode == ErrorCode.SESSION_INVALID);
        }
    }
}
