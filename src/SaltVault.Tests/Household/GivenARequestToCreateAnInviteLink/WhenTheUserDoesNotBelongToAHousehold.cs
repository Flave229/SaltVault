using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SaltVault.Core.Exception;
using SaltVault.Core.Household;
using SaltVault.Core.Users;

namespace SaltVault.Tests.Household.GivenARequestToCreateAnInviteLink
{
    [TestClass]
    public class WhenTheUserDoesNotBelongToAHousehold
    {
        [TestMethod]
        public void ThenAnExceptionIsReturned()
        {
            InviteLinkService inviteLinkService = new InviteLinkService();
            ActiveUser user = new ActiveUser
            {
                HouseId = -1,
                PersonId = 5,
                Created = DateTime.Now
            };

            Assert.ThrowsException<ErrorCodeException>(delegate { inviteLinkService.GenerateInviteLinkForHousehold(user); });
        }
    }
}