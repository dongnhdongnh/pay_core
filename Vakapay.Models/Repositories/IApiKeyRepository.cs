using System.Collections.Generic;
using Vakapay.Models.Entities;
using Vakapay.Models.Repositories.Base;

namespace Vakapay.Models.Repositories
{
    public interface IApiKeyRepository : IRepositoryBase<ApiKey>
    {
        string QuerySearch(Dictionary<string, string> models);
        ApiKey FindWhere(string sql);
        List<ApiKey> GetListApiKey(string sql, int skip, int take);
        int GetCount();
    }
}