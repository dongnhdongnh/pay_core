﻿using System.Collections.Generic;
using System.Threading.Tasks;

namespace VakaSharp
{
    public interface ISignProvider
    {
        Task<IEnumerable<string>> GetAvailableKeys();

        Task<IEnumerable<string>> Sign(string chainId, IEnumerable<string> requiredKeys, byte[] signBytes,
            IEnumerable<string> abiNames = null);
    }
}