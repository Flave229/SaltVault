using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SaltVault.Core.Household;
using SaltVault.Core.Users;

namespace SaltVault.Tests.Household.GivenARequestToCreateAnInviteLink
{
    [TestClass]
    public class WhenTheUserBelongsToAHousehold
    {
        [TestMethod]
        public void ThenTheReturnedCodeContainsSixCharacters()
        {
            InviteLinkService inviteLinkService = new InviteLinkService();
            ActiveUser user = new ActiveUser
            {
                HouseId = 3,
                PersonId = 5,
                Created = DateTime.Now
            };

            string inviteCode = inviteLinkService.GenerateInviteLinkForHousehold(user);

            Assert.AreEqual(6, inviteCode.Length);
        }
    }
}