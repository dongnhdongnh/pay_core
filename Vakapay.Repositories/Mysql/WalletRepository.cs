using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Text;
using Dapper;
using Vakapay.Models.Domains;
using Vakapay.Models.Entities;
using Vakapay.Models.Repositories;
using Vakapay.Repositories.Mysql.Base;

namespace Vakapay.Repositories.Mysql
{
    public class WalletRepository : MySqlBaseRepository<Wallet>, IWalletRepository
    {
        public WalletRepository(string connectionString) : base(connectionString)
        {
        }

        public WalletRepository(IDbConnection dbConnection) : base(dbConnection)
        {
        }

        public ReturnObject Update(Wallet objectUpdate)
        {
            try
            {
                if (Connection.State != ConnectionState.Open)
                    Connection.Open();
                var result = 0;
                result = Connection.Update<Wallet>(objectUpdate);
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

        public ReturnObject Delete(string Id)
        {
            throw new System.NotImplementedException();
        }

        public ReturnObject Insert(Wallet objectInsert)
        {
            try
            {
                if (Connection.State != ConnectionState.Open)
                    Connection.Open();
                var result = Connection.InsertTask<string, Wallet>(objectInsert);
                var status = !String.IsNullOrEmpty(result) ? Status.StatusSuccess : Status.StatusError;
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

        public Wallet FindById(string Id)
        {
            try
            {
                if (Connection.State != ConnectionState.Open)
                    Connection.Open();

                string sQuery = "SELECT * FROM wallet WHERE Id = @ID";
                var result = Connection.QuerySingle<Wallet>(sQuery, new {ID = Id});

                return result;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public List<Wallet> FindBySql(string sqlString)
        {
            try
            {
                if (Connection.State != ConnectionState.Open)
                    Connection.Open();
                var result = Connection.Query<Wallet>(sqlString);
                return result.ToList();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public ReturnObject UpdateBalanceWallet(decimal amount, string id, int version)
        {
            try
            {
                if (Connection.State != ConnectionState.Open)
                    Connection.Open();
                Int32 unixTimestamp = (Int32) DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
                string sQuery =
                    "UPDATE wallet SET Balance = Balance + @AMOUNT, Version = @VERSION + 1, UpdatedAt = @TIMESTAMP WHERE Id = @ID AND Version = @VERSION";

                var result = Connection.Query(sQuery,
                    new
                    {
                        ID = id,
                        VERSION = version,
                        AMOUNT = amount,
                        TIMESTAMP = unixTimestamp
                    });

                var status = !String.IsNullOrEmpty(result.ToString()) ? Status.StatusSuccess : Status.StatusError;
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

        public Wallet FindByAddress(string address)
        {
            try
            {
                string query = $"SELECT * FROM wallet WHERE Address = '{address}'";
                List<Wallet> wallets = FindBySql(query);
                if (wallets == null || wallets.Count == 0)
                    return null;
                return wallets[0];
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }
        
        public List<Wallet> FindByAddressAndNetworkName(string address, string networkName)
        {
            try
            {
                string query;
                if (address == null)
                {
                    query = $"SELECT * FROM wallet WHERE ISNULL(Address) AND NetworkName = '{networkName}'";
                }
                else
                {
                    query = $"SELECT * FROM wallet WHERE Address = '{address}' AND NetworkName = '{networkName}'";
                }
                List<Wallet> wallets = FindBySql(query);
                if (wallets == null || wallets.Count == 0)
                    return null;
                return wallets;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        public List<Wallet> queryResult(Dictionary<string, string> whereValue, string tableName)
        {
            try
            {
                StringBuilder whereStr = new StringBuilder("");
                int count = 0;
                foreach (var prop in whereValue)
                {
                    if (prop.Value != null)
                    {
                        if (count > 0)
                            whereStr.Append(" AND ");
                        whereStr.AppendFormat(" {0}='{1}'", prop.Key, prop.Value);
                        count++;
                    }
                }

                string output = string.Format("SELECT * FROM {0} WHERE {1}", tableName, whereStr);

                List<Wallet> wallets = FindBySql(output);
                if (wallets == null || wallets.Count == 0)
                    return null;
                return wallets;
            }
            catch (Exception e)
            {
                return null;
            }
        }
    }
}