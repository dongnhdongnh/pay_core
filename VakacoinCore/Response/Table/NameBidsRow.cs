using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VakacoinCore.Utilities;
using VakacoinCore.Lib;

namespace VakacoinCore.Response.Table
{
    public class NameBidsRow : IVAKATable
    {
        public string newname { get; set; }
        public string high_bidder { get; set; }
        public long high_bid { get; set; }
        public string last_bid_time { get; set; }
        public string last_bid_time_utc
        {
            get
            {
                return VAKAUtility.FromUnixTime(long.Parse(last_bid_time) / 1000000).ToString();
            }
        }
        public VAKATableMetadata GetMetaData()
        {

            var meta = new VAKATableMetadata
            {
                primaryKey = "newname",
                contract = "vaka",
                scope = "vaka",
                table = "namebids"
            };

            return meta;
        }
    }
}
