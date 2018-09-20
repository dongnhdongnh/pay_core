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
                
                for(var i = 0; i< 10; i++)
                {
                    var ts = new Thread(()=>RunSend(repositoryConfig));
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

            var business = new VakacoinBusiness.VakacoinBusiness(repoFactory);
            var connection = repoFactory.GetDbConnection();
            try
            {
                while (true)
                {
                    Console.WriteLine("Start Send Vakacoin...");
                        
                    var rpc = new VakacoinRPC("http://127.0.0.1:8000");
                    
                    business.SetAccountRepositoryForRpc(rpc);

                    var repo = repoFactory.GetVakacoinWithdrawTransactionRepository(connection);
                    var resultSend = business.SendTransactionAsync(repo, rpc, "");
                    Console.WriteLine(JsonHelper.SerializeObject(resultSend.Result));
                    
                    Console.WriteLine("Send Vakacoin End...");
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