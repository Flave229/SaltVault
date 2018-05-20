using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SaltVault.Core;
using SaltVault.Core.Bills;
using SaltVault.Core.Bills.Models;

namespace SaltVault.Tests.Bills.GivenAMonthlyRecurringBill
{
    [TestClass]
    public class WhenTheBillIsDueInTheFuture
    {
        private Mock<IBillRepository> _billRepository;

        [TestInitialize]
        public void Setup()
        {
            _billRepository = new Mock<IBillRepository>();
            _billRepository.Setup(x => x.GetAllBasicBillDetails(It.IsAny<Pagination>(), It.IsAny<int>())).Returns(new List<Bill>
            {
                new Bill
                {
                    RecurringType = RecurringType.Monthly,
                    FullDateDue = DateTime.Today.AddDays(10)
                }
            });
            var subject = new RecurringBillWorker(_billRepository.Object);
            subject.GenerateNextMonthsBills();
        }

        [TestMethod]
        public void ThenNoNewBillIsAdded()
        {
            _billRepository.Verify(x => x.AddBill(It.IsAny<AddBill>()), Times.Never);
        }

        [TestMethod]
        public void ThenThePreviousBillIsNotUpdated()
        {
            _billRepository.Verify(x => x.UpdateBill(It.IsAny<UpdateBill>()), Times.Never);
        }
    }
}
