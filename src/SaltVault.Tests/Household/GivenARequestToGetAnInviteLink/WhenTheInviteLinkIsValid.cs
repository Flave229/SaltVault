using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SaltVault.Core.Household;
using SaltVault.Core.Users;

namespace SaltVault.Tests.Household.GivenARequestToGetAnInviteLink
{
    [TestClass]
    public class WhenTheInviteLinkIsValid
    {
        [TestMethod]
        public void ThenTheHouseIdIsReturned()
        {
            InviteLinkService inviteLinkService = new InviteLinkService();
            ActiveUser user = new ActiveUser
            {
                HouseId = 3,
                PersonId = 5,
                Created = DateTime.Now
            };

            string inviteCode = inviteLinkService.GenerateInviteLinkForHousehold(user);
            int id = inviteLinkService.GetHouseholdForInviteLink(inviteCode);

            Assert.AreEqual(user.HouseId, id);
        }
    }
}
