using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using NLog;
using Vakapay.Models.Domains;
using Vakapay.Models.Entities;
using Vakapay.Models.Repositories;
using Vakapay.Repositories.Mysql.Base;

namespace Vakapay.Repositories.Mysql
{
    public class BitcoinDepositTransactionRepository : MySqlBaseRepository<BitcoinDepositTransaction>,
        IBitcoinDepositTransactionRepository
    {
//        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
//        private const string TableName = "bitcoindeposittransaction";
//
        public BitcoinDepositTransactionRepository(string connectionString) : base(connectionString)
        {
        }

        public BitcoinDepositTransactionRepository(IDbConnection dbConnection) : base(dbConnection)
        {
        }
//
//
//        public ReturnObject Update(BitcoinDepositTransaction objectUpdate)
//        {
//            try
//            {
//                if (Connection.State != ConnectionState.Open)
//                    Connection.Open();
//                var isSuccess = Connection.Update(objectUpdate);
//                var status = !string.IsNullOrEmpty(isSuccess.ToString()) ? Status.StatusSuccess : Status.StatusError;
//                Logger.Debug("BitcoinDepositTransactioRepository =>> insert status: " + status);
//                return new ReturnObject
//                {
//                    Status = Status.StatusError,
//                    Message = status == Status.StatusError ? "Cannot Update" : "Update Success",
//                    Data = ""
//                };
//            }
//            catch (Exception e)
//            {
//                Logger.Error("BitcoinDepositTransactioRepository =>> update fail: " + e.Message);
//                return new ReturnObject
//                {
//                    Status = Status.StatusError,
//                    Message = "Cannot insert: " + e.Message,
//                    Data = ""
//                };
//            }
//        }
//
//        public ReturnObject Delete(string id)
//        {
//            try
//            {
//                if (Connection.State != ConnectionState.Open)
//                    Connection.Open();
//
//                var result = Connection.Delete(new BitcoinDepositTransaction {Id = id});
//                var status = !string.IsNullOrEmpty(result.ToString()) ? Status.StatusSuccess : Status.StatusError;
//                return new ReturnObject
//                {
//                    Status = status,
//                    Message = status == Status.StatusError ? "Cannot insert" : "Insert Success",
//                };
//            }
//            catch (Exception e)
//            {
//                Logger.Error("BitcoinDepositTransactioRepository =>> Delete fail: " + e.Message);
//                throw;
//            }
//        }
//
//        public ReturnObject Insert(BitcoinDepositTransaction objectInsert)
//        {
//            try
//            {
//                if (Connection.State != ConnectionState.Open)
//                    Connection.Open();
//
//
//                var result = Connection.InsertTask<string, BitcoinDepositTransaction>(objectInsert);
//                var status = !string.IsNullOrEmpty(result) ? Status.StatusSuccess : Status.StatusError;
//                Logger.Debug("BitcoinDepositTransactioRepository =>> insert status: " + status);
//                return new ReturnObject
//                {
//                    Status = status,
//                    Message = status == Status.StatusError ? "Cannot insert" : "Insert Success",
//                    Data = result
//                };
//            }
//            catch (Exception e)
//            {
//                Logger.Error("BitcoinDepositTransactioRepository =>> insert fail: " + e.Message);
//                return new ReturnObject
//                {
//                    Status = Status.StatusError,
//                    Message = "Cannot insert: " + e.Message,
//                    Data = ""
//                };
//            }
//        }

        public List<BitcoinDepositTransaction> FindWhere(BitcoinDepositTransaction bitcoinDepositTransaction)
        {
            try
            {
                if (Connection.State != ConnectionState.Open)
                    Connection.Open();


                string sQuery = "SELECT * FROM " + TableName + " WHERE 1 = 1";


                if (!string.IsNullOrEmpty(bitcoinDepositTransaction.Hash))
                    sQuery += " AND Hash" + "='" + bitcoinDepositTransaction.Hash + "'";

                if (!string.IsNullOrEmpty(bitcoinDepositTransaction.ToAddress))
                    sQuery += " AND ToAddress" + "='" + bitcoinDepositTransaction.ToAddress + "'";


                var result = Connection.Query<BitcoinDepositTransaction>(sQuery);
                return result.ToList();
            }
            catch (Exception e)
            {
                Logger.Error("BitcoinDepositTransactioRepository =>> FindWhere fail: " + e.Message);
                return null;
            }
        }

        public BitcoinDepositTransaction FindOneWhere(BitcoinDepositTransaction objectTransaction)
        {
            try
            {
                if (Connection.State != ConnectionState.Open)
                    Connection.Open();


                var sQuery = "SELECT * FROM " + TableName + " WHERE Hash = @HASH";

                var result = Connection.QueryFirstOrDefault<BitcoinDepositTransaction>(sQuery,
                    new {HASH = objectTransaction.Hash});

                return result;
            }
            catch (Exception e)
            {
                Logger.Error("BitcoinDepositTransactioRepository =>> FindWhere fail: " + e.Message);
                return null;
            }
        }

//        public BitcoinDepositTransaction FindById(string id)
//        {
//            try
//            {
//                if (Connection.State != ConnectionState.Open)
//                    Connection.Open();
//
//                var sQuery = "SELECT * FROM " + TableName + " WHERE Id = @ID";
//
//                var result = Connection.QuerySingleOrDefault<BitcoinDepositTransaction>(sQuery, new {ID = id});
//
//
//                return result;
//            }
//            catch (Exception e)
//            {
//                Logger.Error("BitcoinDepositTransactioRepository =>> FindById fail: " + e.Message);
//                return null;
//            }
//        }
//
//        public List<BitcoinDepositTransaction> FindBySql(string sqlString)
//        {
//            try
//            {
//                if (Connection.State != ConnectionState.Open)
//                    Connection.Open();
//
//                var result = Connection.Query<BitcoinDepositTransaction>(sqlString);
//
//                return result.ToList();
//            }
//            catch (Exception e)
//            {
//                Logger.Error("BitcoinDepositTransactioRepository =>> insert fail: " + e.Message);
//                return null;
//            }
//        }


        public BlockchainTransaction FindTransactionPending()
        {
            return new BitcoinDepositTransaction();
        }

        public List<BlockchainTransaction> FindTransactionsPending()
        {
            throw new NotImplementedException();
        }

        public BlockchainTransaction FindTransactionError()
        {
            throw new NotImplementedException();
        }

        public BlockchainTransaction FindTransactionByStatus(string status)
        {
            throw new NotImplementedException();
        }

        public async Task<ReturnObject> LockForProcess(BlockchainTransaction transaction)
        {
            try
            {
                if (Connection.State != ConnectionState.Open)
                    Connection.Open();
                string sqlCommand = "Update " + TableName +
                                    " Set Version = Version + 1, OnProcess = 1 Where Id = @ID and Version = @VERSION";

                var update = Connection.Execute(sqlCommand, new {ID = transaction.Id, VERSION = transaction.Version});
                if (update == 1)
                {
                    return new ReturnObject
                    {
                        Status = Status.StatusSuccess,
                        Message = "Update Success",
                    };
                }

                return new ReturnObject
                {
                    Status = Status.StatusError,
                    Message = "Update Fail",
                };
            }
            catch (Exception e)
            {
                return new ReturnObject
                {
                    Status = Status.StatusError,
                    Message = e.ToString()
                };
            }
        }

        public async Task<ReturnObject> ReleaseLock(BlockchainTransaction transaction)
        {
            throw new NotImplementedException();
        }

        public async Task<ReturnObject> SafeUpdate(BlockchainTransaction transaction)
        {
            throw new NotImplementedException();
        }

		public List<BlockchainTransaction> FindTransactionsByStatus(string status)
		{
			throw new NotImplementedException();
		}

		public List<BlockchainTransaction> FindTransactionsInProcess()
		{
			throw new NotImplementedException();
		}
	}
}