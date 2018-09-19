using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using NLog;
using Vakapay.Models.Domains;
using Vakapay.Models.Entities;
using Vakapay.Models.Repositories;
using Vakapay.Repositories.Mysql.Base;

namespace Vakapay.Repositories.Mysql
{
    public class EmailRepository : MysqlBaseConnection, IEmailRepository
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private const string TableName = "emailQueue";

        public EmailRepository(string connectionString) : base(connectionString)
        {
        }

        public EmailRepository(IDbConnection dbConnection) : base(dbConnection)
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

        public ReturnObject Update(EmailQueue objectUpdate)
        {
            throw new System.NotImplementedException();
        }

        public ReturnObject Delete(string id)
        {
            throw new System.NotImplementedException();
        }

        public ReturnObject Insert(EmailQueue objectInsert)
        {
            try
            {
                if (Connection.State != ConnectionState.Open)
                    Connection.Open();


                var result = Connection.InsertTask<string, EmailQueue>(objectInsert);
                var status = !string.IsNullOrEmpty(result) ? Status.StatusSuccess : Status.StatusError;
                Logger.Debug("EmailRepository =>> insert status: " + status);
                return new ReturnObject
                {
                    Status = status,
                    Message = status == Status.StatusError ? "EmailRepository =>> Cannot insert" : "Insert Success",
                    Data = result
                };
            }
            catch (Exception e)
            {
                Logger.Error("EmailRepository =>> insert fail: " + e.Message);
                return new ReturnObject
                {
                    Status = Status.StatusError,
                    Message = "EmailRepository =>> Cannot insert: " + e.Message,
                    Data = ""
                };
            }
        }

        public EmailQueue FindById(string id)
        {
            throw new System.NotImplementedException();
        }

        public List<EmailQueue> FindBySql(string sqlString)
        {
            throw new System.NotImplementedException();
        }

        public List<BitcoinDepositTransaction> FindWhere(EmailQueue emailQueue)
        {
            throw new System.NotImplementedException();
        }
    }
}