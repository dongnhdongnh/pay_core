﻿using System;
using System.IO;
using System.Threading;
using Microsoft.Extensions.Configuration;
using Vakapay.Models.Entities;
using Vakapay.Models.Repositories;
using Vakapay.Repositories.Mysql;

namespace Vakapay.SendEmail
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
                .AddJsonFile("Configs.json", optional: true)
                .AddJsonFile($"Configs.{environment}.json", optional: false);
            
            return builder.Build();
        }

        private static void Main(string[] args)
        {
            var configuration = InitConfiguration();
            var apiKey = configuration["Elastic:api"];
            var from = configuration["Elastic:email"];
            var fromName = configuration["fromName"];
            var apiAddress = configuration["apiAddress"];

            var repositoryConfig = new RepositoryConfiguration
            {
                ConnectionString = configuration["ConnectionStrings"]
            };
            
            var persistenceFactory = new VakapayRepositoryMysqlPersistenceFactory(repositoryConfig);
            var sendMailBusiness = new SendMailBusiness.SendMailBusiness(persistenceFactory);

            while (true)
            {
                try
                {
                    var result = sendMailBusiness.SendEmailAsync(apiKey, from, fromName, apiAddress);
                    Console.WriteLine(JsonHelper.SerializeObject(result.Result));
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
                Thread.Sleep(1000);
            }
        }
    }
}