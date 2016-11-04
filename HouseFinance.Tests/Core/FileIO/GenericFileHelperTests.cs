using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using NUnit.Framework;
using Services.FileIO;
using Services.Models.FinanceModels;

namespace HouseFinance.Tests.Core.FileIO
{
    [TestFixture]
    class GivenAValidListOfBills
    {
        private string _filePath;
        private List<Bill> _subject;
        private List<Bill> _originalBillList;

        [SetUp]
        public void WhenWhenOpeningTheBillList()
        {
            _filePath = AppDomain.CurrentDomain.BaseDirectory + @"\TestDirectory\billtest.txt";
            var fileHelper = new GenericFileHelper(_filePath);

            _originalBillList = new List<Bill>
            {
                new Bill
                {
                    AmountOwed = 10,
                    AmountPaid = new List<Guid>(),
                    Due = new DateTime(2016, 10, 25),
                    Name = "Test1",
                    RecurringType = RecurringType.None,
                    People = new List<Guid>()
                },
                new Bill
                {
                    AmountOwed = 229,
                    AmountPaid = new List<Guid>
                    {
                        new Guid("2C362C71-29DD-4ABB-BC0F-409D85569B74")
                    },
                    Due = new DateTime(2016, 10, 25),
                    Name = "Test2",
                    RecurringType = RecurringType.Monthly,
                    People = new List<Guid>()
                }
            };

            var billList = JsonConvert.SerializeObject(_originalBillList);

            var directoryInfo = new FileInfo(_filePath);
            directoryInfo.Directory?.Create();
            File.WriteAllText(directoryInfo.FullName, billList);

            _subject = fileHelper.Open<Bill>();
        }

        [Test]
        public void ThenTheCorrectAmountOfBillsAreReturned()
        {
            Assert.That(_subject.Count, Is.EqualTo(2));
        }

        [Test]
        public void ThenTheDataForBillsAreMappedCorrectly()
        {
            Assert.That(_subject[0].Due, Is.EqualTo(_originalBillList[0].Due));
            Assert.That(_subject[0].AmountOwed, Is.EqualTo(_originalBillList[0].AmountOwed));
            Assert.That(_subject[0].AmountPaid, Is.EqualTo(_originalBillList[0].AmountPaid));
            Assert.That(_subject[0].Name, Is.EqualTo(_originalBillList[0].Name));
            Assert.That(_subject[0].People, Is.EqualTo(_originalBillList[0].People));
            Assert.That(_subject[0].RecurringType, Is.EqualTo(_originalBillList[0].RecurringType));
        }

        [TearDown]
        public void TearDown()
        {
            File.Delete(_filePath);
        }
    }
}
