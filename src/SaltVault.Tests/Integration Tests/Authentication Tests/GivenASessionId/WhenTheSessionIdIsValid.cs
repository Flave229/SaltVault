﻿using System;
using System.Net.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using SaltVault.Tests.TestingHelpers;
using SaltVault.WebApp.Models.Bills;

namespace SaltVault.Tests.Integration_Tests.Authentication_Tests.GivenASessionId
{
    [TestClass]
    public class WhenTheSessionIdIsValid
    {
        private Guid _validSessionId;
        private FakeTestingAccountHelper _fakeTestingAccountHelper;

        [TestInitialize]
        public void Initialize()
        {
            _fakeTestingAccountHelper = new FakeTestingAccountHelper();
            _validSessionId = _fakeTestingAccountHelper.GenerateValidFakeCredentials();
            EndpointHelper.CreateFakeServer();
            EndpointHelper.SetAuthenticationToken(_validSessionId.ToString());
        }

        [TestMethod]
        public void ThenTheResponseContainsNoErrors()
        {
            string responseContent = EndpointHelper.GetBills();
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