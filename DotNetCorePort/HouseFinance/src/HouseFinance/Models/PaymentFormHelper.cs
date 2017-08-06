using System;
using System.Collections.Generic;
using HouseFinance.Core.Bills;
using HouseFinance.Core.Bills.Payments;
using HouseFinance.Core.People;

namespace HouseFinance.Models
{
    public class PaymentFormHelper
    {
        public Bill Bill { get; set; }
        public Payment Payment { get; set; }
        public List<Person> People { get; set; }
    }
}