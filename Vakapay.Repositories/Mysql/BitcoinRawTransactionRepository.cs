using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper;
using NLog;
using Vakapay.Models.Domains;
using Vakapay.Models.Entities;
using Vakapay.Models.Repositories;
using Vakapay.Models.Repositories.Base;
using Vakapay.Repositories.Mysql.Base;

namespace Vakapay.Repositories.Mysql
{
    public class BitcoinRawTransactionRepository : MysqlBaseConnection,
        IBitcoinRawTransactionRepository
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

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
            try
            {
                if (Connection.State != ConnectionState.Open)
                    Connection.Open();

                var isSuccess = Connection.Update(objectUpdate);
                var status = !string.IsNullOrEmpty(isSuccess.ToString()) ? Status.StatusSuccess : Status.StatusError;
                logger.Debug("BitcoinRawTransactionRepository =>> insert status: " + status);
                return new ReturnObject
                {
                    Status = Status.StatusError,
                    Message = status == Status.StatusError ? "Cannot Update" : "Update Success",
                    Data = ""
                };
            }
            catch (Exception e)
            {
                logger.Error("BitcoinRawTransactionRepository =>> update fail: " + e.Message);
                return new ReturnObject
                {
                    Status = Status.StatusError,
                    Message = "Cannot update: " + e.Message,
                    Data = ""
                };
            }
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
            try
            {
                if (Connection.State != ConnectionState.Open)
                    Connection.Open();

                var result = Connection.Query<BitcoinWithdrawTransaction>(sqlString);

                return result.ToList();
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}