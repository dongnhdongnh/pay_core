using System.Collections.Generic;
using Vakapay.Models.Entities;
using Vakapay.Models.Repositories.Base;

namespace Vakapay.Models.Repositories
{
    public interface IConfirmedDevicesRepository : IRepositoryBase<ConfirmedDevices>
    {
        string QuerySearch(Dictionary<string, string> models);
        ConfirmedDevices FindWhere(string sql);
        List<ConfirmedDevices> GetListConfirmedDevices(out int numberData, string sql, int skip, int take, string filter, string sort);
        int GetCount();
    }
}