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
    public class PortfolioHistoryRepository : MySqlBaseRepository<PortfolioHistory>, IPortfolioHistoryRepository

    {
        public PortfolioHistoryRepository(string connectionString) : base(connectionString)
        {
        }

        public PortfolioHistoryRepository(IDbConnection dbConnection) : base(dbConnection)
        {
        }

        public List<PortfolioHistory> FindByUserId(string userId, long from, long to)
        {
            var query = 
                $"SELECT * FROM {TableName} WHERE UserId = '{userId}' AND Timestamp > {from} AND Timestamp < {to} ORDER BY Timestamp ASC";
            try
            {
                if (Connection.State != ConnectionState.Open)
                    Connection.Open();
                var result = Connection.Query<PortfolioHistory>(query);
                return result.ToList();
            }
            catch (Exception e)
            {
                Logger.Error(e, "PortfolioHistoryRepository error when find portfolio ");
                return null;
            }
        }

        public ReturnObject InsertWithPrice(string userId, string vkcPrice, string btcPrice, string ethPrice)
        {
            string queryFromWallet = $"SELECT * FROM Wallet WHERE UserId = '{userId}'";
            List<Wallet> wallets = new List<Wallet>();
            decimal vkcAmount = 0;
            decimal vkcValue = 0;

            decimal btcAmount = 0;
            decimal btcValue = 0;

            decimal ethAmount = 0;
            decimal ethValue = 0;
            try
            {
                if (Connection.State != ConnectionState.Open)
                    Connection.Open();
                var result = Connection.Query<Wallet>(queryFromWallet);
                wallets = result.ToList();
            }
            catch (Exception e)
            {
                Logger.Error(e, "PortfolioHistoryRepository error when queryFromWallet ");
                return new ReturnObject
                {
                    Status = Status.STATUS_ERROR,
                    Message = "PortfolioHistoryRepository error when queryFromWallet, " + e
                };
            }

            foreach (var wallet in wallets)
            {
                switch (wallet.Currency)
                {
                    case CryptoCurrency.VAKA:
                        vkcAmount = wallet.Balance;
                        vkcValue = Convert.ToDecimal(vkcPrice) * vkcAmount;
                        break;
                    case CryptoCurrency.BTC:
                        btcAmount = wallet.Balance;
                        btcValue = Convert.ToDecimal(btcPrice) * btcAmount;
                        break;
                    case CryptoCurrency.ETH:
                        ethAmount = wallet.Balance;
                        ethValue = Convert.ToDecimal(ethPrice) * ethAmount;
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
                Timestamp = CommonHelper.GetUnixTimestamp()
            };
            return Insert(portfolioHistory);
        }
    }
}