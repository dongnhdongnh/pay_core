using System;
using System.Text.RegularExpressions;
using Vakapay.Commons.Constants;
using Vakapay.Models.Domains;

namespace Vakapay.ApiServer.Helpers
{
    public static class ApiAccessHelper
    {
        public static bool ValidateId(string id)
        {
            const string pattern = "^([0-9a-k]{8}[-][0-9a-k]{4}[-][0-9a-k]{4}[-][0-9a-k]{4}[-][0-9a-k]{12})$";
            return Regex.IsMatch(id, pattern);
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