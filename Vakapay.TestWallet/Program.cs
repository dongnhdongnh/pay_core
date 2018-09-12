﻿using System;
using Vakapay.Commons.Helpers;
using Vakapay.Models.Domains;
using Vakapay.Models.Entities;
using Vakapay.Models.Repositories;
using Vakapay.Repositories.Mysql;

namespace Vakapay.TestWallet
{
    using VakacoinBusiness;
    class Program
    {
        static void TestVakacoinBusiness(VakapayRepositoryMysqlPersistenceFactory vkr)
        {
            VakacoinBusiness vkb = new VakacoinBusiness(vkr);
            vkb.CreateNewAddAddress("testID");
        }
        
        static void Main(string[] args)
        {
            Console.WriteLine("Program Test Make new Wallet!!!!");
            try
            {
                var repositoryConfig = new RepositoryConfiguration
                {
                    ConnectionString = "server=localhost;userid=root;password=admin;database=vakapay;port=3306;Connection Timeout=120;SslMode=none"
                };

                var PersistenceFactory = new VakapayRepositoryMysqlPersistenceFactory(repositoryConfig);

                const bool testWalletBusiness = true;
                const bool testVakacoinBusiness = false;

                if (testWalletBusiness) // testWalletBusiness
                {
                    var WalletBusiness = new WalletBusiness.WalletBusiness(PersistenceFactory);

                    var user = new User
                    {
                        Id = CommonHelper.GenerateUuid(),

                    };
                    var blockChainNetwork = new BlockchainNetwork
                    {
                        Name = "Ethereum",
                        Status = Status.StatusActive,
                        Sysbol = "ETH",
                        Id = CommonHelper.GenerateUuid()
                    };

                    var result = WalletBusiness.CreateNewWallet(user, blockChainNetwork);

                    Console.WriteLine(JsonHelper.SerializeObject(result).ToString());
                }

                if (testVakacoinBusiness)
                {
                    TestVakacoinBusiness(PersistenceFactory);
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            
        }
    }
}