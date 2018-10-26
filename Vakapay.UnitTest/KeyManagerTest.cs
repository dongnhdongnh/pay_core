using NUnit.Framework;
using Vakapay.Cryptography;

namespace Vakapay.UnitTest
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
            string privateKey = "5JtUScZK2XEp3g9gh7F8bwtPTRAkASmNrrftmx4AxDKD5K4zDnr";
            string publicKey = "VAKA69X3383RzBZj41k73CSjUNXM5MYGpnDxyPnWUKPEtYQmTBWz4D";
            Assert.AreEqual(KeyManager.GetVakaPublicKey(privateKey), publicKey);

            privateKey = "5KgsBi4ZomsE48s9ZZxntUvFkQyX5v2QooZrApKu3KezVoJQ7Fv";
            publicKey = "VAKA8RGpND31im9KpWJxm64kAbvVtjZQB6w5BujcvBDY58bvjT6RgH";
            Assert.AreEqual(KeyManager.GetVakaPublicKey(privateKey), publicKey);
        }
    }
}