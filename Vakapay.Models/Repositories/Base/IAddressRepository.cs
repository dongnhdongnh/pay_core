using System.Collections.Generic;
using System.Threading.Tasks;
using Vakapay.Models.Domains;

namespace Vakapay.Models.Repositories.Base
{
    public interface IAddressRepository<TBlockchainAddress>
    {
        TBlockchainAddress FindByAddress(string address);
        List<TBlockchainAddress> FindByWalletId(string walletId);
        Task<ReturnObject> InsertAddress(string address, string walletId, string other);
    }
}