using System;

namespace HouseFinance.Core.FileManagement
{
    public interface IPersistedData
    {
        Guid Id { get; set; }
    }
}
