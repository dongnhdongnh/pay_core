using Vakapay.BlockchainBusiness.Base;
using Vakapay.Models.Repositories;

namespace Vakapay.EthereumBusiness
{
    public class EthereumBusinessNew : AbsBlockchainBusiness
    {
        public EthereumBusinessNew(IVakapayRepositoryFactory vakapayRepositoryFactory, bool isNewConnection = true) : base(vakapayRepositoryFactory, isNewConnection)
        {
        }
    }
}