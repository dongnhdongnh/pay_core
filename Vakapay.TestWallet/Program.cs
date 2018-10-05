using System;
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
                var repositoryConfig = new RepositoryConfiguration
                {
                    ConnectionString = "server=localhost;userid=root;password=Abcd@1234;database=vakapay;port=3306;Connection Timeout=120;SslMode=none"
                };

                var PersistenceFactory = new VakapayRepositoryMysqlPersistenceFactory(repositoryConfig);

                var vakacoinBusiness = new VakacoinBusiness.VakacoinBusiness(PersistenceFactory);
                
                VakacoinRPC rpc = new VakacoinRPC("http://api.eosnewyork.io");

//                foreach (var VARIABLE in RpcClient.GetAllTransactionsInBlock("16302351"))
//                {
//                    Console.WriteLine(VARIABLE.ToString());
//                }
//                var result = vakacoinBusiness.CreateTransactionHistory();
                
                

                var WalletBusiness = new WalletBusiness.WalletBusiness(PersistenceFactory);

                var walletLst = WalletBusiness.GetAllWallet();

                foreach (var VARIABLE in walletLst)
                {
                    Console.WriteLine(JsonHelper.SerializeObject(VARIABLE));
                }
                
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