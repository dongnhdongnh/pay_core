using System;

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
