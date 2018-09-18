using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using NLog;
using Vakapay.Models.Domains;
using Vakapay.Models.Repositories;
using Vakapay.Repositories.Mysql;
using Vakapay.VakacoinBusiness;
using VakaSharp.Api.v1;

namespace Vakapay.ScanVakaCoin
{
    class Program
    {
        public static void Main(string[] args)
        {
            string environment = Environment.GetEnvironmentVariable("NETCORE_ENVIRONMENT");

            if (String.IsNullOrWhiteSpace(environment))
                environment = "Development";
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true)
                .AddJsonFile($"appsettings.{environment}.json", optional: false);
            
            IConfiguration configuration = builder.Build();
            var repositoryConfig = new RepositoryConfiguration
            {
                ConnectionString = configuration["ConnectionStrings"],
            };
            VakapayRepositoryMysqlPersistenceFactory persistenceFactory = new VakapayRepositoryMysqlPersistenceFactory(repositoryConfig);

            VakacoinBusiness.VakacoinBusiness vakacoinBusiness = new VakacoinBusiness.VakacoinBusiness(persistenceFactory);
            WalletBusiness.WalletBusiness walletBusiness = new WalletBusiness.WalletBusiness(persistenceFactory);

            VakacoinChainHelper helper = new VakacoinChainHelper(Int32.Parse(configuration["Chain:BlockInterval"]));
            helper.RpcClient = new VakacoinRPC(configuration["Chain:URL"]);
            foreach (GetBlockResponse block in helper.StreamBlock())
            {
                helper.ParseTransaction(block);
            }
        }
    }
}