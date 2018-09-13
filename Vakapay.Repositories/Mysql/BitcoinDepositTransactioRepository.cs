using System;
using System.Collections.Generic;
using System.Data;
using Vakapay.Models.Domains;
using Vakapay.Models.Entities;
using Vakapay.Models.Repositories;
using Vakapay.Models.Repositories.Base;
using Vakapay.Repositories.Mysql.Base;

namespace Vakapay.Repositories.Mysql
{
    public class BitcoinDepositTransactioRepository : MysqlBaseConnection<BitcoinDepositTransactioRepository>, IBitcoinDepositTransactioRepository
    {
        private IBitcoinDepositTransactioRepository _BitcoinDepositTransactioRepositoryImplementation;

        public BitcoinDepositTransactioRepository(string connectionString) : base(connectionString)
        {
        }

        public BitcoinDepositTransactioRepository(IDbConnection dbConnection) : base(dbConnection)
        {
        }

        public ReturnObject Update(BitcoinDepositTransactioRepository objectUpdate)
        {
            throw new System.NotImplementedException();
        }

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

        BitcoinDepositTransaction IRepositoryBase<BitcoinDepositTransaction>.FindById(string Id)
        {
            throw new System.NotImplementedException();
        }

        List<BitcoinDepositTransaction> IRepositoryBase<BitcoinDepositTransaction>.FindBySql(string sqlString)
        {
            throw new System.NotImplementedException();
        }
        
        public ReturnObject Insert(BitcoinDepositTransactioRepository objectInsert)
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

        public void UpdateBlockHash(String txid, String address, string blockhash)
        {
            
        }
    }
}