
using System.Collections.Generic;
using Vakapay.Models;
using Vakapay.Models.Entities;

namespace Vakapay.ApiServer.Models
{
    public class InfoApi
    {
        public Dictionary<string, string> listApi = Constants.listApiAccess;
        public List<Wallet> listWallet { get; set; }
    }
}