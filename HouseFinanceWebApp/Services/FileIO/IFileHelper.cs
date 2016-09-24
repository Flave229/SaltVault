using Services.Models;
using System;
using System.Collections.Generic;

namespace Services.FileIO
{
    public interface IFileHelper
    {
        List<T> Open<T>();

        void Save<T>(List<IPersistedData> bills);

        List<IPersistedData> Add<T>(List<IPersistedData> bills, IPersistedData billToAdd);

        List<IPersistedData> Update<T>(List<IPersistedData> bills, IPersistedData updatedBill);

        void Delete<T>(Guid billId);

        void AddOrUpdate<T>(IPersistedData bill);

        void AddOrUpdate<T>(List<IPersistedData> bill);

        T Get<T>(Guid id);

        List<T> GetAll<T>();
    }
}
