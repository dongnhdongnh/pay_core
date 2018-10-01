using System.Collections.Generic;

namespace Vakapay.Models.Domains
{
    public static class NetworkName
    {
        public static readonly IEnumerable<string> AllNetwork = new string[] { ETH, VAKA, BTC };
        public const string ETH = "Ethereum";
        public const string VAKA = "VAKA";
        public const string BTC = "BTC";
    }
}