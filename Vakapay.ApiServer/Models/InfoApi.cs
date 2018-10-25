
using System.Collections.Generic;
using Vakapay.Commons.Constants;
using Vakapay.Models;
using Vakapay.Models.Entities;

namespace Vakapay.ApiServer.Models
{
    public class InfoApi
    {
        public Dictionary<string, string> listApi = Constants.listApiAccess;
        public IEnumerable<string> listWallet = CryptoCurrency.AllNetwork;
    }
}