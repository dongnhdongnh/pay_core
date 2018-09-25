using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper;
using NLog;
using Vakapay.Models.Domains;
using Vakapay.Models.Entities;
using Vakapay.Models.Repositories;
using Vakapay.Repositories.Mysql.Base;

namespace Vakapay.Repositories.Mysql
{
    public class UserRepository : MySqlBaseRepository<User>, IUserRepository
    {
        private const string TableNameWallet = "wallet";
        private const string TableNameBitcoinAddress = "bitcoinaddress";

        public UserRepository(string connectionString) : base(connectionString)
        {
        }

        public UserRepository(IDbConnection dbConnection) : base(dbConnection)
        {
        }


        public User FindById(string id)
        {
            try
            {
                if (Connection.State != ConnectionState.Open)
                    Connection.Open();

                string query = "SELECT * FROM user WHERE Id = @ID";
                var result = Connection.QuerySingle<User>(query, new {ID = id});

                return result;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public string QuerySearch(Dictionary<string, string> models)
        {
            var sQuery = "SELECT * FROM user WHERE 1 = 1";
            foreach (var model in models)
            {
                sQuery += string.Format(" AND {0}='{1}'", model.Key, model.Value);
            }

            return sQuery;
        }

        public User FindWhere(string sql)
        {
            try
            {
                if (Connection.State != ConnectionState.Open)
                    Connection.Open();

                var result = Connection.QuerySingle<User>(sql);

                return result;
            }
            catch (Exception e)
            {
                Logger.Error("UserRepository =>> FindWhere fail: " + e.Message);
                return null;
            }
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
        
        public string FindEmailByBitcoinAddress(string bitcoinAddress)
        {
            try
            {
                if (Connection.State != ConnectionState.Open)
                    Connection.Open();

                var sQuery = "SELECT Email FROM " + TableName +
                             " t1 INNER JOIN " + TableNameWallet + " t2 ON t1.Id = t2.UserId INNER JOIN " +
                             TableNameBitcoinAddress + " t3 ON t2.Id = t3.WalletId " +
                             "WHERE t3.Address = @BitcoinAddress;";


                var result = Connection.QueryFirstOrDefault<string>(sQuery, new {BitcoinAddress = bitcoinAddress});
                Logger.Error("UserRepository =>> FindEmailByAddressOfWallet result: " + result);
                return result;
            }
            catch (Exception e)
            {
                Logger.Error("UserRepository =>> FindEmailByAddressOfWallet fail: " + e.Message);
                return null;
            }
        }
    }
}