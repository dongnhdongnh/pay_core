using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using Vakapay.Models.Repositories;
using Vakapay.Repositories.Mysql;
using Vakapay.VakacoinBusiness;
using VakaSharp.Api.v1;

namespace Vakapay.ScanVakaCoin
{
    class Program
    {
        public static IConfiguration InitConfiguration()
        {
            string environment = Environment.GetEnvironmentVariable("NETCORE_ENVIRONMENT");

            if (String.IsNullOrWhiteSpace(environment))
                environment = "Development";
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true)
                .AddJsonFile($"appsettings.{environment}.json", optional: false);
            
            return builder.Build();
        }
        public static void Main(string[] args)
        {
            IConfiguration configuration = InitConfiguration();
            var repositoryConfig = new RepositoryConfiguration
            {
                ConnectionString = configuration["ConnectionStrings"],
            };
            VakapayRepositoryMysqlPersistenceFactory persistenceFactory = new VakapayRepositoryMysqlPersistenceFactory(repositoryConfig);

            VakacoinChainHelper helper = new VakacoinChainHelper(
                Int32.Parse(configuration["Chain:BlockInterval"]),
                new VakacoinRPC(configuration["Chain:URL"]),
                new VakacoinBusiness.VakacoinBusiness(persistenceFactory),
                new WalletBusiness.WalletBusiness(persistenceFactory),
                new SendMailBusiness.SendMailBusiness(persistenceFactory)
            );
            foreach (GetBlockResponse block in helper.StreamBlock())
            {
                helper.ParseTransaction(block);
            }
        }
    }
}