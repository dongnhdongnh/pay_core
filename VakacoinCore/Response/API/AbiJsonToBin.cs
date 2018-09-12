using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VakacoinCore.Params;
using VakacoinCore.Lib;

namespace VakacoinCore.Response.API
{
    public class AbiJsonToBin : IEOAPI
    {
        public string binargs { get; set; }
        
        public VAKAAPIMetadata GetMetaData()
        {
            var meta = new VAKAAPIMetadata
            {
                uri = "/v1/chain/abi_json_to_bin"
            };

            return meta;
        }
    }
}
