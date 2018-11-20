using System;
using System.Threading;
using Vakapay.Commons.Helpers;
using Vakapay.Models.Repositories;
using Vakapay.Repositories.Mysql;
using Vakapay.VakacoinBusiness;


namespace Vakapay.SendVakacoin
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            try
            {
                var nodeUrl = AppSettingHelper.GetVakacoinNode();
                var repositoryConfig = new RepositoryConfiguration
                {
                    ConnectionString = AppSettingHelper.GetDbConnection()
                };

                for (var i = 0; i < 20; i++)
                {
                    var ts = new Thread(() => RunSend(repositoryConfig, nodeUrl));
                    ts.Start();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private static void RunSend(RepositoryConfiguration repositoryConfig, string nodeUrl)
        {
            var repoFactory = new VakapayRepositoryMysqlPersistenceFactory(repositoryConfig);

            var business = new VakacoinBusiness.VakacoinBusiness(repoFactory);
            var connection = repoFactory.GetOldConnection() ?? repoFactory.GetDbConnection();

            if (nodeUrl == null)
            {
                Console.WriteLine("node url null");
                return;
            }

            try
            {
                while (true)
                {
                    try
                    {
                        var rpc = new VakacoinRpc(nodeUrl);

                        business.SetAccountRepositoryForRpc(rpc);

                        Console.WriteLine("Start Send Vakacoin...");
                        using (var repo = repoFactory.GetVakacoinWithdrawTransactionRepository(connection))
                        {
                            var resultSend = business.SendTransactionAsync(repo, rpc);
                            Console.WriteLine(JsonHelper.SerializeObject(resultSend.Result));

                            Console.WriteLine("Send Vakacoin End...");
                            Thread.Sleep(100);
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
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