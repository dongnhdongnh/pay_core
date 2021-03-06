﻿using System.Collections.Generic;
using Vakapay.Models.Entities.VAKA;
using Vakapay.Models.Repositories.Base;
using System;
namespace Vakapay.Models.Repositories
{
    public interface
        IVakacoinWithdrawTransactionRepository : IRepositoryBlockchainTransaction<VakacoinWithdrawTransaction>,IDisposable
    {
        string Query_Search(Dictionary<string, string> whereValue);

        string Query_Update(object updateValue, Dictionary<string, string> whereValue);

//		ReturnObject ExcuteSQL(string sqlString, object transaction = null);
        object GetTransaction();
        void TransactionCommit(object transaction);
        void TransactionRollback(object transaction);
    }
}