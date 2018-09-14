using System;
using System.Collections.Generic;
using System.Text;

namespace VakaSharp.DataAttributes
{
    class AbiFieldTypeAttribute : Attribute
    {
        public string AbiType { get; set; }

        public AbiFieldTypeAttribute(string abiType)
        {
            AbiType = abiType;
        }
    }
}
