using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SaltVault.Core.Household;
using SaltVault.Core.Users;

namespace SaltVault.Tests.Household.GivenARequestToCreateAnInviteLink
{
    [TestClass]
    public class WhenRequestedSeveralTimesByDifferentUsersForDifferentHouseholds
    {
        [TestMethod]
        public void ThenEachInviteCodeIsUnique()
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
                HouseId = 4,
                PersonId = 7,
                Created = DateTime.Now
            };
            ActiveUser user3 = new ActiveUser
            {
                HouseId = 5,
                PersonId = 8,
                Created = DateTime.Now
            };
            ActiveUser user4 = new ActiveUser
            {
                HouseId = 6,
                PersonId = 9,
                Created = DateTime.Now
            };
            ActiveUser user5 = new ActiveUser
            {
                HouseId = 7,
                PersonId = 10,
                Created = DateTime.Now
            };

            string inviteCode1 = inviteLinkService.GenerateInviteLinkForHousehold(user1);
            string inviteCode2 = inviteLinkService.GenerateInviteLinkForHousehold(user2);
            string inviteCode3 = inviteLinkService.GenerateInviteLinkForHousehold(user3);
            string inviteCode4 = inviteLinkService.GenerateInviteLinkForHousehold(user4);
            string inviteCode5 = inviteLinkService.GenerateInviteLinkForHousehold(user5);
            List<string> inviteCodes = new List<string> { inviteCode1, inviteCode2, inviteCode3, inviteCode4, inviteCode5 };

            Assert.IsTrue(inviteCodes.Count(x => x == inviteCode1) == 1);
            Assert.IsTrue(inviteCodes.Count(x => x == inviteCode2) == 1);
            Assert.IsTrue(inviteCodes.Count(x => x == inviteCode3) == 1);
            Assert.IsTrue(inviteCodes.Count(x => x == inviteCode4) == 1);
            Assert.IsTrue(inviteCodes.Count(x => x == inviteCode5) == 1);
        }
    }
}