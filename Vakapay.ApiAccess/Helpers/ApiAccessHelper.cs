using System;
using System.Linq;
using System.Text.RegularExpressions;
using Vakapay.Commons.Constants;
using Vakapay.Models.Domains;

namespace Vakapay.ApiServer.Helpers
{
    public static class ApiAccessHelper
    {
        public static bool ValidateCurrency(string currency)
        {
            return CryptoCurrency.AllNetwork.Contains(currency);
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