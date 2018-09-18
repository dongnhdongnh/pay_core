using System.Threading.Tasks;
using Vakapay.BlockchainBusiness;
using Vakapay.BlockchainBusiness.Base;
using Vakapay.Models.Domains;
using Vakapay.Models.Repositories;
using Vakapay.Models.Repositories.Base;

namespace Vakapay.VakacoinBusiness
{
    public class VakacoinBusinessNew : AbsBlockchainBusiness
    {
        public VakacoinBusinessNew(IVakapayRepositoryFactory vakapayRepositoryFactory, bool isNewConnection = true) : base(vakapayRepositoryFactory, isNewConnection)
        {
            
        }     
    }
}