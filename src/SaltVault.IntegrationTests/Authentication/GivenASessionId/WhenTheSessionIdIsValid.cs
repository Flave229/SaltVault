﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using SaltVault.IntegrationTests.TestingHelpers;
using SaltVault.WebApp.Models.Bills;

namespace SaltVault.IntegrationTests.Authentication.GivenASessionId
{
    [TestClass]
    public class WhenTheSessionIdIsValid
    {
        private Guid _validSessionId;
        private FakeTestingAccountHelper _fakeTestingAccountHelper;
        private EndpointHelper _endpointHelper;

        [TestInitialize]
        public void Initialize()
        {
            _fakeTestingAccountHelper = new FakeTestingAccountHelper();
            _validSessionId = _fakeTestingAccountHelper.GenerateValidFakeCredentials();
            _endpointHelper = new EndpointHelper();
            _endpointHelper.Setup()
                .SetAuthenticationToken(_validSessionId.ToString());
        }

        [TestMethod]
        public void ThenTheResponseContainsNoErrors()
        {
            string responseContent = _endpointHelper.GetBills();
            GetBillListResponse billListResponse = JsonConvert.DeserializeObject<GetBillListResponse>(responseContent);
            
            Assert.IsFalse(billListResponse.HasError);
        }

        [TestCleanup]
        public void CleanUp()
        {
            _fakeTestingAccountHelper.CleanUp(_validSessionId);
        }
    }
}