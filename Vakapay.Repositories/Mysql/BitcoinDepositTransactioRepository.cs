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
using Vakapay.Models.Repositories.Base;
using Vakapay.Repositories.Mysql.Base;

namespace Vakapay.Repositories.Mysql
{
    public class BitcoinDepositTransactioRepository : MysqlBaseConnection,
        IBitcoinDepositTransactioRepository
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        public static string tableName = "bitcoindeposittransaction";
        private IBitcoinDepositTransactioRepository _BitcoinDepositTransactioRepositoryImplementation;

        public BitcoinDepositTransactioRepository(string connectionString) : base(connectionString)
        {
        }

        public BitcoinDepositTransactioRepository(IDbConnection dbConnection) : base(dbConnection)
        {
        }


        public ReturnObject Update(BitcoinDepositTransaction objectUpdate)
        {
            try
            {
                if (Connection.State != ConnectionState.Open)
                    Connection.Open();
                var isSuccess = Connection.Update(objectUpdate);
                var status = !string.IsNullOrEmpty(isSuccess.ToString()) ? Status.StatusSuccess : Status.StatusError;
                logger.Debug("BitcoinDepositTransactioRepository =>> insert status: " + status);
                return new ReturnObject
                {
                    Status = Status.StatusError,
                    Message = status == Status.StatusError ? "Cannot Update" : "Update Success",
                    Data = ""
                };
            }
            catch (Exception e)
            {
                logger.Error("BitcoinDepositTransactioRepository =>> update fail: " + e.Message);
                return new ReturnObject
                {
                    Status = Status.StatusError,
                    Message = "Cannot insert: " + e.Message,
                    Data = ""
                };
            }
        }

        public ReturnObject Delete(string Id)
        {
            try
            {
                if (Connection.State != ConnectionState.Open)
                    Connection.Open();

                var result = Connection.Delete(new BitcoinDepositTransaction {Id = Id});
                var status = !string.IsNullOrEmpty(result.ToString()) ? Status.StatusSuccess : Status.StatusError;
                return new ReturnObject
                {
                    Status = status,
                    Message = status == Status.StatusError ? "Cannot insert" : "Insert Success",
                };
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public ReturnObject Insert(BitcoinDepositTransaction objectInsert)
        {
            try
            {
                if (Connection.State != ConnectionState.Open)
                    Connection.Open();


                var result = Connection.InsertTask<string, BitcoinDepositTransaction>(objectInsert);
                var status = !string.IsNullOrEmpty(result) ? Status.StatusSuccess : Status.StatusError;
                logger.Debug("BitcoinDepositTransactioRepository =>> insert status: " + status);
                return new ReturnObject
                {
                    Status = status,
                    Message = status == Status.StatusError ? "Cannot insert" : "Insert Success",
                    Data = result
                };
            }
            catch (Exception e)
            {
                logger.Error("BitcoinDepositTransactioRepository =>> insert fail: " + e.Message);
                return new ReturnObject
                {
                    Status = Status.StatusError,
                    Message = "Cannot insert: " + e.Message,
                    Data = ""
                };
            }
        }
        
        public List<BitcoinDepositTransaction> FindWhere(BitcoinDepositTransaction rawtransaction)
        {
            try
            {
                if (Connection.State != ConnectionState.Open)
                    Connection.Open();


                string sQuery = "SELECT * FROM bitcoindeposittransaction WHERE 1 = 1";


                if (!string.IsNullOrEmpty(rawtransaction.Hash))
                    sQuery += " AND Hash" + "='" + rawtransaction.Hash + "'";

                if (!string.IsNullOrEmpty(rawtransaction.ToAddress))
                    sQuery += " AND ToAddress" + "='" + rawtransaction.ToAddress + "'";


                var result = Connection.Query<BitcoinDepositTransaction>(sQuery);
                return result.ToList();
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        
        public BitcoinDepositTransaction FindById(string Id)
        {
            try
            {
                if (Connection.State != ConnectionState.Open)
                    Connection.Open();

                string sQuery = "SELECT * FROM bitcoindeposittransaction WHERE Id = @ID";

                var result = Connection.QuerySingleOrDefault<BitcoinDepositTransaction>(sQuery, new {ID = Id});


                return result;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public List<BitcoinDepositTransaction> FindBySql(string sqlString)
        {
            try
            {
                if (Connection.State != ConnectionState.Open)
                    Connection.Open();

                var result = Connection.Query<BitcoinDepositTransaction>(sqlString);

                return result.ToList();
            }
            catch (Exception e)
            {
                logger.Error("BitcoinDepositTransactioRepository =>> insert fail: " + e.Message);
                throw e;
            }
        }

        

        

        public IBlockchainTransaction FindTransactionPending()
        {
            return new BitcoinDepositTransaction();
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
            try
            {
                if (Connection.State != ConnectionState.Open)
                    Connection.Open();
                string SqlCommand = "Update " + tableName + " Set Version = Version + 1, OnProcess = 1 Where Id = @Id and Version = @Version";

                var update = Connection.Execute(SqlCommand, new {Id = transaction.Id, Version = transaction.Version});
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