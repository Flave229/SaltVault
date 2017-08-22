using System;
using System.Collections.Generic;
using HouseFinance.Core.Bills;
using HouseFinance.Core.People;

namespace HouseFinance.Models
{
    public class PaymentFormHelper
    {
        public BillDetails Bill { get; set; }
        public BillPayment Payment { get; set; }
        public List<Person> People { get; set; }
    }
}