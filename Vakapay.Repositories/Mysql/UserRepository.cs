using System;
using System.Collections.Generic;
using System.Data;
using Dapper;
using Vakapay.Models.Domains;
using Vakapay.Models.Entities;
using Vakapay.Models.Entities.BTC;
using Vakapay.Models.Entities.ETH;
using Vakapay.Models.Entities.VAKA;
using Vakapay.Models.Repositories;
using Vakapay.Repositories.Mysql.Base;

namespace Vakapay.Repositories.Mysql
{
    public class UserRepository : MySqlBaseRepository<User>, IUserRepository
    {
        private string WalletTableName { get; }
        private string TableNameBitcoinAddress { get; }

        public UserRepository(string connectionString) : base(connectionString)
        {
            WalletTableName = SimpleCRUD.GetTableName(typeof(Wallet));
            TableNameBitcoinAddress = SimpleCRUD.GetTableName(typeof(BitcoinAddress));
        }

        public UserRepository(IDbConnection dbConnection) : base(dbConnection)
        {
            WalletTableName = SimpleCRUD.GetTableName(typeof(Wallet));
            TableNameBitcoinAddress = SimpleCRUD.GetTableName(typeof(BitcoinAddress));
        }


        public string QuerySearch(Dictionary<string, string> models)
        {
            var sQuery = "SELECT * FROM " + TableName + " WHERE 1 = 1";
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

                var result = Connection.QueryFirstOrDefault<User>(sql);

                return result;
            }
            catch (Exception e)
            {
                Logger.Error("UserRepository =>> FindWhere fail: " + e.Message);
                return null;
            }
        }

        public string FindEmailBySendTransaction(BlockchainTransaction transaction)
        {
            try
            {
                //                var blockchainAddressTableName = "";
                //                if (transaction.GetType() == typeof(BitcoinWithdrawTransaction))
                //                {
                //                    blockchainAddressTableName = SimpleCRUD.GetTableName(typeof(BitcoinAddress));
                //                }
                //                else if (transaction.GetType() == typeof(EthereumWithdrawTransaction))
                //                {
                //                    blockchainAddressTableName = SimpleCRUD.GetTableName(typeof(EthereumAddress));
                //                }
                //                else if (transaction.GetType() == typeof(VakacoinWithdrawTransaction))
                //                {
                //                    blockchainAddressTableName = SimpleCRUD.GetTableName(typeof(VakacoinAccount));
                //                }
                //                else
                //                {
                //                    blockchainAddressTableName = "";
                //                }
                //
                //                if (Connection.State != ConnectionState.Open)
                //                    Connection.Open();
                //
                //                var sQuery = $"SELECT Email FROM {TableName} t1 INNER JOIN {WalletTableName} t2 ON t1.Id = t2.UserId "
                //                             + $"INNER JOIN {blockchainAddressTableName} t3 ON t2.Id = t3.WalletId "
                //                             + $"WHERE t3.{nameof(BlockchainAddress.Address)} = @Address;";
                //
                //                var result = Connection.QueryFirstOrDefault<string>(sQuery, new {Address = transaction.FromAddress});
                //                Logger.Debug("UserRepository =>> FindEmailByAddressOfWallet result: " + result);
                //                return result;

                var user = FindById(transaction.UserId);

                return user.Email;
            }
            catch (Exception e)
            {
                Logger.Error("UserRepository =>> FindEmailByAddressOfWallet fail: " + e.Message);
                return null;
            }
        }

        public string FindEmailByBitcoinAddress(string bitcoinAddress)
        {
            try
            {
                if (Connection.State != ConnectionState.Open)
                    Connection.Open();

                var sQuery = "SELECT Email FROM " + TableName +
                             " t1 INNER JOIN " + WalletTableName + " t2 ON t1.Id = t2.UserId INNER JOIN " +
                             TableNameBitcoinAddress + " t3 ON t2.Id = t3.WalletId " +
                             "WHERE t3.Address = @BitcoinAddress;";


                var result = Connection.QueryFirstOrDefault<string>(sQuery, new { BitcoinAddress = bitcoinAddress });
                Logger.Error("UserRepository =>> FindEmailByAddressOfWallet result: " + result);
                return result;
            }
            catch (Exception e)
            {
                Logger.Error("UserRepository =>> FindEmailByAddressOfWallet fail: " + e.Message);
                return null;
            }
        }

        public User FindByEmailAddress(string emailAddress)
        {
            try
            {
                if (Connection.State != ConnectionState.Open)
                    Connection.Open();

                var sQuery = $"SELECT * FROM {TableName} WHERE {nameof(User.Email)} = @Email";

                var result = Connection.QuerySingleOrDefault<User>(sQuery, new { Email = emailAddress });

                return result;
            }
            catch (Exception e)
            {
                Logger.Error("UserRepository =>> FindByEmailAddress fail: " + e.Message);
                throw;
            }
        }

        public List<User> FindAllUser()
        {
            try
            {
                if (Connection.State != ConnectionState.Open)
                    Connection.Open();

                var sQuery = $"SELECT * FROM {TableName} ";

                var result = Connection.Query<User>(sQuery);

                return (List<User>)result;
            }
            catch (Exception e)
            {
                Logger.Error("UserRepository =>> FindByEmailAddress fail: " + e.Message);
                throw;
            }
        }
    }
}