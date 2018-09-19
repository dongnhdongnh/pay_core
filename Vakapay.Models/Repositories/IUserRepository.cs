using System.Collections.Generic;
using Vakapay.Models.Entities;
using Vakapay.Models.Repositories.Base;

namespace Vakapay.Models.Repositories
{
    public interface IUserRepository : IRepositoryBase<User>
    {
        string FindEmailByBitcoinAddress(string bitcoinAddress);
    }
}