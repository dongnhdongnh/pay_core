using System;
using System.Collections.Generic;
using System.Data;
using Dapper;
using Vakapay.Models.Domains;
using Vakapay.Models.Entities;
using Vakapay.Models.Repositories;
using Vakapay.Repositories.Mysql.Base;

namespace Vakapay.Repositories.Mysql
{
    public class UserRepository : MySqlBaseRepository<User>, IUserRepository
    {
        private string TableNameWallet { get; }
        private string TableNameBitcoinAddress { get; }

        public UserRepository(string connectionString) : base(connectionString)
        {
            TableNameWallet = SimpleCRUD.GetTableName(typeof(Wallet));
            TableNameBitcoinAddress = SimpleCRUD.GetTableName(typeof(BitcoinAddress));
        }

        public UserRepository(IDbConnection dbConnection) : base(dbConnection)
        {
            TableNameWallet = SimpleCRUD.GetTableName(typeof(Wallet));
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
                var blockchainAddressTableName = "";
                if (transaction.GetType() == typeof(BitcoinWithdrawTransaction))
                {
                    blockchainAddressTableName = SimpleCRUD.GetTableName(typeof(BitcoinAddress));
                }
                else if (transaction.GetType() == typeof(EthereumWithdrawTransaction))
                {
                    blockchainAddressTableName = SimpleCRUD.GetTableName(typeof(EthereumAddress));
                }
                else if (transaction.GetType() == typeof(VakacoinWithdrawTransaction))
                {
                    blockchainAddressTableName = SimpleCRUD.GetTableName(typeof(VakacoinAccount));
                }
                else
                {
                    blockchainAddressTableName = "";
                }

                if (Connection.State != ConnectionState.Open)
                    Connection.Open();

                var sQuery = $"SELECT Email FROM {TableName} t1 INNER JOIN {TableNameWallet} t2 ON t1.Id = t2.UserId "
                             + $"INNER JOIN {blockchainAddressTableName} t3 ON t2.Id = t3.WalletId ";

                if (transaction.GetType() == typeof(VakacoinWithdrawTransaction))
                {
                    sQuery += $"WHERE t3.{nameof(VakacoinAccount.AccountName)} = @Address;";
                }
                else
                {
                    sQuery += $"WHERE t3.{nameof(BlockchainAddress.Address)} = @Address;";
                }


                var result = Connection.QueryFirstOrDefault<string>(sQuery, new {Address = transaction.FromAddress});
                Logger.Debug("UserRepository =>> FindEmailByAddressOfWallet result: " + result);
                return result;
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