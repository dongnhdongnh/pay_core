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
    public class AbiBinToJsonParam
    {
        public string code { get; set; }
        public string action { get; set; }
        public string binargs { get; set; }        
    }
}
