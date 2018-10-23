using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Microsoft.Extensions.Configuration;
using Vakapay.Models.Repositories;
using Vakapay.Repositories.Mysql;

namespace Vakapay.PortfolioHistory
{
    class Program
    {
        private static IConfiguration InitConfiguration()
        {
            var environment = Environment.GetEnvironmentVariable("NETCORE_ENVIRONMENT");

            if (string.IsNullOrWhiteSpace(environment))
                environment = "Development";
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false)
                .AddJsonFile($"appsettings.{environment}.json", optional: true);

            return builder.Build();
        }

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
                    portfolioHistoryBusiness.InsertWithPrice(userId);
                }
                Thread.Sleep(minutes*60*1000);
            }
        }

        static void Main(string[] args)
        {
            var configuration = InitConfiguration();
            var repositoryConfig = new RepositoryConfiguration
            {
                ConnectionString = configuration["ConnectionStrings"],
            };
            var persistenceFactory = new VakapayRepositoryMysqlPersistenceFactory(repositoryConfig);
            var walletBusiness = new WalletBusiness.WalletBusiness(persistenceFactory);
            var portfolioHistoryBusiness = new PortfolioHistoryBusiness.PortfolioHistoryBusiness(persistenceFactory);

            SavePortfolioHistoryEvery(Int32.Parse(configuration["Interval"]), walletBusiness, portfolioHistoryBusiness);
        }
    }
}