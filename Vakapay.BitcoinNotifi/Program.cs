using System;
using System.IO;
using Vakapay.Models.Domains;
using Vakapay.Models.Entities.BTC;
using Vakapay.Models.Repositories;
using Vakapay.Repositories.Mysql;

namespace Vakapay.BitcoinNotifi
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                string data = "\n data transaction: " + args[0];
                File.AppendAllText("/home/tienchelsea92/Desktop/test.txt",
                    "\n" + DateTime.Now.ToString() + "   " + data);

                var repositoryConfig = new RepositoryConfiguration
                {
                    ConnectionString =
                        "server=127.0.0.1;userid=root;password=Concuacang123!;database=vakapay;port=3306;Connection Timeout=120;SslMode=none"
                };

                var PersistenceFactory = new VakapayRepositoryMysqlPersistenceFactory(repositoryConfig);

                var btcBussines = new BitcoinBusiness.BitcoinBusiness(PersistenceFactory);
                ReturnObject transaction = btcBussines.GetTransaction(args[0]);
                File.AppendAllText("/home/tienchelsea92/Desktop/test.txt",
                    "GetTransaction: "+ transaction.Data);
                BTCTransactionModel transactionModel = BTCTransactionModel.FromJson(transaction.Data);
            }
            catch (Exception e)
            {
                File.AppendAllText("/home/tienchelsea92/Desktop/test.txt",
                    "Exception: "+ e.Message);
            }
        }
    }
}