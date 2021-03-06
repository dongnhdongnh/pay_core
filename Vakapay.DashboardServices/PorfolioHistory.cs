﻿using System.Collections.Generic;
using System.Threading;
using NLog;
using Vakapay.Commons.Constants;
using Vakapay.Commons.Helpers;
using Vakapay.Models.Repositories;
using Vakapay.Repositories.Mysql;

namespace Vakapay.DashboardServices
{
    public class PorfolioHistory
    {
        private static Logger _logger = LogManager.GetCurrentClassLogger();
        
        private static List<string> GetDistinctUserId(WalletBusiness.WalletBusiness walletBusiness)
        {
            return walletBusiness.DistinctUserId();
        }

        private static void SavePortfolioHistoryEvery(int minutes, WalletBusiness.WalletBusiness walletBusiness,
            PortfolioHistoryBusiness.PortfolioHistoryBusiness portfolioHistoryBusiness)
        {
            while (true)
            {
                var lsUserId = GetDistinctUserId(walletBusiness);
                foreach (var userId in lsUserId)
                {
                    _logger.Info("Scanned UserId = " + userId + " at " + CommonHelper.GetUnixTimestamp());
                    portfolioHistoryBusiness.InsertWithPrice(userId);
                }

                Thread.Sleep(minutes * 60 * 1000);
            }
        }

        public static void RunPortfolioHistory()
        {
            var repositoryConfig = new RepositoryConfiguration
            {
                ConnectionString = AppSettingHelper.GetDbConnection()
            };
            var persistenceFactory = new VakapayRepositoryMysqlPersistenceFactory(repositoryConfig);
            var walletBusiness = new WalletBusiness.WalletBusiness(persistenceFactory);
            var portfolioHistoryBusiness = new PortfolioHistoryBusiness.PortfolioHistoryBusiness(persistenceFactory);

            SavePortfolioHistoryEvery(DashboardConfig.INTERVAL, walletBusiness, portfolioHistoryBusiness);
        }
    }
}