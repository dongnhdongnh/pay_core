using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VakacoinCore.Lib;

namespace VakacoinCore.Response.API
{
    public class CurrencyBalance : IEOAPI, IVAKAtringArray
    {
        public List<String> balances { get; set; }
        
        public VAKAAPIMetadata GetMetaData()
        {
            var meta = new VAKAAPIMetadata
            {
                uri = "/v1/chain/get_currency_balance"
            };

            return meta;
        }

        public void SetStringArray(List<String> array)
        {
            balances = array;
        }
    }
}
