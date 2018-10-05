using System.Collections;
using System.Collections.Generic;

namespace Vakapay.Models.Domains
{
    public static class NetworkName
    {
        public static readonly IEnumerable<string> AllNetwork = new string[] { ETH, VAKA, BTC };
        public const string ETH = "Ethereum";
        public const string VAKA = "VAKA";
        public const string BTC = "BTC";

        public static Dictionary<string,string> CurrencySymbols { get; } = new Dictionary<string, string>()
        {
            {BTC, "BTC"},
            {ETH, "ETH"},
            {VAKA, "VAKA"}
        };
        
        public static string GetAmount(string networkName, decimal Amount)
        {
            if (networkName == VAKA)
            {
                return Amount.ToString("N4") + " " + CurrencySymbols[networkName];
            }

            return Amount + " " + CurrencySymbols[networkName];
        }
    }
}