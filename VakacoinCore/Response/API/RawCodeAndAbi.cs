using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VakacoinCore.Params;
using VakacoinCore.Lib;
using Newtonsoft.Json;

namespace VakacoinCore.Response.API
{
    public class RawCodeAndAbi : IEOAPI
    {
        public string account_name { get; set; }
        public string wasm { get; set; }
        public string abi { get; set; }
        
        public VAKAAPIMetadata GetMetaData()
        {
            var meta = new VAKAAPIMetadata
            {
                uri = "/v1/chain/get_raw_code_and_abi"
            };

            return meta;
        }
    }
}
