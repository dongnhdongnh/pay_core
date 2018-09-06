using System.Collections.Generic;
using Vakapay.Models.Domains;
using Vakapay.Models.Entities;
using Vakapay.Models.Repositories;

namespace Vakapay.Repositories.Mysql
{
    public class TransactionReposity : ITransactionRepository
    {
        public ReturnObject Update(BitcoinDepositTransaction objectUpdate)
        {
            throw new System.NotImplementedException();
        }

        public ReturnObject Delete(string Id)
        {
            throw new System.NotImplementedException();
        }

        public ReturnObject Insert(BitcoinDepositTransaction objectInsert)
        {
            throw new System.NotImplementedException();
        }

        public BitcoinDepositTransaction FindById(string Id)
        {
            throw new System.NotImplementedException();
        }

        public List<BitcoinDepositTransaction> FindBySql(string sqlString)
        {
            throw new System.NotImplementedException();
        }
    }
}