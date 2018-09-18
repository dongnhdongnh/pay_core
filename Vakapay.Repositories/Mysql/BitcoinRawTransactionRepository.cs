using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using NLog;
using Vakapay.Models.Domains;
using Vakapay.Models.Entities;
using Vakapay.Models.Repositories;
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

        private const string TableName = "bitcoinwithdrawtransaction";

        public string QuerySearch(Dictionary<string, string> models)
        {
            var sQuery = "SELECT * FROM bitcoinwithdrawtransaction WHERE 1 = 1";
            foreach (var model in models)
            {
                sQuery += string.Format(" AND {0}='{1}'", model.Key, model.Value);
            }

            return sQuery;
        }

        public string QueryUpdate(object updateValue, Dictionary<string, string> whereValue)
        {
            var updateStr = new StringBuilder("");
            var whereStr = new StringBuilder("");

            var count = 0;

            foreach (var prop in updateValue.GetType().GetProperties())
            {
                if (prop.GetValue(updateValue, null) == null) continue;
                if (count > 0)
                    updateStr.Append(",");
                updateStr.AppendFormat(" {0}='{1}'", prop.Name, prop.GetValue(updateValue, null));
                count++;
            }

            count = 0;
            foreach (var model in whereValue)
            {
                if (count > 0)
                    whereStr.Append(" AND ");
                whereStr.AppendFormat(" {0}='{1}'", model.Key, model.Value);
                count++;
            }


            var output = string.Format(@"UPDATE {0} SET {1} WHERE {2}", TableName, updateStr, whereStr);

            return output;
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
                    Status = status,
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

        public ReturnObject Delete(string id)
        {
            try
            {
                if (Connection.State != ConnectionState.Open)
                    Connection.Open();

                var result = Connection.Delete(new BitcoinWithdrawTransaction {Id = id});
                var status = !string.IsNullOrEmpty(result.ToString()) ? Status.StatusSuccess : Status.StatusError;
                return new ReturnObject
                {
                    Status = status,
                    Message = status == Status.StatusError ? "Cannot insert" : "Insert Success"
                };
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public ReturnObject Insert(BitcoinWithdrawTransaction objectInsert)
        {
            try
            {
                if (Connection.State != ConnectionState.Open)
                    Connection.Open();


                var result = Connection.InsertTask<string, BitcoinWithdrawTransaction>(objectInsert);
                var status = !string.IsNullOrEmpty(result) ? Status.StatusSuccess : Status.StatusError;
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


        public BitcoinWithdrawTransaction FindById(string id)
        {
            try
            {
                if (Connection.State != ConnectionState.Open)
                    Connection.Open();

                var sQuery = "SELECT * FROM " + TableName + " WHERE Id = @ID";

                var result = Connection.QuerySingleOrDefault<BitcoinWithdrawTransaction>(sQuery, new {ID = id});


                return result;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public List<BitcoinWithdrawTransaction> FindWhere(BitcoinWithdrawTransaction rawtransaction)
        {
            try
            {
                if (Connection.State != ConnectionState.Open)
                    Connection.Open();


                var sQuery = "SELECT * FROM " + TableName + " WHERE 1 = 1";


                if (!string.IsNullOrEmpty(rawtransaction.Hash))
                    sQuery += " AND Hash" + "='" + rawtransaction.Hash + "'";

                if (!string.IsNullOrEmpty(rawtransaction.ToAddress))
                    sQuery += " AND ToAddress" + "='" + rawtransaction.ToAddress + "'";


                var result = Connection.Query<BitcoinWithdrawTransaction>(sQuery);
                return result.ToList();
            }
            catch (Exception e)
            {
                throw e;
            }
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

        public ReturnObject ExcuteSQL(string sqlString)
        {
            try
            {
                if (Connection.State != ConnectionState.Open)
                    Connection.Open();

                var result = Connection.Execute(sqlString);

                var status = result > 0 ? Status.StatusSuccess : Status.StatusError;

                return new ReturnObject
                {
                    Status = status,
                    Message = status == Status.StatusError ? "Cannot Excute" : "Excute Success",
                    Data = sqlString
                };
            }
            catch (Exception e)
            {
                return new ReturnObject
                {
                    Status = Status.StatusError,
                    Message = e.Message,
                    Data = sqlString
                };
            }
        }

        public IBlockchainTransaction FindTransactionPending()
        {
            try
            {
                return FindTransactionByStatus(Status.StatusPending);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public IBlockchainTransaction FindTransactionError()
        {
            try
            {
                return FindTransactionByStatus(Status.StatusError);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public IBlockchainTransaction FindTransactionByStatus(string status)
        {
            try
            {
                if (Connection.State != ConnectionState.Open)
                    Connection.Open();

                var sqlString = $"Select * from {TableName} where Status = @status and InProcess = 0";
                var result =
                    Connection.QueryFirstOrDefault<EthereumWithdrawTransaction>(sqlString, new {status = status});
                return result;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<ReturnObject> LockForProcess(IBlockchainTransaction transaction)
        {
            try
            {
                if (Connection.State != ConnectionState.Open)
                    Connection.Open();
                string SqlCommand = "Update " + TableName +
                                    " Set Version = Version + 1, InProcess = 1 Where Id = @Id and Version = @Version and InProcess = 0";

                var update = Connection.Execute(SqlCommand, new {Id = transaction.Id, Version = transaction.Version});
                if (update == 1)
                {
                    return new ReturnObject
                    {
                        Status = Status.StatusSuccess,
                        Message = "Update Success"
                    };
                }

                return new ReturnObject
                {
                    Status = Status.StatusError,
                    Message = "Update Fail"
                };
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<ReturnObject> ReleaseLock(IBlockchainTransaction transaction)
        {
            try
            {
                if (Connection.State != ConnectionState.Open)
                    Connection.Open();
                string SqlCommand = "Update " + TableName +
                                    " Set Version = Version + 1, InProcess = 0 Where Id = @Id and Version = @Version and InProcess = 1";

                var update = Connection.Execute(SqlCommand, new {Id = transaction.Id, Version = transaction.Version});
                if (update == 1)
                {
                    return new ReturnObject
                    {
                        Status = Status.StatusSuccess,
                        Message = "Update Success"
                    };
                }

                return new ReturnObject
                {
                    Status = Status.StatusError,
                    Message = "Update Fail"
                };
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<ReturnObject> SafeUpdate(IBlockchainTransaction transaction)
        {
            try
            {
                if (Connection.State != ConnectionState.Open)
                    Connection.Open();
                string SqlCommand = "Update " + TableName +
                                    " Set Version = Version + 1, InProcess = 0, Status = @Status, UpdatedAt = @UpdatedAt Where Id = @Id and Version = @Version and InProcess = 1";

                var update = Connection.Execute(SqlCommand,
                    new
                    {
                        Id = transaction.Id,
                        Version = transaction.Version,
                        Status = transaction.Status,
                        UpdatedAt = transaction.UpdatedAt
                    });
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
                    Message = "Update Fail"
                };
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}