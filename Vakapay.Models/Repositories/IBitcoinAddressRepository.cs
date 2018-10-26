using Vakapay.Models.Entities.BTC;
using Vakapay.Models.Repositories.Base;

namespace Vakapay.Models.Repositories
{
    public interface IBitcoinAddressRepository : IRepositoryBlockchainAddress<BitcoinAddress>
    {
    }
}