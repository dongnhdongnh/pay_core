using System.Collections.Generic;
using Vakapay.Models.Entities;
using Vakapay.Models.Repositories.Base;
using System;
namespace Vakapay.Models.Repositories
{
    public interface IWebSessionRepository : IRepositoryBase<WebSession>,IDisposable
    {
        string QuerySearch(Dictionary<string, string> models);
        WebSession FindWhere(string sql);
        List<WebSession> GetListWebSession(string sql, int skip, int take);
        int GetCount();
    }
}