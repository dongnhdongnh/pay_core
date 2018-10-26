using NUnit.Framework;
using Vakapay.Cryptography;

namespace Vakapay.UnitTest
{
    [TestFixture]
    public class BlockchainHelperTest
    {
        [SetUp]
        public void Setup()
        {
        }

        [TestCase(true, "0xc1912fee45d61c87cc5ea59dae31190fffff232d")]
        [TestCase(true, "c1912fee45d61c87cc5ea59dae31190fffff232d")]
        [TestCase(true, "0XC1912FEE45D61C87CC5EA59DAE31190FFFFF232D")]
        [TestCase(true, "0XC1912FEE45D61C87CC5EA59DAE31190FFFEF232D")]
        [TestCase(true, "0xc1912fEE45d61C87Cc5EA59DaE31190FFFFf232d")]
        [TestCase(false, "0xc1912fEE45d61C87Cc5EA59DaE31190FFFEf232d")]
        [TestCase(false, "0xc1912fEE45d61C87Cc5EA59DaE31190FFFFf232l")]
        [TestCase(false, "0xC1912fEE45d61C87Cc5EA59DaE31190FFFFf232d")]
        public void IsAddress(bool result, string address)
        {
            Assert.AreEqual(result, BlockchainHeper.IsEthereumAddress(address));
        }
    }
}