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
            billRepository.Setup(x => x.GetAllBasicBillDetails(It.IsAny<Pagination>(), It.IsAny<int>())).Returns(new List<Bill>
            {
                new Bill()
            });
            var shoppingRepository = new Mock<IShoppingRepository>();
            var userClient = new Mock<IUserService>();
            userClient.Setup(x => x.AuthenticateSession(It.IsAny<string>())).Returns(true);
            userClient.Setup(x => x.GetUserInformationFromAuthHeader(It.IsAny<string>())).Returns(new ActiveUser
            {
                HouseId = 1
            });
            var subject = new BillController(billRepository.Object, null, userClient.Object)
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
