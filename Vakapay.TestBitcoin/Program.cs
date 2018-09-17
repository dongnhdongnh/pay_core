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

                var bitcoinRpc = new BitcoinBusiness(PersistenceFactory);
                //  bitcoinRpc.test("a");
                //bitcoinRpc.test("18382a96c79dc20a8b345c4e88708661a887cdfad22f27770638483805359d14");

                //address
                var address = bitcoinRpc.CreateNewAddAddress("815f09b4-e329-498d-bd0c-c389d0f6fb32");
                Console.WriteLine(JsonHelper.SerializeObject(address).ToString());
                //bitcoinRpc.test("18382a96c79dc20a8b345c4e88708661a887cdfad22f27770638483805359d14");

                //send many
                var mutil = new JObject();
                mutil.Add("2Mv5zhXas6Erc5bVRoSPXYGvqqoybyrghSS", 1);
                mutil.Add("2MskqLDPuAQ4bjmYhWWDNFegys3LCcQDcp9", 2);

                var send = bitcoinRpc.Sendmany("", mutil);
                Console.WriteLine(JsonHelper.SerializeObject(send));

//                var tran = bitcoinRpc.TransactionByTxidBlockchain(
//                    "7ae65806e278251462fae15117fc3193de74918b9e0aedc0b4166a475b3af683");

                //sendfrom 
                var sendfrom = bitcoinRpc.SendFrom("", "n1noYY4HBM38MxHN7cxir1fxtq2XjAprWi", 1);
                Console.WriteLine(JsonHelper.SerializeObject(sendfrom));

                //sendtoaddress
                var bitcoinraw = new BitcoinWithdrawTransaction
                {
                    Id = CommonHelper.GenerateUuid(),
                    Hash = "",
                    BlockNumber = "",
                    BlockHash = "",
                    NetworkName = "Bitcoin",
                    Amount = 1,
                    FromAddress = "",
                    ToAddress = "n1noYY4HBM38MxHN7cxir1fxtq2XjAprWi",
                    Fee = 0,
                    CreatedAt = (int)CommonHelper.GetUnixTimestamp(),
                    UpdatedAt = (int)CommonHelper.GetUnixTimestamp(),
                    Status = Status.StatusPending,
                };

                var bitcoinRawTransactionRepo =
                    bitcoinRpc.factory.GeBitcoinRawTransactionRepository(bitcoinRpc.dbconnect);
                bitcoinRawTransactionRepo.Insert(bitcoinraw);
                var send1 = bitcoinRpc.SendTransaction(bitcoinraw);
                Console.WriteLine(JsonHelper.SerializeObject(send1).ToString());


                //var list = bitcoinRpc.GetlistWallets();
                // Console.WriteLine(list);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}