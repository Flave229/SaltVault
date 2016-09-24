using System;
using System.Collections.Generic;

namespace Services.Models.FinanceModels
{
    public enum RecurringType
    {
        None,
        Monthly,
        Yearly,
        Quarterly
    }

    public class Bill : IPersistedData
    {
        public Guid Id                      { get; set; }
        public string Name                  { get; set; }
        public decimal AmountOwed           { get; set; }
        public List<Guid> AmountPaid        { get; set; }
        public DateTime Due                 { get; set; }
        public List<Guid> People            { get; set; }
        public RecurringType RecurringType  { get; set; }

        public Bill()
        {
            Id = Guid.NewGuid();
            Name = "Unnamed Bill";
            AmountOwed = 0;
            AmountPaid = new List<Guid>();
            Due = new DateTime();
            People = new List<Guid>();
        }
    }
}