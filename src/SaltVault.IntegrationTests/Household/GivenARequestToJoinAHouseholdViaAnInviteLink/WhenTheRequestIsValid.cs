using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SaltVault.IntegrationTests.TestingHelpers;
using SaltVault.WebApp.Models.Household;
using SaltVault.WebApp.Models.Users;

namespace SaltVault.IntegrationTests.Household.GivenARequestToJoinAHouseholdViaAnInviteLink
{
    [TestClass]
    public class WhenTheRequestIsValid
    {
        private IAccountHelper _accountHelper;
        private Guid _firstSessionId;
        private Guid _secondUserSession;
        private EndpointHelper _endpointHelper;
        private JoinHouseholdResponse _joinHouseholdResponse;

        [TestInitialize]
        public void Initialize()
        {
            _accountHelper = new RealAccountHelper();
            _firstSessionId = _accountHelper.GenerateValidCredentials();
            _endpointHelper = new EndpointHelper();
            CreateHouseholdInviteLinkResponse inviteLinkResponse = _endpointHelper.Setup()
                .SetAuthenticationToken(_firstSessionId.ToString())
                .AddHousehold(typeof(WhenTheRequestIsValid).Name)
                .CreateHouseholdInviteLink()
                .ReturnHouseholdLink();

            _secondUserSession = _accountHelper.GenerateValidCredentials();
            _endpointHelper.Setup().SetAuthenticationToken(_secondUserSession.ToString());

            _joinHouseholdResponse = _endpointHelper.JoinHousehold(inviteLinkResponse.InviteLink);
        }

        [TestMethod]
        public void ThenTheResponseContainsNoErrors()
        {
            Assert.IsFalse(_joinHouseholdResponse == null);
            Assert.IsFalse(_joinHouseholdResponse.HasError);
        }

        [TestMethod]
        public void ThenTheUserIsAddedToTheHousehold()
        {
            int firstUserHousehold = _endpointHelper.GetHousehold().House.Id;
            Assert.AreEqual(firstUserHousehold, _joinHouseholdResponse.Id);
        }

        [TestCleanup]
        public void CleanUp()
        {
            _endpointHelper.CleanUp(false);
            _accountHelper.CleanUp();
        }
    }
}
