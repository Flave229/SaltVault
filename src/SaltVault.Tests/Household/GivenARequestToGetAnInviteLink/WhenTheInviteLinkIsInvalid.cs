using Microsoft.VisualStudio.TestTools.UnitTesting;
using SaltVault.Core.Exception;
using SaltVault.Core.Household;

namespace SaltVault.Tests.Household.GivenARequestToGetAnInviteLink
{
    [TestClass]
    public class WhenTheInviteLinkIsInvalid
    {
        [TestMethod]
        public void ThenAnExceptionIsThrown()
        {
            InviteLinkService inviteLinkService = new InviteLinkService();
            
            Assert.ThrowsException<ErrorCodeException>(delegate { inviteLinkService.GetHouseholdForInviteLink("Test"); });
        }
    }
}