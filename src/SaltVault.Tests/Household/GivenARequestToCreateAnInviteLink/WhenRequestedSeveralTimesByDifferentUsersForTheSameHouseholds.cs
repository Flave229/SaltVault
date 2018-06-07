using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SaltVault.Core.Household;
using SaltVault.Core.Users;

namespace SaltVault.Tests.Household.GivenARequestToCreateAnInviteLink
{
    [TestClass]
    public class WhenRequestedSeveralTimesByDifferentUsersForTheSameHouseholds
    {
        [TestMethod]
        public void ThenTheInviteLinksAreIdentical()
        {
            InviteLinkService inviteLinkService = new InviteLinkService();
            ActiveUser user1 = new ActiveUser
            {
                HouseId = 3,
                PersonId = 6,
                Created = DateTime.Now
            };
            ActiveUser user2 = new ActiveUser
            {
                HouseId = 3,
                PersonId = 7,
                Created = DateTime.Now
            };

            string inviteCode1 = inviteLinkService.GenerateInviteLinkForHousehold(user1);
            string inviteCode2 = inviteLinkService.GenerateInviteLinkForHousehold(user2);

            Assert.IsTrue(inviteCode1 == inviteCode2);
        }
    }
}