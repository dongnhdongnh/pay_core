using System;
using System.IO;
using System.Threading;
using System.Timers;
using Microsoft.Extensions.Configuration;
using Vakapay.Models.Domains;
using Vakapay.Models.Repositories;
using Vakapay.Repositories.Mysql;
using NLog;
using Vakapay.BitcoinBusiness;

namespace Vakapay.SendBitcoin
{
    class Program
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        static void Main()
        {
            try
            {
                var builder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("setting.json");
                IConfiguration Configuration = builder.Build();

                var connectionString = Configuration.GetConnectionString("DefaultConnection");
                var repositoryConfig = new RepositoryConfiguration
                {
                    ConnectionString = connectionString
                };

                var bitcoinConnect = new BitcoinRPCConnect
                {
                    Host = Configuration.GetSection("EndpointUrl").Value,
                    UserName = Configuration.GetSection("User").Value,
                    Password = Configuration.GetSection("Password").Value
                };

                for (var i = 0; i < 10; i++)
                {
                    Thread ts = new Thread(() => runSend(repositoryConfig, bitcoinConnect));
                    ts.Start();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        static void runSend(RepositoryConfiguration repositoryConfig, BitcoinRPCConnect bitcoinConnect)
        {
            var repoFactory = new VakapayRepositoryMysqlPersistenceFactory(repositoryConfig);

            var bitcoinBusiness = new BitcoinBusiness.BitcoinBusiness(repoFactory);
            var connection = repoFactory.GetDbConnection();
            try
            {
                while (true)
                {
                    Console.WriteLine("Start Send Bitcoin....");
                    var rpc = new BitcoinRpc(bitcoinConnect.Host, bitcoinConnect.UserName, bitcoinConnect.Password);

                    var bitcoinRepo = repoFactory.GetBitcoinDepositTransactionRepository(connection);
                    var resultSend = bitcoinBusiness.SendTransactionAsync(bitcoinRepo, rpc, "");
                    Console.WriteLine(JsonHelper.SerializeObject(resultSend.Result));

                    Console.WriteLine("Send Bitcoin End...");
                    Thread.Sleep(1000);
                }
            }
            catch (Exception e)
            {
                logger.Error(e, "Send Bitcoin");
                Console.WriteLine(e.ToString());
            }
        }
    }
}