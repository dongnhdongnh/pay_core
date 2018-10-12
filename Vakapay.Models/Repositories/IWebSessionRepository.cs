using System.Collections.Generic;
using Vakapay.Models.Entities;
using Vakapay.Models.Repositories.Base;

namespace Vakapay.Models.Repositories
{
    public interface IWebSessionRepository : IRepositoryBase<WebSession>
    {
        string QuerySearch(Dictionary<string, string> models);
        WebSession FindWhere(string sql);
        List<WebSession> GetListLog(string sql, int skip, int take);
    }
}