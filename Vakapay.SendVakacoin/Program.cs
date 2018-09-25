using System;
using System.IO;
using System.Threading;
using Microsoft.Extensions.Configuration;
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
                var builder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("setting.json");
                IConfiguration configuration = builder.Build();

                var connectionString = configuration.GetConnectionString("DefaultConnection");
                var repositoryConfig = new RepositoryConfiguration
                {
                    ConnectionString = connectionString
                };

                var nodeUrl = configuration.GetSection("Node")?["Url"];
                for(var i = 0; i < 10; i++)
                {
                    var ts = new Thread(()=>RunSend(repositoryConfig, nodeUrl));
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
            try
            {
                while (true)
                {
                    try
                    {
                        var rpc = new VakacoinRPC(nodeUrl);
                    
                        business.SetAccountRepositoryForRpc(rpc);

                        Console.WriteLine("Start Send Vakacoin...");
                        var repo = repoFactory.GetVakacoinWithdrawTransactionRepository(connection);
                        var resultSend = business.SendTransactionAsync(repo, rpc, "");
                        Console.WriteLine(JsonHelper.SerializeObject(resultSend.Result));
                        
                        Console.WriteLine("Send Vakacoin End...");
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
                connection.Close();
                Console.WriteLine(e.ToString());
            }
        }
    }
}