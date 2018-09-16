using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using Vakapay.Models.Domains;
using Vakapay.Models.Entities;
using Vakapay.Models.Repositories;
using Vakapay.Repositories.Mysql.Base;

namespace Vakapay.Repositories.Mysql
{
    public class EthereumWithdrawnTransactionRepository : MysqlBaseConnection, IEthereumWithdrawTransactionRepository
    {
        String tableName = "vakapay.ethereumwithdrawtransaction";
        public string Query_Search(string SearchName, string SearchData)
        {
            return String.Format("SELECT * FROM {3} Where {0}='{1}'", SearchName, SearchData, tableName);
        }

        public string Query_Update(object updatevalues, object selectorvalue)
        {
            StringBuilder updateStr = new StringBuilder("");
            StringBuilder whereStr = new StringBuilder("");

            foreach (PropertyInfo prop in updatevalues.GetType().GetProperties())
            {
                if (prop.GetValue(updatevalues, null) != null)
                    updateStr.AppendFormat(" %s=%s", prop.Name, prop.GetValue(updatevalues, null));
            }

            foreach (PropertyInfo prop in selectorvalue.GetType().GetProperties())
            {
                if (prop.GetValue(selectorvalue, null) != null)
                    updateStr.AppendFormat(" %s=%s", prop.Name, prop.GetValue(selectorvalue, null));
            }


            return string.Format(@"UPDATE %s SET %s WHERE %s", tableName, updateStr, selectorvalue);

            // return sqlStmt;
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


    }
}
