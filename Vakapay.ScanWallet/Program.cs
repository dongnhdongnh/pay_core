using System;
using System.Threading;
using System.Threading.Tasks;
using Vakapay.BitcoinBusiness;
using Vakapay.Commons.Helpers;
using Vakapay.Configuration;
using Vakapay.EthereumBusiness;
using Vakapay.Models.Domains;
using Vakapay.Models.Entities;
using Vakapay.Models.Entities.BTC;
using Vakapay.Models.Repositories;
using Vakapay.Repositories.Mysql;
using Vakapay.VakacoinBusiness;

namespace Vakapay.ScanWallet
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            try
            {
                for (var i = 0; i < 10; i++)
                {
                    var ts = new Thread(RunScan);
                    ts.Start();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private static void RunScan()
        {
            var repositoryConfig = new RepositoryConfiguration()
            {
                ConnectionString = VakapayConfiguration.DefaultSqlConnection
            };

            var repoFactory = new VakapayRepositoryMysqlPersistenceFactory(repositoryConfig);

            var business = new WalletBusiness.WalletBusiness(repoFactory);
            try
            {
                while (true)
                {
                    try
                    {
                        Console.WriteLine("Start Scan wallet...");
                        var resultSend = business.CreateAddressAsync();
                        Console.WriteLine(JsonHelper.SerializeObject(resultSend.Result));

                        Console.WriteLine("End Create Address...");
                        Thread.Sleep(100);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

//            try
//            {
//                var repositoryConfig = new RepositoryConfiguration()
//                {
//                    ConnectionString = VakapayConfiguration.DefaultSqlConnection
//                };
//
//                var repoFactory = new VakapayRepositoryMysqlPersistenceFactory(repositoryConfig);
//                var connection = repoFactory.GetDbConnection();
//                var walletBusiness = new WalletBusiness.WalletBusiness(repoFactory);
//                var walletRepo = repoFactory.GetWalletRepository(connection);
//                var ethAddressRepos = repoFactory.GetEthereumAddressRepository(connection);
//                var btcAddressRepos = repoFactory.GetBitcoinAddressRepository(connection);
//                var vakaAddressRepos = repoFactory.GetVakacoinAccountRepository(connection);
//
//                var ethereumBusiness = new EthereumBusiness.EthereumBusiness(repoFactory);
//                var bitcoinBusiness = new BitcoinBusiness.BitcoinBusiness(repoFactory);
//                var vakaBusiness = new VakacoinBusiness.VakacoinBusiness(repoFactory);
//
//                var bitcoinRpcAccount = VakapayConfiguration.GetBitcoinRpcAccount();
//
//                //	get all address = null with same networkName of walletId
//                while (true)
//                {
//                    try
//                    {
//                        Console.WriteLine("Scan wallet :START");
//                        var walletNoAddress = walletRepo.FindNullAddress();
//                        if (walletNoAddress == null || walletNoAddress.Count <= 0)
//                        {
//                        }
//                        else
//                        {
//                            Console.WriteLine("Scan wallet :START with " + walletNoAddress.Count);
//
//                            Task<ReturnObject> task = null;
//                            foreach (var wallet in walletNoAddress)
//                            {
//                                try
//                                {
//                                    var pass = CommonHelper.RandomString(15);
//                                    switch (wallet.Currency)
//                                    {
//                                        case CryptoCurrency.ETH:
//                                            Console.WriteLine("make eth");
//                                            task = ethereumBusiness.CreateAddressAsync<EthereumAddress>(walletBusiness,
//                                                ethAddressRepos,
//                                                new EthereumRpc(VakapayConfiguration.GetEthereumNode()),
//                                                wallet.Id, pass);
//                                            break;
//
//                                        case CryptoCurrency.BTC:
//                                            Console.WriteLine("make btc");
//                                            task = bitcoinBusiness.CreateAddressAsync<BitcoinAddress>(walletBusiness,
//                                                btcAddressRepos,
//                                                new BitcoinRpc(VakapayConfiguration.GetBitcoinNode(),
//                                                    bitcoinRpcAccount.Username,
//                                                    bitcoinRpcAccount.Password),
//                                                wallet.Id, pass);
//                                            break;
//
//                                        case CryptoCurrency.VKC:
//                                            Console.WriteLine("make vaka");
//                                            task = vakaBusiness.CreateAddressAsync<VakacoinAccount>(walletBusiness,
//                                                vakaAddressRepos,
//                                                new VakacoinRPC(VakapayConfiguration.GetVakacoinNode()),
//                                                wallet.Id, pass);
//                                            break;
//                                        default:
//                                            break;
//                                    }
//                                }
//                                catch (Exception e)
//                                {
//                                    Console.WriteLine(e.ToString());
////                                    throw;
//                                }
//                            }
//
//                            if (task != null)
//                            {
//                                task.Wait();
//                                Console.WriteLine(JsonHelper.SerializeObject(task.Result));
//                            }
//                        }
//
//                        Console.WriteLine("Scan wallet :END");
//                        //Task.
//                        Thread.Sleep(1000);
//                    }
//                    catch (Exception e)
//                    {
//                        Console.WriteLine(e.ToString());
//                        throw;
//                    }
//                }
//            }
//            catch (Exception e)
//            {
//                Console.WriteLine(e.ToString());
//                throw;
//            }
//        }
        }
    }
}