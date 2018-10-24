using System;
using System.Collections.Generic;
using System.Threading;
using Vakapay.Commons.Helpers;
using Vakapay.Models.Repositories;
using Vakapay.Repositories.Mysql;

namespace Vakapay.PortfolioHistory
{
    class Program
    {
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
                    Console.WriteLine("Scanned UserId = "+userId + " at "+ CommonHelper.GetUnixTimestamp());
                    portfolioHistoryBusiness.InsertWithPrice(userId);
                }
                Thread.Sleep(minutes*60*1000);
            }
        }

        static void Main(string[] args)
        {
            var repositoryConfig = new RepositoryConfiguration
            {
                ConnectionString = AppSettingHelper.GetDBConnection(),
            };
            var persistenceFactory = new VakapayRepositoryMysqlPersistenceFactory(repositoryConfig);
            var walletBusiness = new WalletBusiness.WalletBusiness(persistenceFactory);
            var portfolioHistoryBusiness = new PortfolioHistoryBusiness.PortfolioHistoryBusiness(persistenceFactory);

            SavePortfolioHistoryEvery(DashboardConfig.INTERVAL, walletBusiness, portfolioHistoryBusiness);
        }
    }
}