using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Vakapay.Commons.Helpers;
using Vakapay.Models.Domains;
using Vakapay.Models.Entities;
using Vakapay.Models.Repositories;
using Vakapay.Repositories.Mysql;
using Vakapay.VakacoinBusiness;

namespace Vakapay.TestWallet
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Program Test Make new Wallet!!!!");
            try
            {
                int t = 1;
                var cache = new System.Runtime.Caching.MemoryCache("testCache");
                DateTimeOffset timeOffset = new DateTimeOffset(DateTime.UtcNow + TimeSpan.FromTicks(1537027339));
//                timeOffset.AddYears(1);
//                cache.Set("test3", t, timeOffset);
//                cache["test1"] = t;
                var x = cache["test3"];
                var y = cache["test2"];
                Console.WriteLine("test3 = "+((int) x+1));
                
//                if (y == null) Console.WriteLine("aaaaaaaaaaaa");
//                var repositoryConfig = new RepositoryConfiguration
//                {
//                    ConnectionString = "server=localhost;userid=root;password=Abcd@1234;database=vakapay;port=3306;Connection Timeout=120;SslMode=none"
//                };
//
//                var PersistenceFactory = new VakapayRepositoryMysqlPersistenceFactory(repositoryConfig);
//
//                var vakacoinBusiness = new VakacoinBusiness.VakacoinBusiness(PersistenceFactory);
//                
//                VakacoinRpc rpc = new VakacoinRpc("http://api.eosnewyork.io");

//                foreach (var VARIABLE in rpc.GetAllTransactionsInBlock("16302351"))
//                {
//                    Console.WriteLine(VARIABLE.ToString());
//                }
//                var result = vakacoinBusiness.CreateTransactionHistory();



//                var WalletBusiness = new WalletBusiness.WalletBusiness(PersistenceFactory);
//
//                var walletLst = WalletBusiness.GetAllWallet();
//
//                foreach (var VARIABLE in walletLst)
//                {
//                    Console.WriteLine(JsonHelper.SerializeObject(VARIABLE));
//                }

//                var user = new User
//                {
//                    Id = CommonHelper.GenerateUuid(),
//                };
//                var blockChainNetwork = new BlockchainNetwork
//                {
//                    Name = "Ethereum",
//                    Status = Status.StatusActive,
//                    Sysbol = "ETH",
//                    Id = CommonHelper.GenerateUuid()
//                };
//
//                var result = WalletBusiness.CreateNewWallet(user, blockChainNetwork);

//                Console.WriteLine(JsonHelper.SerializeObject(walletLst));

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            
        }
    }
}