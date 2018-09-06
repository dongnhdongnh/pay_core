using System.Data;
using Vakapay.Models.Domains;
using Vakapay.Models.Entities;
using Vakapay.Models.Repositories;
using Vakapay.Repositories.Mysql.Base;

namespace Vakapay.Repositories.Mysql
{
    public class WalletRepository : MysqlBaseConnection<WalletRepository> ,IWalletRepository
    {
        public ReturnObject UpdateBalanceWallet(decimal amount, string Id, int version)
        {
            throw new System.NotImplementedException();
        }

        public Wallet FindWalletById(string Id)
        {
            throw new System.NotImplementedException();
        }

        public Wallet FindWalletByUserId(string userId)
        {
            throw new System.NotImplementedException();
        }

        public Wallet FindWalletBySql(string SqlString)
        {
            throw new System.NotImplementedException();
        }

        public WalletRepository(string connectionString) : base(connectionString)
        {
        }

        public WalletRepository(IDbConnection dbConnection) : base(dbConnection)
        {
        }
    }
}