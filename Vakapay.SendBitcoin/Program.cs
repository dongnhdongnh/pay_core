using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using Vakapay.Models.Domains;
using Vakapay.Models.Repositories;
using Vakapay.Repositories.Mysql;
using NLog;

namespace Vakapay.SendBitcoin
{
    class Program
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        static void Main(string[] args)
        {
            try
            {
                var builder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("Configs.json");
                IConfiguration Configuration = builder.Build();

                var repositoryConfig = new RepositoryConfiguration
                {
                    ConnectionString =
                        Configuration.GetSection("ConnectionStrings").Value
                };

                var persistenceFactory = new VakapayRepositoryMysqlPersistenceFactory(repositoryConfig);

                var bitcoinConnect = new BitcoinRPCConnect
                {
                    Host = Configuration.GetSection("EndpointUrl").Value,
                    UserName = Configuration.GetSection("User").Value,
                    Password = Configuration.GetSection("Password").Value
                };
                var btcBusiness = new BitcoinBusiness.BitcoinBusiness(persistenceFactory, bitcoinConnect);

                while (true)
                {
                    btcBusiness.RunSendTransaction();
                    System.Threading.Thread.Sleep(1000);
                }
            }
            catch (Exception e)
            {
                logger.Error(e, "Sendbitcoin exception");
                throw;
            }
        }
    }
}