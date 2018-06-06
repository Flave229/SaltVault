using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SaltVault.Core.Household;
using SaltVault.Core.Users;

namespace SaltVault.Tests.Household.GivenARequestToCreateAnInviteLink
{
    [TestClass]
    public class WhenTheUserBelongsToAHouseholdAndAValidInviteLinkAlreadyExists
    {
        [TestMethod]
        public void ThenTheReturnedCodeIsIdenticalToTheFirstCode()
        {
            InviteLinkService inviteLinkService = new InviteLinkService();
            ActiveUser user = new ActiveUser
            {
                HouseId = 3,
                PersonId = 5,
                Created = DateTime.Now
            };

            string firstInviteCode = inviteLinkService.GenerateInviteLinkForHousehold(user);
            string secondInviteCode = inviteLinkService.GenerateInviteLinkForHousehold(user);

            Assert.AreEqual(firstInviteCode, secondInviteCode);
        }
    }
}