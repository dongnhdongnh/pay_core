using System;
using System.Collections.Generic;
using Vakapay.Models.Entities;
using Vakapay.Models.Repositories.Base;

namespace Vakapay.Models.Repositories
{
    public interface IUserActionLogRepository : IRepositoryBase<UserActionLog>, IDisposable
    {
        string QuerySearch(Dictionary<string, string> models);
        UserActionLog FindWhere(string sql);
        List<UserActionLog> GetListLog(out int numberData, string sql, int skip, int take, string filter, string sort);
    }
}