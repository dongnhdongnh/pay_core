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
    public class CodeParam
    {
        public string account_name { get; set; }
        public bool code_as_wasm { get; set; }
    }
}
