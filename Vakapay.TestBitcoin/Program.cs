using System;
using Newtonsoft.Json.Linq;
using Vakapay.Commons.Helpers;
using Vakapay.Models.Domains;
using Vakapay.Models.Entities;
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
                    Password = "wqfgewgewi"
                };
                var bitcoinRpc = new BitcoinBusiness(PersistenceFactory, bitcoinConnect);
                //  bitcoinRpc.test("a");
                //bitcoinRpc.test("18382a96c79dc20a8b345c4e88708661a887cdfad22f27770638483805359d14");

                //address
                var address = bitcoinRpc.CreateNewAddAddress("815f09b4-e329-498d-bd0c-c389d0f6fb32");
                Console.WriteLine(JsonHelper.SerializeObject(address).ToString());
                //bitcoinRpc.test("18382a96c79dc20a8b345c4e88708661a887cdfad22f27770638483805359d14");
                return;
                
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}