using System;

namespace Services.Models.FinanceModels
{
    public interface IPersistedData
    {
        Guid Id { get; set; }
    }
}
