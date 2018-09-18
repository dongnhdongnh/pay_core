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
        private static Logger logger = LogManager.GetCurrentClassLogger();

        private static Timer _timer;
        private static BitcoinBusiness.BitcoinBusiness _btcBusiness;
        private const int TimeInterval = 1000;

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

            var bitcoinBusiness = new BitcoinBusinessNew(repoFactory);
            var connection = repoFactory.GetDbConnection();
            try
            {
                while (true)
                {
                    Console.WriteLine("Start Send Ethereum....");

                    var rpc = new BitcoinRpc(bitcoinConnect.Host, bitcoinConnect.UserName, bitcoinConnect.Password);

                    var ethereumRepo = repoFactory.GetBitcoinDepositTransactionRepository(connection);
                    var resultSend = bitcoinBusiness.SendTransactionAsync(ethereumRepo, rpc, "");
                    Console.WriteLine(JsonHelper.SerializeObject(resultSend.Result));

                    Console.WriteLine("Send Ethereum End...");
                    Thread.Sleep(1000);
                }
            }
            catch (Exception e)
            {
                connection.Close();
                Console.WriteLine(e.ToString());
            }
        }
    }
}