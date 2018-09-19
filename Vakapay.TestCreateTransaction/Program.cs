﻿using System;
using Vakapay.Commons.Helpers;
using Vakapay.Models.Domains;
using Vakapay.Models.Entities;
using Vakapay.Models.Repositories;
using Vakapay.Repositories.Mysql;

namespace Vakapay.TestCreateTransaction
{
    class Program
    {
        static void Main(string[] args)
        {
            var repositoryConfig = new RepositoryConfiguration
            {
                ConnectionString =
                    "server=127.0.0.1;userid=root;password=Concuacang123!;database=vakapay;port=3306;Connection Timeout=120;SslMode=none"
            };


            var persistenceFactory = new VakapayRepositoryMysqlPersistenceFactory(repositoryConfig);
            var ethereumWithdrawTransactionRepository =
                persistenceFactory.GetEthereumWithdrawTransactionRepository(persistenceFactory.GetDbConnection());
            try
            {
                for (var i = 0; i < 20; i++)
                {
                    var trans = new EthereumWithdrawTransaction
                    {
                        Amount = 1,
                        CreatedAt = (int) CommonHelper.GetUnixTimestamp(),
                        Fee = 0,
                        BlockNumber = 0,
                        FromAddress = null,
                        Hash = null,
                        Id = CommonHelper.GenerateUuid(),
                        InProcess = 0,
                        NetworkName = "ETH",
                        Status = Status.StatusPending,
                        Version = 0,
                        UpdatedAt = (int)CommonHelper.GetUnixTimestamp(),
                        ToAddress = "0x13f022d72158410433cbd66f5dd8bf6d2d129924"

                    };
                    var resultInsert = ethereumWithdrawTransactionRepository.Insert(trans);
                    Console.WriteLine(JsonHelper.SerializeObject(resultInsert));

                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            
            
        }
    }
}