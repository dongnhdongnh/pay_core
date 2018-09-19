namespace Vakapay.Models.Repositories.Base
{
    public interface IAddressRepository<TBlockchainAddress>
    {
        TBlockchainAddress FindByAddress(string address);
    }
}