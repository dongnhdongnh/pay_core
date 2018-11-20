using System;
using System.Threading;
using Vakapay.Models.Repositories;
using Vakapay.Repositories.Mysql;
using NLog;
using Vakapay.BitcoinBusiness;
using Vakapay.Commons.Helpers;

namespace Vakapay.SendBitcoin
{
    internal static class Program
    {
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        private static void Main()
        {
            try
            {
                var repositoryConfig = new RepositoryConfiguration
                {
                    ConnectionString = AppSettingHelper.GetDbConnection()
                };

                for (var i = 0; i < 10; i++)
                {
                    var ts = new Thread(() => RunSend(repositoryConfig));
                    ts.Start();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private static void RunSend(RepositoryConfiguration repositoryConfig)
        {
            var repoFactory = new VakapayRepositoryMysqlPersistenceFactory(repositoryConfig);

            var bitcoinBusiness = new BitcoinBusiness.BitcoinBusiness(repoFactory);
            var connection = repoFactory.GetOldConnection() ?? repoFactory.GetDbConnection();
            try
            {
                while (true)
                {
                    Console.WriteLine("Start Send Bitcoin....");
                    var rpc = new BitcoinRpc(AppSettingHelper.GetBitcoinNode(),
                        AppSettingHelper.GetBitcoinRpcAuthentication());

                    using (var bitcoinRepo = repoFactory.GetBitcoinWithdrawTransactionRepository(connection))
                    {
                        var resultSend = bitcoinBusiness.SendTransactionAsync(bitcoinRepo, rpc);
                        Console.WriteLine(JsonHelper.SerializeObject(resultSend.Result));

                        Console.WriteLine("Send Bitcoin End...");
                        Thread.Sleep(1000);
                    }
                }
            }
            catch (Exception e)
            {
                connection.Close();
                _logger.Error(e, "Send Bitcoin");
                Console.WriteLine(e.ToString());
            }
        }
    }
}