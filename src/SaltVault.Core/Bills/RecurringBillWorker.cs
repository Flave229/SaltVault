using System;
using System.Linq;
using System.Threading;
using SaltVault.Core.Bills.Models;

namespace SaltVault.Core.Bills
{
    public class RecurringBillWorker
    {
        private readonly IBillRepository _billRepository;

        public RecurringBillWorker(IBillRepository billRepository)
        {
            _billRepository = billRepository;
        }

        public void GenerateNextMonthsBills()
        {
            var bills = _billRepository.GetAllBasicBillDetails();

            var recurringBills = bills.Where(bill => bill.RecurringType == RecurringType.Monthly);

            foreach (var recurringBill in recurringBills)
            {
                if (recurringBill.FullDateDue > DateTime.Today)
                    continue;

                var billUpdateRequest = new UpdateBillRequest
                {
                    Id = recurringBill.Id,
                    RecurringType = RecurringType.None
                };
                _billRepository.UpdateBill(billUpdateRequest);

                var addBillRequest = new AddBillRequest
                {
                    Due = recurringBill.FullDateDue.AddMonths(1),
                    RecurringType = RecurringType.Monthly,
                    Name = recurringBill.Name,
                    PeopleIds = recurringBill.People.Select(person => person.Id).ToList(),
                    TotalAmount = recurringBill.TotalAmount
                };
                _billRepository.AddBill(addBillRequest);
            }
        }

        public void StartWorker()
        {
            try
            {
                while (true)
                {
                    GenerateNextMonthsBills();
                    Thread.Sleep(3600000);
                }
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }
    }
}