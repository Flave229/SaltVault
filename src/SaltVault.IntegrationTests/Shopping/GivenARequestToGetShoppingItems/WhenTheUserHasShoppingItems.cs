using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using SaltVault.IntegrationTests.TestingHelpers;
using SaltVault.WebApp.Models.Shopping;

namespace SaltVault.IntegrationTests.Shopping.GivenARequestToGetShoppingItems
{
    [TestClass]
    public class WhenTheUserHasShoppingItems
    {
        private FakeAccountHelper _fakeAccountHelper;
        private Guid _validSessionId;
        private GetShoppingResponse _getShoppingItemResponse;
        private EndpointHelper _endpointHelper;

        [TestInitialize]
        public void Initialize()
        {
            _fakeAccountHelper = new FakeAccountHelper();
            Guid validSessionId = _fakeAccountHelper.GenerateValidCredentials();
            _endpointHelper = new EndpointHelper();
            _endpointHelper.Setup()
                .SetAuthenticationToken(validSessionId.ToString())
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
            _fakeAccountHelper.CleanUp();
        }
    }
}