using System.Threading.Tasks;
using Vakapay.Models.Domains;

namespace Vakapay.Models.Repositories.Base
{
    public interface IRepositoryBlockchainTransaction<TBlockchainTransaction> : IRepositoryTransaction,
        IRepositoryBase<TBlockchainTransaction>
    {
        
    }
}