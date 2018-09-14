using Vakapay.Models.Domains;

namespace Vakapay.BlockchainBusiness
{
    public interface IBlockchainBusiness
    {
        ReturnObject SendTransaction(string From, string To, decimal amount);
        ReturnObject SendTransaction(string From, string To, decimal amount, string Password);
        ReturnObject CreateNewAddress(string WalletId, string password = "");
        
    }
}