using System.Collections.Generic;

namespace Vakapay.Commons.Constants
{
    public static class CryptoCurrency
    {
        public const string ETH = "Ethereum";
        public const string VAKA = "Vakacoin";
        public const string BTC = "Bitcoin";
        
        public static readonly IEnumerable<string> AllNetwork = new string[] { ETH, VAKA, BTC };
        private static readonly Dictionary<string, string> Symbols = new Dictionary<string, string>
        {
            { ETH, "ETH" },
            { VAKA, "VAKA" },
            { BTC, "BTC" }
        };

        public static string GetAmount(string currency, decimal amount)
        {
            if (currency == VAKA)
            {
                return amount.ToString("N4") + " " + Symbols[currency];
            }

            return amount + " " + Symbols[currency];
        }
    }
}