using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SaltVault.Core;
using SaltVault.Core.Bills;
using SaltVault.Core.Shopping;
using SaltVault.Core.Users;
using SaltVault.WebApp.Controllers;

namespace SaltVault.Tests.Bills.GivenABillListRequest
{
    [TestClass]
    public class WhenTheRequestIsValid
    {
        [TestMethod]
        public void ThenTheBillListIsReturned()
        {
            var billRepository = new Mock<IBillRepository>();
            billRepository.Setup(x => x.GetAllBasicBillDetails(It.IsAny<Pagination>())).Returns(new List<Bill>
            {
                new Bill()
            });
            var shoppingRepository = new Mock<IShoppingRepository>();
            var userClient = new Mock<IUserService>();
            userClient.Setup(x => x.AuthenticateSession(It.IsAny<string>())).Returns(true);
            var subject = new ApiController(billRepository.Object, shoppingRepository.Object, null, null, null, userClient.Object, null)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext
                    {
                        Request =
                        {
                            Headers = {{"Authorization", "Test Token"}}
                        }
                    }
                }
            };
            
            var result = subject.GetBillList(null, null, null);

            Assert.AreEqual(result.Bills.Count, 1);
        }
    }
}
