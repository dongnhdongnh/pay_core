using System.Collections.Generic;
using System.Data;
using Vakapay.Models.Domains;
using Vakapay.Models.Entities;
using Vakapay.Models.Repositories;
using Vakapay.Repositories.Mysql.Base;

namespace Vakapay.Repositories.Mysql
{
    public class WalletRepository : MysqlBaseConnection<WalletRepository> ,IWalletRepository
    {
        public WalletRepository(string connectionString) : base(connectionString)
        {
        }

        public WalletRepository(IDbConnection dbConnection) : base(dbConnection)
        {
        }

        public ReturnObject Update(Wallet objectUpdate)
        {
            throw new System.NotImplementedException();
        }

        public ReturnObject Delete(string Id)
        {
            throw new System.NotImplementedException();
        }

        public ReturnObject Insert(Wallet objectInsert)
        {
            throw new System.NotImplementedException();
        }

        public Wallet FindById(string Id)
        {
            throw new System.NotImplementedException();
        }

        public List<Wallet> FindBySql(string sqlString)
        {
            throw new System.NotImplementedException();
        }

        public ReturnObject UpdateBalanceWallet(decimal amount, string Id, int version)
        {
            throw new System.NotImplementedException();
        }
    }
}