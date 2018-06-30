using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SaltVault.IntegrationTests.TestingHelpers;
using SaltVault.WebApp.Models.Household;

namespace SaltVault.IntegrationTests.Household.GivenARequestToEditTheHouseName
{
    [TestClass]
    class WhenTheUserBelongsToAHouseAndTheRequestIsValid
    {
        private EndpointHelper _endpointHelper;
        private RealAccountHelper _accountHelper;
        private int _houseHoldId;

        [TestInitialize]
        public void Initialize()
        {
            _accountHelper = new RealAccountHelper();
            Guid validSessionId = _accountHelper.GenerateValidCredentials();
            _endpointHelper = new EndpointHelper();
            _endpointHelper.Setup()
                .SetAuthenticationToken(validSessionId.ToString())
                .AddHousehold(typeof(WhenTheUserBelongsToAHouseAndTheRequestIsValid).Name);

            _houseHoldId = _endpointHelper.GetHousehold().House.Id;
        }

        [TestMethod]
        public void ThenTheHouseholdNameIsUpdated()
        {
            _endpointHelper.EditHousehold(new UpdateHouseholdRequest
            {
                Id = _houseHoldId,
                Name = "Changed Name: " + typeof(WhenTheUserBelongsToAHouseAndTheRequestIsValid).Name
            });
            string newHouseholdName = _endpointHelper.GetHousehold().House.Name;
            Assert.IsFalse(newHouseholdName == "Changed Name: " + typeof(WhenTheUserBelongsToAHouseAndTheRequestIsValid).Name);
        }

        [TestCleanup]
        public void CleanUp()
        {
            _endpointHelper.CleanUp();
            _accountHelper.CleanUp();
        }
    }
}
