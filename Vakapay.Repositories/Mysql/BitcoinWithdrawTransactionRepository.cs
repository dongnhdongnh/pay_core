using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Dapper;
using NLog;
using Vakapay.Commons.Constants;
using Vakapay.Models.Domains;
using Vakapay.Models.Entities;
using Vakapay.Models.Repositories;

namespace Vakapay.Repositories.Mysql
{
    public class BitcoinWithdrawTransactionRepository : BlockchainTransactionRepository<BitcoinWithdrawTransaction>,
        IBitcoinWithdrawTransactionRepository
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        private IBitcoinWithdrawTransactionRepository _bitcoinRawTransactionRepositoryImplementation;

        public BitcoinWithdrawTransactionRepository(string connectionString) : base(connectionString)
        {
        }

        public BitcoinWithdrawTransactionRepository(IDbConnection dbConnection) : base(dbConnection)
        {
        }


        public string QuerySearch(Dictionary<string, string> models)
        {
            var sQuery = "SELECT * FROM BitcoinWithdrawTransaction WHERE 1 = 1";
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


        public ReturnObject ExcuteSQL(string sqlString)
        {
            try
            {
                if (Connection.State != ConnectionState.Open)
                    Connection.Open();

                var result = Connection.Execute(sqlString);

                var status = result > 0 ? Status.STATUS_SUCCESS : Status.STATUS_ERROR;

                return new ReturnObject
                {
                    Status = status,
                    Message = status == Status.STATUS_ERROR ? "Cannot Excute" : "Excute Success",
                    Data = sqlString
                };
            }
            catch (Exception e)
            {
                return new ReturnObject
                {
                    Status = Status.STATUS_ERROR,
                    Message = e.Message,
                    Data = sqlString
                };
            }
        }
    }
}