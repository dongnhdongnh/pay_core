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
    public class AbiJsonToBinParam
    {
        public string code { get; set; }
        public string action { get; set; }
        public object args { get; set; }
    }
}
