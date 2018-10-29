using Cryptography.ECDSA;

namespace Vakapay.Cryptography
{
    public class KeyManager
    {
        public static KeyPair GenerateKeyPair()
        {
            KeyPair keyPair = new KeyPair();
            byte[] privateKey = Secp256K1Manager.GenerateRandomKey();
            keyPair.PrivateKey = WifUtility.GetPrivateWif(privateKey);
            byte[] publicKey = Secp256K1Manager.GetPublicKey(privateKey, true);
            keyPair.PublicKey = WifUtility.GetPublicWif(publicKey, "VAKA");
            return keyPair;
        }

        public static string GetVakaPublicKey(string privatekey)
        {
            var privateBytes = WifUtility.DecodePrivateWif(privatekey);
            var publicKey = Secp256K1Manager.GetPublicKey(privateBytes, true);
            return WifUtility.GetPublicWif(publicKey, "VAKA");
        }

        public class KeyPair
        {
            public string PrivateKey { get; set; }

            public string PublicKey { get; set; }
        }
    }
}