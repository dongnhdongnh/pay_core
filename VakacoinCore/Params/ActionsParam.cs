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
    public class ActionsParam
    {
        public int pos { get; set; }
        public int offset { get; set; }
        public string account_name { get; set; }        
    }
}
