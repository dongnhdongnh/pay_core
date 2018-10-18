using System;
using Vakapay.Commons.Helpers;
using Vakapay.Models.Domains;
using Vakapay.Models.Repositories;
using Vakapay.Repositories.Mysql;

namespace Vakapay.TestBitcoin
{
    using BitcoinBusiness;

    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var repositoryConfig = new RepositoryConfiguration
                {
                    ConnectionString = AppSettingHelper.GetDBConnection()
                };


                var PersistenceFactory = new VakapayRepositoryMysqlPersistenceFactory(repositoryConfig);
               
                var bitcoinRpc = new BitcoinBusiness(PersistenceFactory);
                var connection = PersistenceFactory.GetDbConnection();
                var rpc = new BitcoinRpc(AppSettingHelper.GetBitcoinNode(), AppSettingHelper.GetBitcoinRpcAuthentication());
                var bitcoinRepo = PersistenceFactory.GetBitcoinAddressRepository(connection);
                var walletBusiness = new WalletBusiness.WalletBusiness(PersistenceFactory);
                var result = bitcoinRpc.CreateAddressAsync( bitcoinRepo, rpc, "0c0f59ef-f14c-4b1f-b18d-ca28055162d5");
                Console.WriteLine(JsonHelper.SerializeObject(result));

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}