using System.Data;
using Vakapay.Models.Entities;
using Vakapay.Repositories.Mysql.Base;

namespace Vakapay.Repositories.Mysql
{
    public class InternalTransactionsRepository : MultiThreadUpdateEntityRepository<InternalWithdrawTransaction>
    {
        public InternalTransactionsRepository(string connectionString) : base(connectionString)
        {
        }

        public InternalTransactionsRepository(IDbConnection dbConnection) : base(dbConnection)
        {
        }
    }
}