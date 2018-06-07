using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SaltVault.IntegrationTests.TestingHelpers;
using SaltVault.WebApp.Models.Household;

namespace SaltVault.IntegrationTests.Household.GivenARequestToGetAHousehold
{
    [TestClass]
    public class WhenTheUserIsValidAndBelongsToAHousehold
    {
        private FakeAccountHelper _fakeAccountHelper;
        private Guid _validSessionId;
        private GetHouseholdResponse _getHouseholdResponse;
        private EndpointHelper _endpointHelper;

        [TestInitialize]
        public void Initialize()
        {
            _fakeAccountHelper = new FakeAccountHelper();
            _validSessionId = _fakeAccountHelper.GenerateValidCredentials();
            _endpointHelper = new EndpointHelper();
            _endpointHelper.Setup()
                .SetAuthenticationToken(_validSessionId.ToString());

            _getHouseholdResponse = _endpointHelper.GetHousehold();
        }

        [TestMethod]
        public void ThenTheHouseholdIdIsTheUsersHouse()
        {
            Assert.AreEqual(3, _getHouseholdResponse.House.Id);
        }

        [TestCleanup]
        public void CleanUp()
        {
            _endpointHelper.CleanUp();
            _fakeAccountHelper.CleanUp();
        }
    }
}
