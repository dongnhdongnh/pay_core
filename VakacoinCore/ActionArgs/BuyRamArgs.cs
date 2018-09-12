using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VakacoinCore.ActionArgs
{
    public class BuyRamArgs
    {
        public string payer { get; set; }
        public string receiver { get; set; }
        public string quant { get; set; }
    }
}
