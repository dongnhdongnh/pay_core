﻿using System;
using System.Collections.Generic;
using Vakapay.Models.Domains;
using Vakapay.Models.Entities.BTC;
using Vakapay.Models.Repositories.Base;

namespace Vakapay.Models.Repositories
{
    public interface
        IBitcoinWithdrawTransactionRepository : IRepositoryBlockchainTransaction<BitcoinWithdrawTransaction>, IDisposable
    {
        List<BitcoinWithdrawTransaction> FindWhere(BitcoinWithdrawTransaction objectTransaction);
        string QuerySearch(Dictionary<string, string> models);
        string QueryUpdate(object objectUpdate, Dictionary<string, string> whereValue);
        ReturnObject ExecuteSql(string sql);
    }
}