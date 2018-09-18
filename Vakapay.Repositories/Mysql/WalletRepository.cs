using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security;
using Dapper;
using Vakapay.Models.Domains;
using Vakapay.Models.Entities;
using Vakapay.Models.Repositories;
using Vakapay.Repositories.Mysql.Base;

namespace Vakapay.Repositories.Mysql
{
    public class WalletRepository : MysqlBaseConnection, IWalletRepository
    {
        public WalletRepository(string connectionString) : base(connectionString)
        {
        }

        public WalletRepository(IDbConnection dbConnection) : base(dbConnection)
        {
        }

        public ReturnObject Update(Wallet objectUpdate)
        {
            throw new System.NotImplementedException();
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
                var result = Connection.QuerySingleOrDefault<Wallet>(sQuery, new {ID = Id});

                return result;
            }
            catch (Exception e)
            {
                throw e;
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

        public ReturnObject UpdateBalanceWallet(decimal amount, string Id, int version)
        {
            try
            {
                if (Connection.State != ConnectionState.Open)
                    Connection.Open();
                string sQuery = "UPDATE wallet SET Balance = Balance + @AMOUNT WHERE Id = @ID AND Version = @VERSION";

                var result = Connection.Query(sQuery, new {ID = Id, VERSION = version, AMOUNT = amount});

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

        public ReturnObject UpdateBalanceWallet(decimal amount, string id, int version, string addr, string networkName)
        {
            try
            {
                if (Connection.State != ConnectionState.Open)
                    Connection.Open();
                Int32 unixTimestamp = (Int32) (DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
                string sQuery =
                    "UPDATE wallet SET Balance = Balance + @AMOUNT, Version = @VERSION + 1, UpdateAt = @TIMESTAMP WHERE Id = @ID AND Version = @VERSION AND Address = @ADDRESS AND NetworkName = @NETWORKNAME";

                var result = Connection.Query(sQuery,
                    new
                    {
                        ID = id,
                        VERSION = version,
                        AMOUNT = amount,
                        ADDRESS = addr,
                        NETWORKNAME = networkName,
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

        public Wallet FindByAddress(string address, string networkName)
        {
            try
            {
                string query = $"SELECT * FROM wallet WHERE Address = '{address}' AND NetworkName = '{networkName}'";
                List<Wallet> wallets = FindBySql(query);
                if (wallets == null || wallets.Count == 0)
                    return null;
                return wallets[0];
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw e;
            }
        }
    }
}