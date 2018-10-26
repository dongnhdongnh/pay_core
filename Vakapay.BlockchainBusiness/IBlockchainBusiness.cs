using System.Threading.Tasks;
using Vakapay.Models.Domains;

namespace Vakapay.BlockchainBusiness
{
    public interface IBlockchainBusiness
    {
        ReturnObject SendTransaction(string from, string to, decimal amount);
        ReturnObject SendTransaction(string from, string to, decimal amount, string password);
        ReturnObject SendMultiTransaction(string from, string[] to, decimal amount);
        ReturnObject SignData(string data, string privateKey);
        ReturnObject CreateNewAddress(string walletId, string password = "");
        Task<ReturnObject> SendTransactionAsysn(string from, string to, decimal amount);
    }
}