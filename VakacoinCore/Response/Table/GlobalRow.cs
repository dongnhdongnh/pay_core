using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VakacoinCore.Lib;

namespace VakacoinCore.Response.Table
{
    public class GlobalRow : IVAKATable
    {
        public string total_producer_vote_weight { get; set; }

        public VAKATableMetadata GetMetaData()
        {
            var meta = new VAKATableMetadata
            {
                primaryKey = "",
                contract = "vaka",
                scope = "vaka",
                table = "global"
            };
            return meta;
        }
    }
}
