using Vakapay.Models.Entities.VAKA;
using Vakapay.Models.Repositories.Base;

namespace Vakapay.Models.Repositories
{
    public interface IVakacoinAccountRepository : IRepositoryBase<VakacoinAccount>, IAddressRepository<VakacoinAccount>
    {
    }
}