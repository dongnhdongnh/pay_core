using System;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Org.BouncyCastle.Crypto.Digests;
using Vakapay.Commons.Helpers;

namespace Vakapay.Cryptography
{
    public class BlockchainHeper
    {
        public static bool ValidateBitcoinAddress(string address)
        {
            if (address.Length < 26 || address.Length > 35) throw new Exception("wrong length");
            var decoded = DecodeBase58(address);
            var d1 = Hash(decoded.SubArray(0, 21));
            var d2 = Hash(d1);
            if (!decoded.SubArray(21, 4).SequenceEqual(d2.SubArray(0, 4))) throw new Exception("bad digest");
            return true;
        }

        private const string ALPHABET = "123456789ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz";
        private const int SIZE = 25;

        private static byte[] DecodeBase58(string input)
        {
            var output = new byte[SIZE];
            foreach (var t in input)
            {
                var p = ALPHABET.IndexOf(t);
                if (p == -1) throw new Exception("invalid character found");
                var j = SIZE;
                while (--j > 0)
                {
                    p += 58 * output[j];
                    output[j] = (byte) (p % 256);
                    p /= 256;
                }

                if (p != 0) throw new Exception("address too long");
            }

            return output;
        }

        private static byte[] Hash(byte[] bytes)
        {
            var hasher = new SHA256Managed();
            return hasher.ComputeHash(bytes);
        }

        public static bool IsEthereumAddress(string address)
        {
            if (!new Regex("^(0x|0X)?[0-9a-fA-F]{40}$").IsMatch(address))
            {
                // check if it has the basic requirements of an address
                return false;
            }
            else if (new Regex("^(0x)?[0-9a-f]{40}$").IsMatch(address) ||
                     new Regex("^(0x|0X)?[0-9A-F]{40}$").IsMatch(address))
            {
                // If it's all small caps or all all caps, return "true
                return true;
            }
            else
            {
                // Otherwise check each case
                return IsChecksumAddress(address);
            }
        }

        public static bool IsBitcoinAddress(string address)
        {
            byte[] hex = Base58CheckToByteArray(address);
            if (hex == null || hex.Length != 21)
                return false;
            else
                return true;
        }

        public static byte[] Base58CheckToByteArray(string base58)
        {

            byte[] bb = Base58.ToByteArray(base58);
            if (bb == null || bb.Length < 4) return null;

            Sha256Digest bcsha256a = new Sha256Digest();
            bcsha256a.BlockUpdate(bb, 0, bb.Length - 4);

            byte[] checksum = new byte[32];  
            bcsha256a.DoFinal(checksum, 0);
            bcsha256a.BlockUpdate(checksum, 0, 32);
            bcsha256a.DoFinal(checksum, 0);

            for (int i = 0; i < 4; i++)
            {
                if (checksum[i] != bb[bb.Length - 4 + i]) return null;
            }

            byte[] rv = new byte[bb.Length - 4];
            Array.Copy(bb, 0, rv, 0, bb.Length - 4);
            return rv;
        }
        
        /// <summary>
        /// Validates if the hex string is 40 alphanumeric characters
        /// </summary>
        public bool IsValidEthereumAddressHexFormat(string address)
        {
            return address.HasHexPrefix() && IsValidAddressLength(address) &&
                   address.ToCharArray().All(char.IsLetterOrDigit);
        }

        private static bool IsChecksumAddress(string address)
        {
            address = address.RemoveHexPrefix();
            var addressHash = new Sha3Keccack().CalculateHash(address.ToLower());

            for (var i = 0; i < 40; i++)
            {
                var value = int.Parse(addressHash[i].ToString(), NumberStyles.HexNumber);
                // the nth letter should be uppercase if the nth digit of casemap is 1
                if (value > 7 && address[i].ToString().ToUpper() != address[i].ToString() ||
                    value <= 7 && address[i].ToString().ToLower() != address[i].ToString())
                    return false;
            }

            return true;
        }

        public bool IsValidAddressLength(string address)
        {
            address = address.RemoveHexPrefix();
            return address.Length == 40;
        }
    }

    public class Sha3Keccack
    {
        public string CalculateHash(string value)
        {
            var input = Encoding.UTF8.GetBytes(value);
            var output = CalculateHash(input);
            return output.ToHex();
        }

        public string CalculateHashFromHex(params string[] hexValues)
        {
            var joinedHex = string.Join("", hexValues.Select(x => x.RemoveHexPrefix()).ToArray());
            return CalculateHash(joinedHex.HexToByteArray()).ToHex();
        }

        public byte[] CalculateHash(byte[] value)
        {
            var digest = new KeccakDigest(256);
            var output = new byte[digest.GetDigestSize()];
            digest.BlockUpdate(value, 0, value.Length);
            digest.DoFinal(output, 0);
            return output;
        }
    }

    public static class HexByteConverterExtensions
    {
        private static readonly byte[] EMPTY = new byte[0];

        public static string ToHex(this byte[] value, bool prefix = false)
        {
            var strPrex = prefix ? "0x" : "";
            return strPrex + string.Concat(value.Select(b => b.ToString("x2")).ToArray());
        }

        public static bool HasHexPrefix(this string value)
        {
            return value.StartsWith("0x");
        }

        public static string RemoveHexPrefix(this string value)
        {
            return value.Replace("0x", "");
        }

        private static byte[] HexToByteArrayInternal(string value)
        {
            byte[] bytes = null;
            if (string.IsNullOrEmpty(value))
            {
                bytes = EMPTY;
            }
            else
            {
                var stringLength = value.Length;
                var characterIndex = value.StartsWith("0x", StringComparison.Ordinal) ? 2 : 0;
                // Does the string define leading HEX indicator '0x'. Adjust starting index accordingly.               
                var numberOfCharacters = stringLength - characterIndex;

                var addLeadingZero = false;
                if (0 != numberOfCharacters % 2)
                {
                    addLeadingZero = true;

                    numberOfCharacters += 1; // Leading '0' has been striped from the string presentation.
                }

                bytes = new byte[numberOfCharacters / 2]; // Initialize our byte array to hold the converted string.

                var writeIndex = 0;
                if (addLeadingZero)
                {
                    bytes[writeIndex++] = FromCharacterToByte(value[characterIndex], characterIndex);
                    characterIndex += 1;
                }

                for (var readIndex = characterIndex; readIndex < value.Length; readIndex += 2)
                {
                    var upper = FromCharacterToByte(value[readIndex], readIndex, 4);
                    var lower = FromCharacterToByte(value[readIndex + 1], readIndex + 1);

                    bytes[writeIndex++] = (byte) (upper | lower);
                }
            }

            return bytes;
        }

        public static byte[] HexToByteArray(this string value)
        {
            try
            {
                return HexToByteArrayInternal(value);
            }
            catch (FormatException ex)
            {
                throw new FormatException(string.Format(
                    "String '{0}' could not be converted to byte array (not hex?).", value), ex);
            }
        }

        private static byte FromCharacterToByte(char character, int index, int shift = 0)
        {
            var value = (byte) character;
            if (0x40 < value && 0x47 > value || 0x60 < value && 0x67 > value)
            {
                if (0x40 == (0x40 & value))
                    if (0x20 == (0x20 & value))
                        value = (byte) ((value + 0xA - 0x61) << shift);
                    else
                        value = (byte) ((value + 0xA - 0x41) << shift);
            }
            else if (0x29 < value && 0x40 > value)
            {
                value = (byte) ((value - 0x30) << shift);
            }
            else
            {
                throw new FormatException(string.Format(
                    "Character '{0}' at index '{1}' is not valid alphanumeric character.", character, index));
            }

            return value;
        }
    }

    public static class ArrayExtensions
    {
        public static T[] SubArray<T>(this T[] data, int index, int length)
        {
            var result = new T[length];
            Array.Copy(data, index, result, 0, length);
            return result;
        }
    }
}