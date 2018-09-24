using NUnit.Framework;
using Vakapay.BitcoinBusiness;

namespace Vakapay.UnitTest
{
    [TestFixture]
    public class BTCRpcTest
    {
        BitcoinRpc btcRpc;
        [SetUp]
        public void Setup()
        {
            btcRpc = new BitcoinRpc("HELOO");
        }

        [Test]
        public void FindTransactionByHashTest()
        {
            var output = btcRpc.FindTransactionByHash("2Mv5zhXas6Erc5bVRoSPXYGvqqoybyrghSS");
            Assert.IsNotNull(output);
        }

        [Test]
        public void SendTransaction()
        {
            var output = btcRpc.SendToAddress("2Mv5zhXas6Erc5bVRoSPXYGvqqoybyrghSS", 1);
            
            Assert.IsNotNull(output);
        }
    }
}