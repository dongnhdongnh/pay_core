using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Vakapay.Models.Domains;
using Vakapay.Models.Entities;
using Vakapay.Models.Repositories;
using Vakapay.Repositories.Mysql.Base;

namespace Vakapay.Repositories.Mysql
{
    public class PendingVakacoinTransactionRepository : MysqlBaseConnection, IPendingVakacoinTransactionRepository
    {
        public PendingVakacoinTransactionRepository(string connectionString) : base(connectionString)
        {
        }

        public PendingVakacoinTransactionRepository(IDbConnection dbConnection) : base(dbConnection)
        {
        }

        public BlockchainTransaction FindTransactionPending()
        {
            throw new System.NotImplementedException();
        }

        public BlockchainTransaction FindTransactionError()
        {
            throw new System.NotImplementedException();
        }

        public BlockchainTransaction FindTransactionByStatus(string status)
        {
            throw new System.NotImplementedException();
        }

        public Task<ReturnObject> LockForProcess(BlockchainTransaction transaction)
        {
            throw new System.NotImplementedException();
        }

        public Task<ReturnObject> ReleaseLock(BlockchainTransaction transaction)
        {
            throw new System.NotImplementedException();
        }

        public Task<ReturnObject> SafeUpdate(BlockchainTransaction transaction)
        {
            throw new System.NotImplementedException();
        }

        public ReturnObject Update(PendingVakacoinTransaction objectUpdate)
        {
            throw new System.NotImplementedException();
        }

        public ReturnObject Delete(string Id)
        {
            throw new System.NotImplementedException();
        }

        public ReturnObject Insert(PendingVakacoinTransaction objectInsert)
        {
            throw new System.NotImplementedException();
        }

        public PendingVakacoinTransaction FindById(string Id)
        {
            throw new System.NotImplementedException();
        }

        public List<PendingVakacoinTransaction> FindBySql(string sqlString)
        {
            try
            {
                if (Connection.State != ConnectionState.Open)
                    Connection.Open();
                var result = Connection.Query<PendingVakacoinTransaction>(sqlString);                
                return  result.ToList();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        public ReturnObject UpdateTransaction(string id, string status, int version)
        {
            try
            {
                if (Connection.State != ConnectionState.Open)
                    Connection.Open();
                string updateSql = "UPDATE VakacoinTransactionHistory SET Status = @STATUS WHERE Id = @ID AND Version = @VERSION";
                
                var result = Connection.Query(updateSql, new {ID = id, VERSION = version, STATUS = status});
                
                var rsStatus = !String.IsNullOrEmpty(result.ToString()) ? Status.StatusSuccess : Status.StatusError;
                return new ReturnObject
                {
                    Status = rsStatus,
                    Message = rsStatus == Status.StatusError ? "Cannot insert" : "Insert Success"
                };
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}