using System.Collections.Generic;

namespace Vakapay.Commons.Constants
{
    public static class CryptoCurrency
    {
        public static readonly IEnumerable<string> AllNetwork = new string[] { ETH, VAKA, BTC };
        public const string ETH = "ETH";
        public const string VAKA = "VAKA";
        public const string BTC = "BTC";

//        public static Dictionary<string, string> CurrencySymbol { get; } = new Dictionary<string, string>()
//        {
//            {ETH, "ETH"},
//            {BTC, "BTC"},
//            {VAKA, "VAKA"}
//        };

        public static string GetSymbol(string currency)
        {
//            return CurrencySymbol[currency];
            return currency;
        }
        
        public static string GetAmount(string currency, decimal amount)
        {
            if (currency == VAKA)
            {
                return amount.ToString("N4") + " " + GetSymbol(currency);
            }

            return amount + " " + GetSymbol(currency);
        }
    }
}