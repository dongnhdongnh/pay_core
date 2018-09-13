using System;
using System.Collections.Generic;
using System.Data;
using Dapper;
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
            try
            {
                if (Connection.State != ConnectionState.Open)
                    Connection.Open();
                
                
                var result = Connection.InsertTask<string, BitcoinWithdrawTransaction>(objectInsert);
                var status = !String.IsNullOrEmpty(result) ? Status.StatusSuccess : Status.StatusError;
                return new ReturnObject
                {
                    Status = status,
                    Message = status == Status.StatusError ? "Cannot insert" : "Insert Success",
                    Data = result
                };
            }
            catch (Exception e)
            {
                throw e;
            }
        }


        public BitcoinWithdrawTransaction FindById(string Id)
        {
            throw new System.NotImplementedException();
        }

        public List<BitcoinWithdrawTransaction> FindBySql(string sqlString)
        {
            throw new System.NotImplementedException();
        }
    }
}