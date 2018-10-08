using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using Vakapay.Models.Repositories;
using Vakapay.Repositories.Mysql;
using Vakapay.VakacoinBusiness;
using VakaSharp.Api.v1;

namespace Vakapay.ScanVakaCoin
{
    internal static class Program
    {
        private static IConfiguration InitConfiguration()
        {
            var environment = Environment.GetEnvironmentVariable("NETCORE_ENVIRONMENT");

            if (string.IsNullOrWhiteSpace(environment))
                environment = "Development";
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false)
                .AddJsonFile($"appsettings.{environment}.json", optional: true);
            
            return builder.Build();
        }
        public static void Main(string[] args)
        {
            var configuration = InitConfiguration();
            var repositoryConfig = new RepositoryConfiguration
            {
                ConnectionString = configuration["ConnectionStrings"],
            };
            var persistenceFactory = new VakapayRepositoryMysqlPersistenceFactory(repositoryConfig);

            var helper = new VakacoinChainHelper(
                int.Parse(configuration["Chain:BlockInterval"]),
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