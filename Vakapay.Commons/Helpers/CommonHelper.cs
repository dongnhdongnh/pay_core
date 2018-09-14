using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Vakapay.Commons.Helpers
{
	public static class CommonHelper
	{
		public static string GenerateUuid()
		{
			return Guid.NewGuid().ToString();
		}

		public static long GetUnixTimestamp()
		{
			return UnixTimestamp.ToUnixTimestamp(DateTime.UtcNow);
		}

		public static string Md5(string input)
		{
			// step 1, calculate MD5 hash from input
			var md5 = MD5.Create();
			var inputBytes = Encoding.ASCII.GetBytes(input);
			var hash = md5.ComputeHash(inputBytes);
			// step 2, convert byte array to hex string
			var sb = new StringBuilder();
			for (var i = 0; i < hash.Length; i++)
			{
				sb.Append(hash[i].ToString("X2"));
			}

			return sb.ToString();
		}

		public static bool ValidateGuid(string guidString)
		{
			return Guid.TryParse(guidString, out var _);
		}
		
		public static string RandomString(int length)
		{
			Random random = new Random();
			const string chars = "abcdefghijklmnopqrstuvwxyz0123456789";
			return new string(Enumerable.Repeat(chars, length)
				.Select(s => s[random.Next(s.Length)]).ToArray());
		}

		public static string RandomAccountNameVakacoin(int length = 12)
		{
			Random random = new Random();
			const string chars = "abcdefghijklmnopqrstuvwxyz12345";
			return new string(Enumerable.Repeat(chars, length)
				.Select(s => s[random.Next(s.Length)]).ToArray());
		}

		public static string IntToHex(this int input, string extra = null)
		{
			if (extra == null)
				return "0x" + input.ToString("X");
			else
				return extra + input.ToString("X");
		}

		public static bool HexToInt(this string hex, out int result)
		{
			char[] _trim_hex = new char[] { '0', 'x' };
			return int.TryParse(hex.TrimStart(_trim_hex), System.Globalization.NumberStyles.HexNumber, null, out result);

		}
	}
}