using System;
using Vakapay.Commons.Constants;
using Vakapay.Commons.Helpers;
using Vakapay.Models.Entities.ETH;
using Vakapay.Models.Repositories;
using Vakapay.Repositories.Mysql;

namespace Vakapay.TestCreateTransaction
{
    class Program
    {
        static void Main(string[] args)
        {
            var repositoryConfig = new RepositoryConfiguration
            {
                ConnectionString = AppSettingHelper.GetDbConnection()
            };


            var persistenceFactory = new VakapayRepositoryMysqlPersistenceFactory(repositoryConfig);
            var ethereumWithdrawTransactionRepository =
                persistenceFactory.GetEthereumWithdrawTransactionRepository(persistenceFactory.GetDbConnection());
            try
            {
                for (var i = 0; i < 20; i++)
                {
                    var trans = new EthereumWithdrawTransaction
                    {
                        Amount = 1,
                        Fee = 0,
                        BlockNumber = 0,
                        FromAddress = null,
                        Hash = null,
                        IsProcessing = 0,
//                        NetworkName = "ETH",
                        Status = Status.STATUS_PENDING,
                        Version = 0,
                        ToAddress = "0x13f022d72158410433cbd66f5dd8bf6d2d129924"
                    };
                    var resultInsert = ethereumWithdrawTransactionRepository.Insert(trans);
                    Console.WriteLine(JsonHelper.SerializeObject(resultInsert));
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }
}