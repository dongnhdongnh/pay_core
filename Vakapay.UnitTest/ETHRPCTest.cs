using NUnit.Framework;
using System;
using System.Threading;
using Vakapay.EthereumBusiness;
using Vakapay.Models.Entities;
using Vakapay.Commons.Helpers;
namespace Vakapay.UnitTest
{

    [TestFixture]
    class ETHRPCTest
    {

        EthereumRpc _etheRpc;
        [SetUp]
        public void Setup()
        {
            _etheRpc = new EthereumRpc("http://localhost:9900");
        }

        [TestCase(0)]
        public void GetBlock(int id)
        {
            var output = _etheRpc.FindBlockByNumber(id);
            //Trace.Write(JsonHelper.SerializeObject(output));
            //	Console.WriteLine(JsonHelper.SerializeObject(output));
            Assert.IsNotNull(output);
        }

        [Test]
        public async void SendTransactionAsync()
        {

            // var _etheRpc = new EthereumRpc("http://localhost:9900");


            // var output = _etheRpc.SendTransactionWithPassphrase("0x12890d2cce102216644c59dae5baed380d84830c", "0x3a2e25cfb83d633c184f6e4de1066552c5bf4517", 10, "password");
            //var output = _etheRpc.SendTransaction("0x12890d2cce102216644c59dae5baed380d84830c", "0x3a2e25cfb83d633c184f6e4de1066552c5bf4517", 10);
            //Trace.Write(JsonHelper.SerializeObject(output));
            var output = _etheRpc.SendTransactionAsync(new EthereumWithdrawTransaction() { Amount = 10, ToAddress = "0x3a2e25cfb83d633c184f6e4de1066552c5bf4517" });
            var thing = await output;
            Console.WriteLine(JsonHelper.SerializeObject(thing));
            //  var _balance = _etheRpc.GetBalance("0x3a2e25cfb83d633c184f6e4de1066552c5bf4517");
            // Thread.Sleep(1000);
            //  var ba = 0;
            // _balance.Data.ToString().HexToInt(out ba);
            //  Console.WriteLine("BALANCE:" + ba);
            //Assert.IsNotNull(output);
        }

        [Test]
        public void SendTransactionMultiThread()
        {
            var _balance = _etheRpc.GetBalance("0x3a2e25cfb83d633c184f6e4de1066552c5bf4517");
            var ba = 0;
            _balance.Data.ToString().HexToInt(out ba);
            Console.WriteLine("START BALANCE:" + ba);
            int numOfThread = 20;
            for (var i = 0; i < numOfThread; i++)
            {
                var ts = new Thread(async () => SendTransactionAsync());
                ts.Start();
            }
            //Thread.Sleep(10000);
            var ba2 = 0;
            while (true)
            {
                var _balance2 = _etheRpc.GetBalance("0x3a2e25cfb83d633c184f6e4de1066552c5bf4517");
                // Thread.Sleep(1000);
             
                _balance2.Data.ToString().HexToInt(out ba2);
                if (ba2 - ba >= numOfThread * 10)
                    break;
            }
            Console.WriteLine("END RUN " + ba2);
            Assert.IsNotNull(1);
        }
    }
}
