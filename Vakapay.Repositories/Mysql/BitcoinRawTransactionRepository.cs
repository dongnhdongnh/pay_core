using System.Collections.Generic;
using System.Data;
using Vakapay.Models.Domains;
using Vakapay.Models.Entities;
using Vakapay.Models.Repositories;
using Vakapay.Models.Repositories.Base;
using Vakapay.Repositories.Mysql.Base;

namespace Vakapay.Repositories.Mysql
{
    public class BitcoinRawTransactionRepository : MysqlBaseConnection<BitcoinRawTransactionRepository>, IBitcoinRawTransactionRepository
    {
        private IBitcoinRawTransactionRepository _bitcoinRawTransactionRepositoryImplementation;

        public BitcoinRawTransactionRepository(string connectionString) : base(connectionString)
        {
        }

        public BitcoinRawTransactionRepository(IDbConnection dbConnection) : base(dbConnection)
        {
        }

        public ReturnObject Update(BitcoinRawTransactionRepository objectUpdate)
        {
            throw new System.NotImplementedException();
        }

        public ReturnObject Update(BitcoinWithdrawTransaction objectUpdate)
        {
            throw new System.NotImplementedException();
        }

        public ReturnObject Delete(string Id)
        {
            throw new System.NotImplementedException();
        }

        public ReturnObject Insert(BitcoinWithdrawTransaction objectInsert)
        {
            throw new System.NotImplementedException();
        }

        BitcoinWithdrawTransaction IRepositoryBase<BitcoinWithdrawTransaction>.FindById(string Id)
        {
            throw new System.NotImplementedException();
        }

        List<BitcoinWithdrawTransaction> IRepositoryBase<BitcoinWithdrawTransaction>.FindBySql(string sqlString)
        {
            throw new System.NotImplementedException();
        }

        public ReturnObject Insert(BitcoinRawTransactionRepository objectInsert)
        {
            throw new System.NotImplementedException();
        }

        public BitcoinRawTransactionRepository FindById(string Id)
        {
            throw new System.NotImplementedException();
        }

        public List<BitcoinRawTransactionRepository> FindBySql(string sqlString)
        {
            throw new System.NotImplementedException();
        }
    }
}