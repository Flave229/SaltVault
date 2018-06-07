using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SaltVault.IntegrationTests.TestingHelpers;
using SaltVault.WebApp.Models.Household;

namespace SaltVault.IntegrationTests.Household.GivenARequestToCreateAnInviteLink
{
    [TestClass]
    public class WhenTheRequestIsValid
    {
        private FakeAccountHelper _fakeAccountHelper;
        private Guid _validSessionId;
        private EndpointHelper _endpointHelper;
        private CreateHouseholdInviteLinkResponse _inviteLink;

        [TestInitialize]
        public void Initialize()
        {
            _fakeAccountHelper = new FakeAccountHelper();
            _validSessionId = _fakeAccountHelper.GenerateValidCredentials();
            _endpointHelper = new EndpointHelper();
            _inviteLink = _endpointHelper.Setup()
                .SetAuthenticationToken(_validSessionId.ToString())
                .CreateHouseholdInviteLink()
                .ReturnHouseholdLink();
        }

        [TestMethod]
        public void ThenAResponseContainingNoErrorsIsReturned()
        {
            Assert.IsFalse(_inviteLink == null);
            Assert.IsFalse(_inviteLink.HasError);
        }

        [TestCleanup]
        public void CleanUp()
        {
            _endpointHelper.CleanUp();
            _fakeAccountHelper.CleanUp(_validSessionId);
        }
    }
}
