using System;

namespace Services.Models
{
    public interface IPersistedData
    {
        Guid Id { get; set; }
    }
}
