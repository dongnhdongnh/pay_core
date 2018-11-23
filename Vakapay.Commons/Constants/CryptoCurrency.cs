using System;
using System.Collections.Generic;

namespace Vakapay.Commons.Constants
{
    public static class CryptoCurrency
    {
        public const string ETH = "Ethereum";
        public const string VAKA = "Vakacoin";
        public const string BTC = "Bitcoin";

        public static readonly IEnumerable<string> ALL_NETWORK = new string[] { ETH, VAKA, BTC };

        private static readonly Dictionary<string, string> SYMBOLS = new Dictionary<string, string>
        {
            {ETH, "ETH"},
            {VAKA, "VAKA"},
            {BTC, "BTC"}
        };

        public static string GetAmount(string currency, decimal amount)
        {
            try
            {
                if (currency == VAKA)
                {
                    return amount.ToString("N4") + " " + SYMBOLS[currency];
                }

                return amount + " " + SYMBOLS[currency];
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return amount + " " + currency;
            }

        }
    }
}