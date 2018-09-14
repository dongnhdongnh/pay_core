using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices.ComTypes;
using Hazelcast.Client;
using Hazelcast.Config;
using Hazelcast.Core;
using Microsoft.Extensions.Configuration;
using Vakapay.Models.Entities;
using Vakapay.Models.Repositories;
using Vakapay.Repositories.Mysql;
using Vakapay.VakacoinBusiness;

namespace Vakapay.ScanVakaCoin
{
    class Program
    {
        static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("Configs.json");
            IConfiguration Configuration = builder.Build();
            
            Console.WriteLine("Configuration = " + Configuration.GetSection("ConnectionStrings").Value);
            
            var repositoryConfig = new RepositoryConfiguration
            {
                ConnectionString = Configuration.GetSection("ConnectionStrings").Value
            };
            var PersistenceFactory = new VakapayRepositoryMysqlPersistenceFactory(repositoryConfig);
            var vakacoinBusiness = new VakacoinBusiness.VakacoinBusiness(PersistenceFactory);

            VakacoinRpc rpc = new VakacoinRpc(Configuration.GetSection("EndpointUrl").Value);

            ReceivedScan rcScan = new ReceivedScan();
            rcScan.ReceivedProcess(vakacoinBusiness, rpc.GetAllTransactionsInBlock("16124751"));
        }
    }
}