using System;
using Vakapay.Commons.Helpers;
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
                    ConnectionString = "server=127.0.0.1;userid=root;password=Concuacang123!;database=vakapay;port=3306;Connection Timeout=120;SslMode=none"
                };
                

                var PersistenceFactory = new VakapayRepositoryMysqlPersistenceFactory(repositoryConfig);
                
                var bitcoinRpc = new BitcoinBusiness(PersistenceFactory);
                var address = bitcoinRpc.CreateNewAddAddress(null);
                Console.WriteLine(JsonHelper.SerializeObject(address).ToString());
                var list = bitcoinRpc.GetlistWallets();
                Console.WriteLine(list);

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}