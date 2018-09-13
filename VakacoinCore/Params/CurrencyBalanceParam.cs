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
    public class CurrencyBalanceParam
    {
        public string account { get; set; }
        public string code { get; set; }
        public string symbol { get; set; }
    }
}
