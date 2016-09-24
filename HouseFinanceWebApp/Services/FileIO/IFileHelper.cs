using Services.Models;
using System;
using System.Collections.Generic;

namespace Services.FileIO
{
    public interface IFileHelper
    {
        List<IPersistedData> Open();

        void Save(List<IPersistedData> bills);

        List<IPersistedData> Add(List<IPersistedData> bills, IPersistedData billToAdd);

        List<IPersistedData> Update(List<IPersistedData> bills, IPersistedData updatedBill);

        void Delete(Guid billId);

        void AddOrUpdate(IPersistedData bill);

        void AddOrUpdate(List<IPersistedData> bill);

        IPersistedData Get(Guid id);

        List<IPersistedData> GetAll();
    }
}
