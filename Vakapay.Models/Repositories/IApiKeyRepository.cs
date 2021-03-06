﻿using System;
using System.Collections.Generic;
using Vakapay.Models.Entities;
using Vakapay.Models.Repositories.Base;

namespace Vakapay.Models.Repositories
{
    public interface IApiKeyRepository : IRepositoryBase<ApiKey>, IDisposable
    {
        string QuerySearch(Dictionary<string, string> models);
        ApiKey FindWhere(string sql);
        List<ApiKey> GetListApiKey(out int numberData,string sql, int skip, int take, string filter, string sort);
        int GetCount();
    }
}