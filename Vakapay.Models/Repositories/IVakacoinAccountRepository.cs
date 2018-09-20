using Vakapay.Models.Entities;
using Vakapay.Models.Repositories.Base;

namespace Vakapay.Models.Repositories
{
    public interface IVakacoinAccountRepository : IRepositoryBase<VakacoinAccount>, IAddressRepository<VakacoinAccount>
    {
        VakacoinAccount FindByAccountName(string accountName);
    }
}