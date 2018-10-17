using System;
using Newtonsoft.Json;
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
                    ConnectionString =
                        "server=127.0.0.1;userid=root;password=huan@123;database=vakapay;port=3306;Connection Timeout=120;SslMode=none"
                };


                var PersistenceFactory = new VakapayRepositoryMysqlPersistenceFactory(repositoryConfig);
                var bitcoinConnect = new BitcoinRPCConnect
                {
                    Host = "http://127.0.0.1:18443",
                    UserName = "bitcoinrpc",
                    Password = "7bLjxV1CKhNJmdxTUMxTpF4vEemWCp49kMX9CwvZabYi"
                };
               
                var bitcoinRpc = new BitcoinBusiness(PersistenceFactory);
                var connection = PersistenceFactory.GetDbConnection();
                var rpc = new BitcoinRpc(bitcoinConnect.Host, bitcoinConnect.UserName, bitcoinConnect.Password);
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