using System.Collections.Generic;
using Vakapay.Commons.Constants;

namespace Vakapay.ApiServer.Models
{
    public class InfoApi
    {
        public Dictionary<string, string> ListApi = ApiAccess.LIST_API_ACCESS;
        public IEnumerable<string> ListWallet = CryptoCurrency.ALL_NETWORK;
    }
}