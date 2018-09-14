using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace VakaSharp
{
    public interface ISignProvider
    {
        Task<IEnumerable<string>> GetAvailableKeys();
        Task<IEnumerable<string>> Sign(string chainId, List<string> requiredKeys, byte[] signBytes);
    }
}
