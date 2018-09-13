using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VakacoinCore;
using VakacoinCore.Serialization;
using Newtonsoft.Json;

namespace VakacoinCore.Params
{
    public class Action
    {
        [JsonConverter(typeof(AccountName))]
        public AccountName account { get; set; }
        [JsonConverter(typeof(ActionName))]
        public ActionName name { get; set; }
        public Authorization[] authorization { get; set; }
        [JsonConverter(typeof(BinaryString))]
        public BinaryString data { get; set; }        
    }
}
