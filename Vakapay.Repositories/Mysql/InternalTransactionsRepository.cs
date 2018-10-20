using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Vakapay.Models.Domains;
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

        public override Task<ReturnObject> SafeUpdate(InternalWithdrawTransaction row)
        {
            return base.SafeUpdate(row, new List<string>());
        }
    }
}