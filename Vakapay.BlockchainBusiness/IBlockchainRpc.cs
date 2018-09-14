using Vakapay.Models.Domains;

namespace Vakapay.BlockchainBusiness
{
    public interface IBlockchainRpc
    {
        string endpointRpc { get; set; }

        ReturnObject CreateNewAddress(string password);
        ReturnObject CreateNewAddress();
        ReturnObject CreateNewAddress(string privateKey, string publicKey);
        ReturnObject SendTransaction(string data);
        ReturnObject GetBalance(string address);
        ReturnObject SignTransaction(string privateKey, object[] transactionData);
        
    }
}