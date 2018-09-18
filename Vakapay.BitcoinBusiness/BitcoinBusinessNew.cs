using Vakapay.BlockchainBusiness.Base;
using Vakapay.Models.Repositories;

namespace Vakapay.BitcoinBusiness
{
    public class BitcoinBusinessNew : AbsBlockchainBusiness
    {
        public BitcoinBusinessNew(IVakapayRepositoryFactory vakapayRepositoryFactory, bool isNewConnection = true) : base(vakapayRepositoryFactory, isNewConnection)
        {
            
        }
    }
}