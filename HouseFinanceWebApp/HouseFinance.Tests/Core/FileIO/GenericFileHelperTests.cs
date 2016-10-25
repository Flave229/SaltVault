using System;
using System.Collections.Generic;
using System.IO;
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

        [SetUp]
        public void WhenWhenOpeningTheBillList()
        {
            _filePath = AppDomain.CurrentDomain.BaseDirectory + @"\TestDirectory\billtest.txt";
            var fileHelper = new GenericFileHelper(_filePath);

            var billList = JsonConvert.SerializeObject(new List<Bill>
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
                    RecurringType = RecurringType.None,
                    People = new List<Guid>()
                }
            });

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

        [TearDown]
        public void TearDown()
        {
            File.Delete(_filePath);
        }
    }
}
