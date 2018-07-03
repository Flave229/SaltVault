using System;
using System.Linq;
using System.Threading;
using SaltVault.Core.Bills.Models;

namespace SaltVault.Core.Bills
{
    public interface IRecurringBillWorker
    {
        void GenerateNextMonthsBills(int houseId);
    }

    public class RecurringBillWorker : IRecurringBillWorker
    {
        private readonly IBillRepository _billRepository;

        public RecurringBillWorker(IBillRepository billRepository)
        {
            _billRepository = billRepository;
        }

        public void GenerateNextMonthsBills(int houseId)
        {
            // Only works for first household
            var bills = _billRepository.GetAllBasicBillDetails(new Pagination
            {
                Page = 0,
                ResultsPerPage = int.MaxValue
            }, houseId);

            var recurringBills = bills.Where(bill => bill.RecurringType == RecurringType.Monthly);

            foreach (var recurringBill in recurringBills)
            {
                if (recurringBill.FullDateDue > DateTime.Today)
                    continue;

                var billUpdateRequest = new UpdateBill
                {
                    Id = recurringBill.Id,
                    RecurringType = RecurringType.None
                };
                _billRepository.UpdateBill(billUpdateRequest);

                var addBillRequest = new AddBill
                {
                    Due = recurringBill.FullDateDue.AddMonths(1),
                    RecurringType = RecurringType.Monthly,
                    Name = recurringBill.Name,
                    PeopleIds = recurringBill.People.Select(person => person.Id).ToList(),
                    TotalAmount = recurringBill.TotalAmount,
                    HouseId = houseId
                };
                _billRepository.AddBill(addBillRequest);
            }
        }
    }
}