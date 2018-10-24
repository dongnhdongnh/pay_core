using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper;
using Vakapay.Commons.Constants;
using Vakapay.Commons.Helpers;
using Vakapay.Models.Domains;
using Vakapay.Models.Entities;
using Vakapay.Models.Repositories;
using Vakapay.Repositories.Mysql.Base;

namespace Vakapay.Repositories.Mysql
{
    public class PortfolioHistoryRepository: MySqlBaseRepository<PortfolioHistory>, IPortfolioHistoryRepository

    {
        public PortfolioHistoryRepository(string connectionString) : base(connectionString)
        {
        }

        public PortfolioHistoryRepository(IDbConnection dbConnection) : base(dbConnection)
        {
        }

        public List<PortfolioHistory> FindByUserId(string userId, long from, long to)
        {
            var query = $"SELECT * FROM {TableName} WHERE UserId = '{userId}' AND Timestamp > {from} AND Timestamp < {to}";
            try
            {
                if (Connection.State != ConnectionState.Open)
                    Connection.Open();
                var result = Connection.Query<PortfolioHistory>(query);
                return result.ToList();
            }
            catch (Exception e)
            {
                Logger.Error("PortfolioHistoryRepository error when find portfolio ", e);
                return null;
            }
        }

        public ReturnObject InsertWithPrice(string userId, string vkcPrice, string btcPrice, string ethPrice, string eosPrice)
        {
            string queryFromWallet = $"SELECT * FROM Wallet WHERE UserId = '{userId}'";
            List<Wallet> wallets = new List<Wallet>();
            decimal vkcAmount = 0;
            decimal vkcValue = 0;

            decimal btcAmount = 0;
            decimal btcValue = 0;

            decimal ethAmount = 0;
            decimal ethValue = 0;

            decimal eosAmount = 0;
            decimal eosValue = 0;
            try
            {
                if (Connection.State != ConnectionState.Open)
                    Connection.Open();
                var result = Connection.Query<Wallet>(queryFromWallet);
                wallets = result.ToList();
            }
            catch (Exception e)
            {
                Logger.Error("PortfolioHistoryRepository error when queryFromWallet ", e);
                return new ReturnObject
                {
                    Status = Status.STATUS_ERROR,
                    Message = "PortfolioHistoryRepository error when queryFromWallet, "+e
                };
            }

            foreach (var wallet in wallets)
            {
                switch (wallet.Currency)
                {
                        case "Vakacoin":
                            vkcAmount = wallet.Balance;
                            vkcValue = Convert.ToDecimal(vkcPrice) * vkcAmount;
                            break;
                        case "Bitcoin":
                            btcAmount = wallet.Balance;
                            btcValue = Convert.ToDecimal(btcPrice) * btcAmount;
                            break;
                        case "Ethereum":
                            ethAmount = wallet.Balance;
                            ethValue = Convert.ToDecimal(ethPrice) * ethAmount;
                            break;
                        case "Eosio": case "Eos":
                            eosAmount = wallet.Balance;
                            eosValue = Convert.ToDecimal(eosPrice) * eosAmount;
                            break;
                }
            }

            var portfolioHistory = new PortfolioHistory
            {
                Id = CommonHelper.GenerateUuid(),
                UserId = userId,
                VakacoinAmount = vkcAmount,
                VakacoinValue = vkcValue,
                BitcoinAmount = btcAmount,
                BitcoinValue = btcValue,
                EthereumAmount = ethAmount,
                EthereumValue = ethValue,
                EosAmount = eosAmount,
                EosValue = eosValue,
                Timestamp = CommonHelper.GetUnixTimestamp()
            };
            return Insert(portfolioHistory);
        }
    }
}