namespace Vakapay.Models.Repositories.Base
{
    public interface IRepositoryBlockchainTransaction<TBlockchainTransaction> : IRepositoryTransaction,
        IRepositoryBase<TBlockchainTransaction>
    {
        
    }
}