using System.Collections.Generic;
using System.Net.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SaltVault.Core.Authentication;
using SaltVault.Core.Bills;
using SaltVault.Core.Shopping;
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
            billRepository.Setup(x => x.GetAllBasicBillDetails()).Returns(new List<Bill>
            {
                new Bill()
            });
            var shoppingRepository = new Mock<IShoppingRepository>();
            var authentication = new Mock<IAuthentication>();
            authentication.Setup(x => x.CheckKey(It.IsAny<string>())).Returns(true);
            var subject = new ApiController(billRepository.Object, shoppingRepository.Object, authentication.Object, null)
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
            
            var result = subject.GetBillList(null);

            Assert.AreEqual(result.Bills.Count, 1);
        }
    }
}
