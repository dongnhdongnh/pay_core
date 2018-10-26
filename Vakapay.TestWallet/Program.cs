using System;
using Vakapay.Commons.Helpers;
using Vakapay.Models.Repositories;
using Vakapay.Repositories.Mysql;

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
                    ConnectionString = AppSettingHelper.GetDbConnection()
                };

                var persistenceFactory = new VakapayRepositoryMysqlPersistenceFactory(repositoryConfig);

//                foreach (var VARIABLE in RpcClient.GetAllTransactionsInBlock("16302351"))
//                {
//                    Console.WriteLine(VARIABLE.ToString());
//                }
//                var result = vakacoinBusiness.CreateTransactionHistory();


                var walletBusiness = new WalletBusiness.WalletBusiness(persistenceFactory);

                var walletLst = walletBusiness.GetAllWallet();

                foreach (var variable in walletLst)
                {
                    Console.WriteLine(JsonHelper.SerializeObject(variable));
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