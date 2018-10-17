using System;
using System.Threading;
using Vakapay.Commons.Helpers;
using Vakapay.Models.Repositories;
using Vakapay.Repositories.Mysql;

namespace Vakapay.ScanWallet
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            try
            {
                var repositoryConfig = new RepositoryConfiguration
                {
                    ConnectionString = AppSettingHelper.GetDBConnection()
                };

                for (var i = 0; i < 10; i++)
                {
                    var ts = new Thread(() => RunScan(repositoryConfig));
                    ts.Start();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private static void RunScan(RepositoryConfiguration repositoryConfig)
        {
            var repoFactory = new VakapayRepositoryMysqlPersistenceFactory(repositoryConfig);

            var business = new WalletBusiness.WalletBusiness(repoFactory);
            try
            {
                while (true)
                {
                    try
                    {
                        Console.WriteLine("Start Scan wallet...");
                        var resultSend = business.CreateAddressAsync();
                        Console.WriteLine(JsonHelper.SerializeObject(resultSend.Result));

                        Console.WriteLine("End Create Address...");
                        Thread.Sleep(100);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }
}