using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Vakapay.Models.Domains;
using Vakapay.Models.Entities;
using Vakapay.Models.Repositories;
using Vakapay.Repositories.Mysql.Base;

namespace Vakapay.Repositories.Mysql
{
	
    public class EthereumWithdrawnTransactionRepository : MysqlBaseConnection, IEthereumWithdrawTransactionRepository
    {
        public static string tableName = "ethereumwithdrawtransaction";
        public string Query_Search(string SearchName, string SearchData)
        {
            return String.Format("SELECT * FROM {2} Where {0}='{1}'", SearchName, SearchData, tableName);
        }

        public string Query_Update(object updatevalues, object selectorvalue)
        {
            StringBuilder updateStr = new StringBuilder("");
            StringBuilder whereStr = new StringBuilder("");

            int count = 0;
            foreach (PropertyInfo prop in updatevalues.GetType().GetProperties())
            {
                if (prop.GetValue(updatevalues, null) != null)
                {
                    if (count > 0)
                        updateStr.Append(",");
                    updateStr.AppendFormat(" {0}='{1}'", prop.Name, prop.GetValue(updatevalues, null));
                    count++;
                }
            }

            // if (whereStr != null)
            count = 0;
            foreach (PropertyInfo prop in selectorvalue.GetType().GetProperties())
            {
                if (prop.GetValue(selectorvalue, null) != null)
                {
                    if (count > 0)
                        whereStr.Append(" AND ");
                    whereStr.AppendFormat(" {0}='{1}'", prop.Name, prop.GetValue(selectorvalue, null));
                    count++;
                }
            }


            string output = string.Format(@"UPDATE {0} SET {1} WHERE {2}", tableName, updateStr, whereStr);
            Console.WriteLine(output);
            return output;
        }

        public EthereumWithdrawnTransactionRepository(string connectionString) : base(connectionString)
        {
        }

        public EthereumWithdrawnTransactionRepository(IDbConnection dbConnection) : base(dbConnection)
        {
        }

        public ReturnObject Delete(string Id)
        {
            throw new NotImplementedException();
        }

        public EthereumWithdrawTransaction FindById(string Id)
        {
            throw new NotImplementedException();
        }

        public List<EthereumWithdrawTransaction> FindBySql(string sqlString)
        {
            try
            {
                if (Connection.State != ConnectionState.Open)
                    Connection.Open();
                var result = Connection.Query<EthereumWithdrawTransaction>(sqlString).ToList();


                return result;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public ReturnObject Insert(EthereumWithdrawTransaction objectInsert)
        {
            try
            {
                if (Connection.State != ConnectionState.Open)
                    Connection.Open();
                var result = Connection.InsertTask<string, EthereumWithdrawTransaction>(objectInsert);
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

        public ReturnObject Update(EthereumWithdrawTransaction objectUpdate)
        {
            try
            {
                if (Connection.State != ConnectionState.Open)
                    Connection.Open();
                var result = Connection.Update<EthereumWithdrawTransaction>(objectUpdate);
                var status = result > 0 ? Status.StatusSuccess : Status.StatusError;
                return new ReturnObject
                {
                    Status = status,
                    Message = status == Status.StatusError ? "Cannot update" : "Update Success"
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

        public ReturnObject ExcuteSQL(string sqlString)
        {
            try
            {
                if (Connection.State != ConnectionState.Open)
                    Connection.Open();

                var result = Connection.Execute(sqlString);


                var status = result > 0 ? Status.StatusSuccess : Status.StatusError;
                Console.WriteLine("Excute thing " + result);
                return new ReturnObject
                {
                    Status = status,
                    Message = status == Status.StatusError ? "Cannot Excute" : "Excute Success"
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
