using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Vakapay.Models.Domains;
using Vakapay.Models.Entities;
using Vakapay.Repositories.Mysql.Base;
using Vakapay.Models.Repositories;
namespace Vakapay.Repositories.Mysql
{
    public class InternalTransactionsRepository : MultiThreadUpdateEntityRepository<InternalWithdrawTransaction>, IInternalTransactionRepository
    {
        public InternalTransactionsRepository(string connectionString) : base(connectionString)
        {
        }

        public InternalTransactionsRepository(IDbConnection dbConnection) : base(dbConnection)
        {
        }

        public List<BlockchainTransaction> FindTransactionHistory(int offset, int limit, string[] orderBy)
        {
            throw new System.NotImplementedException();
        }

        public List<BlockchainTransaction> FindTransactionHistoryAll(out int numberData, string userID, string currencyName, string TableNameWithdrawn, string TableNameDeposit, string TableInternalWithdrawn, int offset, int limit, string[] orderBy)
        {
            throw new System.NotImplementedException();
        }

        public List<BlockchainTransaction> FindTransactionsByUserId(string userId)
        {
            throw new System.NotImplementedException();
        }

        public List<BlockchainTransaction> FindTransactionsFromAddress(string fromAddress)
        {
            throw new System.NotImplementedException();
        }

        public List<BlockchainTransaction> FindTransactionsInner(string innerAddress)
        {
            throw new System.NotImplementedException();
        }

        public List<InternalWithdrawTransaction> FindTransactionsNotCompleteOnNet()
        {
            throw new System.NotImplementedException();
        }

        public List<BlockchainTransaction> FindTransactionsToAddress(string toAddress)
        {
            throw new System.NotImplementedException();
        }

        public string GetTableName()
        {
            return TableName;
        }

        public override Task<ReturnObject> SafeUpdate(InternalWithdrawTransaction row)
        {
            return base.SafeUpdate(row, new List<string>());
        }
    }
}