using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VakacoinCore.Lib;

namespace VakacoinCore.Response.Table
{
    public class ProducerRow : IVAKATable
    {
        public string owner { get; set; }
        public string total_votes { get; set; }
        public string producer_key { get; set; }
        public bool is_active { get; set; }
        public long unpaid_blocks { get; set; }
        public string url { get; set; }

        public double total_votes_long
        {
            get
            {
                return double.Parse(total_votes);
            }
        }
        public VAKATableMetadata GetMetaData()
        {

            var meta = new VAKATableMetadata
            {
                primaryKey = "owner",
                contract = "vaka",
                scope = "vaka",
                table = "producers",
                key_type = "name"
            };

            return meta;
        }

    }
}
