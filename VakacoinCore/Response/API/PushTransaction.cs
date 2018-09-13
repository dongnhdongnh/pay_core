using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VakacoinCore.Params;
using VakacoinCore.Lib;

namespace VakacoinCore.Response.API
{
    public class PushTransaction : IEOAPI
    {
        public string transaction_id { get; set; }
        
        public VAKAAPIMetadata GetMetaData()
        {
            var meta = new VAKAAPIMetadata
            {
                uri = "/v1/chain/push_transaction"
            };

            return meta;
        }
    }
}
