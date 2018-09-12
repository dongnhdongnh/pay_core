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
    public class Authorization
    {
        [JsonConverter(typeof(AccountName))]
        public AccountName actor { get; set; }
        [JsonConverter(typeof(PermissionName))]
        public PermissionName permission { get; set; }
    }
}
