using System;
using System.Threading;
using System.Threading.Tasks;
using Vakapay.BitcoinBusiness;
using Vakapay.Commons.Helpers;
using Vakapay.Configuration;
using Vakapay.EthereumBusiness;
using Vakapay.Models.Domains;
using Vakapay.Models.Entities;
using Vakapay.Models.Repositories;
using Vakapay.Repositories.Mysql;
using Vakapay.VakacoinBusiness;

namespace Vakapay.ScanWallet
{
    internal static class Program
    {
//		const string RPCEndpoint = "http://localhost:9900";
//		private static string ConnectionString { get; } = VakapayConfiguration.DefaultSqlConnection;

        private static void Main(string[] args)
        {
            RunScan();
        }

        private static void RunScan()
        {
            try
            {
                var repositoryConfig = new RepositoryConfiguration()
                {
                    ConnectionString = VakapayConfiguration.DefaultSqlConnection
                };

                var persistence = new VakapayRepositoryMysqlPersistenceFactory(repositoryConfig);
                var repoFactory = new VakapayRepositoryMysqlPersistenceFactory(repositoryConfig);
                var connection = repoFactory.GetDbConnection();
                var walletBusiness = new WalletBusiness.WalletBusiness(persistence);
                var walletRepo = repoFactory.GetWalletRepository(connection);
                var ethAddressRepos = repoFactory.GetEthereumAddressRepository(connection);
                var btcAddressRepos = repoFactory.GetBitcoinAddressRepository(connection);
                var vakaAddressRepos = repoFactory.GetVakacoinAccountRepository(connection);

                var ethereumBusiness = new EthereumBusiness.EthereumBusiness(repoFactory);
                var bitcoinBusiness = new BitcoinBusiness.BitcoinBusiness(repoFactory);
                var vakaBusiness = new VakacoinBusiness.VakacoinBusiness(repoFactory);

                //	get all address = null with same networkName of walletId
                while (true)
                {
                    try
                    {
                        Console.WriteLine("Scan wallet :START");
                        var walletNoAddress = walletRepo.FindNullAddress();
                        if (walletNoAddress == null || walletNoAddress.Count <= 0)
                        {
                        }
                        else
                        {
                            Console.WriteLine("Scan wallet :START with " + walletNoAddress.Count);

                            Task<ReturnObject> task = null;
                            foreach (var wallet in walletNoAddress)
                            {
                                try
                                {
                                    var pass = CommonHelper.RandomString(15);
                                    switch (wallet.NetworkName)
                                    {
                                        case NetworkName.ETH:
                                            Console.WriteLine("make eth");
                                            task = ethereumBusiness.CreateAddressAsync<EthereumAddress>(walletBusiness,
                                                ethAddressRepos,
                                                new EthereumRpc(VakapayConfiguration.GetEthereumNode()),
                                                wallet.Id, pass);
                                            break;

                                        case NetworkName.BTC:
                                            Console.WriteLine("make btc");
                                            task = bitcoinBusiness.CreateAddressAsync<BitcoinAddress>(walletBusiness,
                                                btcAddressRepos,
                                                new BitcoinRpc(VakapayConfiguration.GetBitcoinNode(), "",
                                                    ""), //TODO Username Password
                                                wallet.Id, pass);
                                            break;

                                        case NetworkName.VAKA:
                                            Console.WriteLine("make vaka");
                                            task = vakaBusiness.CreateAddressAsync<VakacoinAccount>(walletBusiness,
                                                vakaAddressRepos,
                                                new VakacoinRPC(VakapayConfiguration.GetVakacoinNode()),
                                                wallet.Id, pass);
                                            break;
                                        default:
                                            break;
                                    }
                                }
                                catch (Exception e)
                                {
                                    Console.WriteLine(e.ToString());
//                                    throw;
                                }
                            }

                            if (task != null)
                            {
                                task.Wait();
                                Console.WriteLine(JsonHelper.SerializeObject(task.Result));
                            }
                        }

                        Console.WriteLine("Scan wallet :END");
                        //Task.
                        Thread.Sleep(1000);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.ToString());
                        throw;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                throw;
            }
        }
    }
}