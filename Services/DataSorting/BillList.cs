using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Services.Models.FinanceModels;

namespace Services.DataSorting
{
    public class BillList
    {
        public void OrderBillByDate(ref List<Bill> bills)
        {
            bills = bills.OrderBy(bill => bill.Due).ToList();
        }
    }
}
