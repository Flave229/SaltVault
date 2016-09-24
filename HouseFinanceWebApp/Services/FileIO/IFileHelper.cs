using Services.Models.FinanceModels;
using System;
using System.Collections.Generic;

namespace Services.FileIO
{
    public interface IFileHelper
    {
        List<IFinanceModel> Open();

        void Save(List<IFinanceModel> bills);

        List<IFinanceModel> Add(List<IFinanceModel> bills, IFinanceModel billToAdd);

        List<IFinanceModel> Update(List<IFinanceModel> bills, IFinanceModel updatedBill);

        void Delete(Guid billId);

        void AddOrUpdate(IFinanceModel bill);

        void AddOrUpdate(List<IFinanceModel> bill);

        IFinanceModel Get(Guid id);

        IFinanceModel Get(string name);

        List<IFinanceModel> GetAll();
    }
}
