﻿using System;
using System.IO;
using System.Threading;
using Microsoft.Extensions.Configuration;
using Vakapay.Models.Domains;
using Vakapay.Models.Repositories;
using Vakapay.Repositories.Mysql;
using NLog;
using Vakapay.BitcoinBusiness;

namespace Vakapay.SendBitcoin
{
    internal static class Program
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private static void Main()
        {
            try
            {
                var environment = Environment.GetEnvironmentVariable("NETCORE_ENVIRONMENT");

                if (string.IsNullOrWhiteSpace(environment))
                    environment = "Development";

                var builder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", optional: true)
                    .AddJsonFile($"appsettings.{environment}.json", optional: false);
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
                    var ts = new Thread(() => RunSend(repositoryConfig, bitcoinConnect));
                    ts.Start();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private static void RunSend(RepositoryConfiguration repositoryConfig, BitcoinRPCConnect bitcoinConnect)
        {
            var repoFactory = new VakapayRepositoryMysqlPersistenceFactory(repositoryConfig);

            var bitcoinBusiness = new BitcoinBusiness.BitcoinBusiness(repoFactory);
            var connection = repoFactory.GetOldConnection() ?? repoFactory.GetDbConnection();
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
                Logger.Error(e, "Send Bitcoin");
                Console.WriteLine(e.ToString());
            }
        }
    }
}