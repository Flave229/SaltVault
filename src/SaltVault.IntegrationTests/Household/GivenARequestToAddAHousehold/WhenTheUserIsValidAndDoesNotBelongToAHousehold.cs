using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using SaltVault.IntegrationTests.TestingHelpers;
using SaltVault.WebApp.Models.Household;

namespace SaltVault.IntegrationTests.Household.GivenARequestToAddAHousehold
{
    [TestClass]
    public class WhenTheUserIsValidAndDoesNotBelongToAHousehold
    {
        private IAccountHelper _accountHelper;
        private Guid _validSessionId;
        private GetHouseholdResponse _getHouseholdResponse;
        private EndpointHelper _endpointHelper;

        [TestInitialize]
        public void Initialize()
        {
            _accountHelper = new RealAccountHelper();
            _validSessionId = _accountHelper.GenerateValidCredentials();
            _endpointHelper = new EndpointHelper();
            _endpointHelper.Setup()
                .SetAuthenticationToken(_validSessionId.ToString())
                .AddHousehold(typeof(WhenTheUserIsValidAndDoesNotBelongToAHousehold).Name);

            _getHouseholdResponse = _endpointHelper.GetHousehold();
        }

        [TestMethod]
        public void ThenTheUsersHouseholdIdIsTheAddedHouse()
        {
            Assert.AreEqual(this.GetType().Name, _getHouseholdResponse.House.Name);
        }

        [TestCleanup]
        public void CleanUp()
        {
            _endpointHelper.CleanUp(false);
            _accountHelper.CleanUp(_validSessionId);
        }
    }
}