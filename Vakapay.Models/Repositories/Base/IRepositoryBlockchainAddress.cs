namespace Vakapay.Models.Repositories.Base
{
    public interface IRepositoryBlockchainAddress<TBlockchainAddress> : IAddressRepository<TBlockchainAddress>,
        IRepositoryBase<TBlockchainAddress>
    {
    }
}