namespace Vakapay.Models.Domains
{
    public interface IBlockchainAddress
    {
        string GetAddress();
        string GetSecret();
    }
}
