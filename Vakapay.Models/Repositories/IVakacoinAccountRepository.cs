using Vakapay.Models.Entities.VAKA;
using Vakapay.Models.Repositories.Base;
using System;
namespace Vakapay.Models.Repositories
{
    public interface IVakacoinAccountRepository : IRepositoryBase<VakacoinAccount>, IAddressRepository<VakacoinAccount>,IDisposable
    {
    }
}