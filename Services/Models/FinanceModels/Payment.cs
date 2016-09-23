using System;

namespace Services.Models.FinanceModels
{
    public class Payment
    {
        public Guid Id          { get; set; }
        public decimal Amount   { get; set; }
        public DateTime Created { get; set; }
        public Guid PersonId    { get; set; }

        public Payment()
        {
            Id = Guid.NewGuid();
            Amount = 0;
            PersonId = new Guid();
        }
    }
}