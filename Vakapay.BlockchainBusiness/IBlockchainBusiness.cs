using System.Threading.Tasks;
using Vakapay.Models.Domains;
using Vakapay.Models.Repositories.Base;

namespace Vakapay.BlockchainBusiness
{
    public interface IBlockchainBusiness
    {
        ReturnObject SendTransaction(string From, string To, decimal amount);
        ReturnObject SendTransaction(string From, string To, decimal amount, string Password);
        ReturnObject SendMultiTransaction(string From, string[] To, decimal amount);
        ReturnObject SignData(string data, string privateKey);
        ReturnObject CreateNewAddress(string WalletId, string password = "");
        Task<ReturnObject> SendTransactionAsysn(string From, string To, decimal amount);
    }
}