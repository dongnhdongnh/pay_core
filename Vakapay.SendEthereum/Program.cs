using System;
using System.IO;
using System.Threading;
using Microsoft.Extensions.Configuration;
using Vakapay.EthereumBusiness;
using Vakapay.Models.Repositories;
using Vakapay.Repositories.Mysql;

namespace Vakapay.SendEthereum
{
    class Program
    {
        static void Main(string[] args)
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
                
            
                /*var connection = repoFactory.GetDbConnection();
                ///One thread
                while (true)
                {
                
                    Console.WriteLine("Start Send Ethereum....");
                    
                    var rpc = new EthereumRpc("http://localhost:8545");

                    var ethereumRepo = repoFactory.GetEthereumWithdrawTransactionRepository(connection);
                    var resultSend = ethereumBusiness.SendTransactionAsysn(ethereumRepo, rpc, "");
                    Console.WriteLine(JsonHelper.SerializeObject(resultSend.Result));
                
                    //connection.Close();
                    Console.WriteLine("Send Ethereum...");
                    Thread.Sleep(1000);
                }*/
                for(var i = 0; i< 10; i++)
                {
                    Thread ts = new Thread(()=>runSend(repositoryConfig));
                    ts.Start();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            
            



        }

        static void runSend(RepositoryConfiguration repositoryConfig)
        {
            var repoFactory = new VakapayRepositoryMysqlPersistenceFactory(repositoryConfig);

            var ethereumBusiness = new EthereumBusinessNew(repoFactory);
            var connection = repoFactory.GetDbConnection();
            try
            {
                while (true)
                {
                    Console.WriteLine("Start Send Ethereum....");
                        
                    var rpc = new EthereumRpc("http://localhost:8545");

                    var ethereumRepo = repoFactory.GetEthereumWithdrawTransactionRepository(connection);
                    var resultSend = ethereumBusiness.SendTransactionAsync(ethereumRepo, rpc, "");
                    Console.WriteLine(JsonHelper.SerializeObject(resultSend.Result));
                
                    
                    Console.WriteLine("Send Ethereum End...");
                    Thread.Sleep(1000);
                }
            }
            catch (Exception e)
            {
                connection.Close();
                Console.WriteLine("Send Ethereum error with message: ");
                Console.WriteLine(e.ToString());
            }
            
        }
    }
}
