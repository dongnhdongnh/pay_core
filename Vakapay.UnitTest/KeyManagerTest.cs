using NUnit.Framework;
using Vakapay.Cryptography;

namespace Vakapay
{
    [TestFixture]
    public class KeyManagerTest
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void GetVakaPublicKey()
        {
            const string privateKey = "5JtUScZK2XEp3g9gh7F8bwtPTRAkASmNrrftmx4AxDKD5K4zDnr";
            const string publicKey = "VAKA69X3383RzBZj41k73CSjUNXM5MYGpnDxyPnWUKPEtYQmTBWz4D";
            Assert.AreEqual(KeyManager.GetVakaPublicKey(privateKey), publicKey);
        }
    }
    
}