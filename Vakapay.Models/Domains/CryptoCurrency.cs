using System.Collections.Generic;

namespace Vakapay.Models.Domains
{
    public static class CryptoCurrency
    {
        public static readonly IEnumerable<string> AllNetwork = new string[] { ETH, VAKA, BTC };
        public const string ETH = "Ethereum";
        public const string VAKA = "Vakacoin";
        public const string BTC = "Bitcoin";
        
        public static string GetAmount(string currency, decimal amount)
        {
            if (currency == VAKA)
            {
                return amount.ToString("N4") + " " + nameof(currency);
            }

            return amount + " " + nameof(currency);
        }
    }
}