﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using SaltVault.Tests.TestingHelpers;
using SaltVault.WebApp.Models.Shopping;

namespace SaltVault.Tests.Integration_Tests.Shopping_Testing.GivenARequestToGetShoppingItems
{
    [TestClass]
    public class WhenTheUserHasShoppingItems
    {
        private FakeTestingAccountHelper _fakeTestingAccountHelper;
        private Guid _validSessionId;
        private GetShoppingResponse _getShoppingItemResponse;
        private EndpointHelper _endpointHelper;

        [TestInitialize]
        public void Initialize()
        {
            _fakeTestingAccountHelper = new FakeTestingAccountHelper();
            _validSessionId = _fakeTestingAccountHelper.GenerateValidFakeCredentials();
            _endpointHelper = new EndpointHelper();
            _endpointHelper.Setup()
                .SetAuthenticationToken(_validSessionId.ToString())
                .AddShoppingItem(typeof(WhenTheUserHasShoppingItems).Name);
            
            string responseContent = _endpointHelper.GetShoppingItems();
            _getShoppingItemResponse = JsonConvert.DeserializeObject<GetShoppingResponse>(responseContent);
        }

        [TestMethod]
        public void ThenTheShoppingListContainsItems()
        {
            Assert.AreEqual(1, _getShoppingItemResponse.ShoppingList.Count);
        }

        [TestMethod]
        public void ThenTheResponseContainsNoErrors()
        {
            Assert.IsFalse(_getShoppingItemResponse.HasError);
        }

        [TestCleanup]
        public void CleanUp()
        {
            _endpointHelper.CleanUp();
            _fakeTestingAccountHelper.CleanUp(_validSessionId);
        }
    }
}