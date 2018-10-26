using System.Linq;
using Vakapay.Commons.Constants;
using Vakapay.Commons.Helpers;
using Vakapay.Models.Domains;

namespace Vakapay.ApiAccess.Helpers
{
    public static class ApiAccessHelper
    {
        public static bool ValidateCurrency(string currency)
        {
            return CryptoCurrency.ALL_NETWORK.Contains(currency);
        }

        public static string CreateDataError(string message)
        {
            return new ReturnObject
            {
                Status = Status.STATUS_ERROR,
                Message = message
            }.ToJson();
        }
    }
}