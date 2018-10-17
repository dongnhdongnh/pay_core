using System.Collections.Generic;

namespace Vakapay.Commons.Constants
{
    public static class CryptoCurrency
    {
        public static readonly IEnumerable<string> AllNetwork = new string[] { ETH, VAKA, BTC };
        public const string ETH = "Ethereum";
        public const string VAKA = "Vakacoin";
        public const string BTC = "Bitcoin";

        private static Dictionary<string, string> CurrencySymbol { get; } = new Dictionary<string, string>()
        {
            {ETH, "ETH"},
            {BTC, "BTC"},
            {VAKA, "VAKA"}
        };
        
        public static string GetAmount(string currency, decimal amount)
        {
            if (currency == VAKA)
            {
                return amount.ToString("N4") + " " + CurrencySymbol[currency];
            }

            return amount + " " + CurrencySymbol[currency];
        }
    }
}