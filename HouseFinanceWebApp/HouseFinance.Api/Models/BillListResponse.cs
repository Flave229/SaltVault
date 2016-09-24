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
        public DateTime DateDue { get; set; }
        public decimal AmountDue { get; set; }
        public List<Images> PeopleImages { get; set; }

        public BillDetails()
        {
            Name = "";
            DateDue = new DateTime();
            AmountDue = 0;
            PeopleImages = new List<Images>();
        }
    }

    public class Images
    {
        public string Link { get; set; }
    }
}
