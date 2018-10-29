using NUnit.Framework;
using Vakapay.BitcoinBusiness;
using Vakapay.Commons.Helpers;

namespace Vakapay.UnitTest
{
    [TestFixture]
    public class BTCRpcTest
    {
        BitcoinRpc _btcRpc;

        [SetUp]
        public void Setup()
        {
            _btcRpc = new BitcoinRpc(AppSettingHelper.GetBitcoinNode(), AppSettingHelper.GetBitcoinRpcAuthentication());
        }

        [Test]
        public void FindTransactionByHashTest()
        {
            var output = _btcRpc.FindTransactionByHash("2Mv5zhXas6Erc5bVRoSPXYGvqqoybyrghSS");
            Assert.IsNotNull(output);
        }

        [Test]
        public void SendTransaction()
        {
            var output = _btcRpc.SendToAddress("2Mv5zhXas6Erc5bVRoSPXYGvqqoybyrghSS", 1);

            Assert.IsNotNull(output);
        }
    }
}