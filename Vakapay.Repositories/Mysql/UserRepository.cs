using System;
using System.Collections.Generic;
using System.Data;
using Dapper;
using NLog;
using Vakapay.Models.Domains;
using Vakapay.Models.Entities;
using Vakapay.Models.Repositories;
using Vakapay.Repositories.Mysql.Base;

namespace Vakapay.Repositories.Mysql
{
    public class UserRepository : MysqlBaseConnection, IUserRepository
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private const string TableName = "user";
        private const string TableNameWallet = "wallet";
        private const string TableNameBitcoinAddress = "bitcoinaddress";

        public UserRepository(string connectionString) : base(connectionString)
        {
        }

        public UserRepository(IDbConnection dbConnection) : base(dbConnection)
        {
        }

        public ReturnObject Update(User objectUpdate)
        {
            throw new NotImplementedException();
        }

        public ReturnObject Delete(string id)
        {
            throw new NotImplementedException();
        }

        public ReturnObject Insert(User objectInsert)
        {
            throw new NotImplementedException();
        }

        public User FindById(string id)
        {
            return new User();
        }

        public List<User> FindBySql(string sqlString)
        {
            throw new NotImplementedException();
        }

        public string FindEmailByAddressOfWallet(string addressOfWallet)
        {
            try
            {
                if (Connection.State != ConnectionState.Open)
                    Connection.Open();

                var sQuery = "SELECT Email FROM " + TableName +
                             " t1 INNER JOIN " + TableNameWallet + " t2 ON t1.Id = t2.UserId INNER JOIN " +
                             TableNameBitcoinAddress + " t3 ON t2.Id = t3.WalletId " +
                             "WHERE t3.Address = @AddressOfWallet;";


                var result = Connection.QueryFirstOrDefault<string>(sQuery, new {AddressOfWallet = addressOfWallet});
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