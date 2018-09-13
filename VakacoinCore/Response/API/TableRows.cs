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
    public class TableRows : IEOAPI
    {
        public List<object> rows { get; set; }
        public bool more { get; set; }
        
        public VAKAAPIMetadata GetMetaData()
        {
            var meta = new VAKAAPIMetadata
            {
                uri = "/v1/chain/get_table_rows"
            };

            return meta;
        }
    }
}
