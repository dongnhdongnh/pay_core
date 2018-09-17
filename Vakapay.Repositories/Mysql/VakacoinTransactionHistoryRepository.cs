using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using Vakapay.Models.Domains;
using Vakapay.Models.Entities;
using Vakapay.Models.Repositories;
using Vakapay.Repositories.Mysql.Base;

namespace Vakapay.Repositories.Mysql
{
    public class VakacoinTransactionHistoryRepository : MysqlBaseConnection, IVakacoinTransactionHistoryRepository
    {
        public VakacoinTransactionHistoryRepository(string connectionString) : base(connectionString)
        {
        }

        public VakacoinTransactionHistoryRepository(IDbConnection dbConnection) : base(dbConnection)
        {
        }

        public ReturnObject Update(VakacoinTransactionHistory objectUpdate)
        {
            throw new System.NotImplementedException();
        }

        public ReturnObject Delete(string Id)
        {
            throw new System.NotImplementedException();
        }

        public ReturnObject Insert(VakacoinTransactionHistory objectInsert)
        {
            try
            {
                if(Connection.State != ConnectionState.Open)
                    Connection.Open();
                var result = Connection.InsertTask<string, VakacoinTransactionHistory>(objectInsert);
                var status = !String.IsNullOrEmpty(result) ? Status.StatusSuccess : Status.StatusError;
                return new ReturnObject
                {
                    Status = status,
                    Message = status == Status.StatusError ? "Cannot insert" : "Insert Success"
                };
            }
            catch (Exception e)
            {
                return new ReturnObject
                {
                    Status = Status.StatusError,
                    Message = e.Message
                };
            }
        }

        public VakacoinTransactionHistory FindById(string Id)
        {
            throw new System.NotImplementedException();
        }

        public List<VakacoinTransactionHistory> FindBySql(string sqlString)
        {
            throw new System.NotImplementedException();
        }


        public IBlockchainTransaction FindTransactionPending()
        {
            throw new NotImplementedException();
        }

        public IBlockchainTransaction FindTransactionError()
        {
            throw new NotImplementedException();
        }

        public IBlockchainTransaction FindTransactionByStatus(string status)
        {
            throw new NotImplementedException();
        }

        public async Task<ReturnObject> LockForProcess(IBlockchainTransaction transaction)
        {
            throw new NotImplementedException();
        }

        public async Task<ReturnObject> ReleaseLock(IBlockchainTransaction transaction)
        {
            throw new NotImplementedException();
        }

        public async Task<ReturnObject> SafeUpdate(IBlockchainTransaction transaction)
        {
            throw new NotImplementedException();
        }
    }
}