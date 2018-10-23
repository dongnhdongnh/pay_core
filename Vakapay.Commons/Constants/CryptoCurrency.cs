using System.Collections.Generic;

namespace Vakapay.Commons.Constants
{
    public static class CryptoCurrency
    {
        public static readonly IEnumerable<string> AllNetwork = new string[] { ETH, VAKA, BTC };
        public const string ETH = "Ethereum";
        public const string VAKA = "VAKA";
        public const string BTC = "BTC";

        public static string GetAmount(string currency, decimal amount)
        {
            if (currency == VAKA)
            {
                return amount.ToString("N4") + " " + currency;
            }

            return amount + " " + currency;
        }
    }
}