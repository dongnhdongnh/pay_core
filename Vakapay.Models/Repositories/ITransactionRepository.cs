using Vakapay.Models.Entities;
using Vakapay.Models.Entities.BTC;
using Vakapay.Models.Repositories.Base;

namespace Vakapay.Models.Repositories
{
    public interface ITransactionRepository : IRepositoryBase<BitcoinDepositTransaction>
    {
        
    }
}