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
    public class TransactionResultParam
    {
       public string id { get; set; }
       public uint? block_num_hint { get; set; }
    }
}
