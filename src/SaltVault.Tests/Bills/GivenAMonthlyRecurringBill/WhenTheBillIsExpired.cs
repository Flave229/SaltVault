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
    public class WhenTheBillIsExpired
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
                    FullDateDue = DateTime.Today.AddDays(-1)
                }
            });
            var subject = new RecurringBillWorker(_billRepository.Object);
            subject.GenerateNextMonthsBills();
        }

        [TestMethod]
        public void ThenTheBillForNextMonthIsAMonthlyRecurringBill()
        {
            _billRepository.Verify(x => x.AddBill(It.Is<AddBill>(y => y.RecurringType == RecurringType.Monthly)), Times.Once);
        }

        [TestMethod]
        public void ThenTheBillForNextMonthIsForNextMonth()
        {
            _billRepository.Verify(x => x.AddBill(It.Is<AddBill>(y => y.Due == DateTime.Today.AddDays(-1).AddMonths(1))), Times.Once);
        }

        [TestMethod]
        public void ThenThePreviousBillIsUpdatedWithNoRecurringType()
        {
            _billRepository.Verify(x => x.UpdateBill(It.Is<UpdateBillRequest>(y => y.RecurringType == RecurringType.None)), Times.Once);
        }
    }
}
