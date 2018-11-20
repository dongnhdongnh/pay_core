using Vakapay.Models.Entities.ETH;
using Vakapay.Models.Repositories.Base;
using System;
namespace Vakapay.Models.Repositories
{
    public interface IEthereumAddressRepository : IRepositoryBase<EthereumAddress>, IAddressRepository<EthereumAddress>,IDisposable
    {
    }
}