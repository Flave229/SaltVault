using System;
using System.Collections.Generic;

namespace HouseFinance.Api.Models
{
    public class BillListResponse
    {
        public List<BillDetails> BillList { get; set; }

        public BillListResponse()
        {
            BillList = new List<BillDetails>();
        }
    }

    public class BillDetails
    {
        public string Name { get; set; }
        public DateTime FullDateDue { get; set; }
        public string DateDue { get; set; }
        public bool Overdue { get; set; }
        public bool Paid { get; set; }
        public decimal AmountDue { get; set; }
        public List<string> PeopleImages { get; set; }

        public BillDetails()
        {
            Name = "";
            FullDateDue = new DateTime();
            DateDue = new DateTime().ToString("yyyy-MM-dd");
            AmountDue = 0;
            PeopleImages = new List<string>();
        }
    }
}
