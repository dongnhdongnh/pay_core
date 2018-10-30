using System;
using System.Threading;
using Vakapay.Commons.Constants;
using Vakapay.Commons.Helpers;
using Vakapay.EthereumBusiness;
using Vakapay.Models.Entities.ETH;
using Vakapay.Models.Repositories;
using Vakapay.Repositories.Mysql;

namespace Vakapay.ScanEthereum
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            var repositoryConfig = new RepositoryConfiguration
            {
                ConnectionString = AppSettingHelper.GetDbConnection()
            };
            RunScan(repositoryConfig);
        }


        private static void RunScan(RepositoryConfiguration repositoryConfig)
        {
            var repoFactory = new VakapayRepositoryMysqlPersistenceFactory(repositoryConfig);

            var ethereumBusiness = new EthereumBusiness.EthereumBusiness(repoFactory);
            var walletBusiness = new WalletBusiness.WalletBusiness(repoFactory);
            var connection = repoFactory.GetOldConnection() ?? repoFactory.GetDbConnection();
            try
            {
                while (true)
                {
                    Console.WriteLine("==========Start Scan Ethereum==========");

                    var rpc = new EthereumRpc(AppSettingHelper.GetEthereumNode());

                    var ethereumRepo = repoFactory.GetEthereumWithdrawTransactionRepository(connection);
                    var ethereumDepoRepo = repoFactory.GetEthereumDepositeTransactionRepository(connection);
                    var resultSend =
                        ethereumBusiness
                            .ScanBlockAsync<EthereumWithdrawTransaction, EthereumDepositTransaction,
                                EthereumBlockResponse, EthereumTransactionResponse>(CryptoCurrency.ETH, walletBusiness,
                                ethereumRepo, ethereumDepoRepo, rpc);
                    Console.WriteLine(JsonHelper.SerializeObject(resultSend.Result));


                    Console.WriteLine("==========Scan Ethereum End==========");
                    Console.WriteLine("==========Wait for next scan==========");
                    Thread.Sleep(5000);
                }
            }
            catch (Exception e)
            {
                connection.Close();
                Console.WriteLine(e.ToString());
            }
        }
    }
}