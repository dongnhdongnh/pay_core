using System;
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
        
        static void Main(string[] args)
        {
            Console.WriteLine("Program Test Make new Wallet!!!!");
            try
            {
                var repositoryConfig = new RepositoryConfiguration
                {
                    ConnectionString = "server=127.0.0.1;userid=root;password=Concuacang123!;database=vakapay;port=3306;Connection Timeout=120;SslMode=none"
                };

                var PersistenceFactory = new VakapayRepositoryMysqlPersistenceFactory(repositoryConfig);


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
            catch (Exception e)
            {
                throw e;
            }
            
        }
    }
}