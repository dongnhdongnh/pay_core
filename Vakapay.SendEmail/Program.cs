using System;
using System.IO;
using System.Threading;
using Microsoft.Extensions.Configuration;
using Vakapay.Models.Entities;
using Vakapay.Models.Repositories;
using Vakapay.Repositories.Mysql;

namespace Vakapay.SendEmail
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
                .AddJsonFile("Configs.json", optional: true)
                .AddJsonFile($"Configs.{environment}.json", optional: false);
            
            return builder.Build();
        }
        
        static void Main(string[] args)
        {
            IConfiguration configuration = InitConfiguration();
            string apikey = configuration["Elastic:api"];
            string from = configuration["Elastic:email"];
            string fromName = configuration["fromName"];
            string apiAddress = configuration["apiAddress"];

            var repositoryConfig = new RepositoryConfiguration
            {
                ConnectionString = configuration["ConnectionStrings"]
            };
            
            VakapayRepositoryMysqlPersistenceFactory persistenceFactory = new VakapayRepositoryMysqlPersistenceFactory(repositoryConfig);
            var sendMailBusiness = new SendMailBusiness.SendMailBusiness(persistenceFactory);

            while (true)
            {
                try
                {
                    var result = sendMailBusiness.SendEmailAsync(apikey, from, fromName, apiAddress);
                    Console.WriteLine(JsonHelper.SerializeObject(result));
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
                Thread.Sleep(1000);
            }
            

            //scan email EmailQueue
//            var send = new SendEmail();
//            var model = new EmailQueue();
//            send.send(model);
        }
    }
}