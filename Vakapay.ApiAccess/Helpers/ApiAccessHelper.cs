using System;
using System.Text.RegularExpressions;
using Vakapay.Commons.Constants;
using Vakapay.Models.Domains;

namespace Vakapay.ApiServer.Helpers
{
    public static class ApiAccessHelper
    { 

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