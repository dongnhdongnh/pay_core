using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;
using Vakapay.Models.Domains;
using Vakapay.Commons.Helpers;
using Cryptography.ECDSA;


namespace Vakapay.VakacoinBusiness
{
    public class VakacoinRpc
    {
        private string EndPointUrl { get; set; }
        private string VakacoinVersion { get; } = "v1";
        
        public VakacoinRpc(string endPointUrl)
        {
            if (endPointUrl[endPointUrl.Length - 1] != '/')
            {
                endPointUrl += "/";
            }

            endPointUrl += VakacoinVersion;
            
            EndPointUrl = endPointUrl; // return end://point.url/v1
        }

        private string GetAccountPostUrl()
        {
            return EndPointUrl + "/chain/get_account";
        }

        private string GetInfoPostUrl()
        {
            return EndPointUrl + "/chain/get_info";
        }

        public ReturnObject CreateAddress(string password)
        {
            return null;
        }

        public bool CheckAccountExist(string accountName)
        {
            var values = new Dictionary<string, string>
            {
                { "account_name", accountName },
            };

            var result = HttpClientHelper.PostRequest(GetAccountPostUrl(), values);
            var jResult= JObject.Parse(result);
            var exist = jResult["error"] == null;
            return exist;
        }

        public VakaKeyPair CreateKey()
        {
            byte[] keybytes = Secp256K1Manager.GenerateRandomKey();
            var version = 0x80;
            
            
            return new VakaKeyPair();
        }
        
        public static string KeyToString(byte[] key, string keyType, string prefix = null)
        {
            byte[] digest = null;

            if (keyType == "sha256x2")
            {
                digest = Sha256Manager.GetHash(Sha256Manager.GetHash(Combine(new List<byte[]>() {
                    new byte[] { 128 },
                    key
                })));
            }
            else if (!string.IsNullOrWhiteSpace(keyType))
            {
                digest = Ripemd160Manager.GetHash(Combine(new List<byte[]>() {
                    key,
                    Encoding.UTF8.GetBytes(keyType)
                }));
            }
            else
            {
                digest = Ripemd160Manager.GetHash(key);
            }

            return prefix + Base58.Encode(Combine(new List<byte[]>() {
                key,
                digest.Take(4).ToArray()
            }));
        }
        
        public static byte[] Combine(IEnumerable<byte[]> arrays)
        {
            byte[] ret = new byte[arrays.Sum(x => x != null ? x.Length : 0)];
            int offset = 0;
            foreach (byte[] data in arrays)
            {
                if (data == null) continue;

                Buffer.BlockCopy(data, 0, ret, offset, data.Length);
                offset += data.Length;
            }
            return ret;
        }

        public void MainTest()
        {
            
        }
     
//        private static object ReadPrivateKey(byte[] data, ref Int32 readIndex)
//        {
//            var type = (byte)ReadByte(data, ref readIndex);
//            var keyBytes = data.Skip(readIndex).Take(CryptoHelper.PRIV_KEY_DATA_SIZE).ToArray();
//
//            readIndex += CryptoHelper.PRIV_KEY_DATA_SIZE;
//
//            if (type == (int)KeyType.r1)
//            {
//                return CryptoHelper.PrivKeyBytesToString(keyBytes, "R1", "PVT_R1_");
//            }
//            else
//            {
//                throw new Exception("private key type not supported.");
//            }
//        }
    }
}