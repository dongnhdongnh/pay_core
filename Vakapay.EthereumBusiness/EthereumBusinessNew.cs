using System.Threading.Tasks;
using Vakapay.BlockchainBusiness;
using Vakapay.BlockchainBusiness.Base;
using Vakapay.Models.Domains;
using Vakapay.Models.Repositories;

namespace Vakapay.EthereumBusiness
{
    public class EthereumBusinessNew : AbsBlockchainBusiness , IBlockchainBusiness
    {
        public EthereumBusinessNew(IVakapayRepositoryFactory vakapayRepositoryFactory, bool isNewConnection = true) : base(vakapayRepositoryFactory, isNewConnection)
        {
        }

        public ReturnObject SendTransaction(string From, string To, decimal amount)
        {
            throw new System.NotImplementedException();
        }

        public ReturnObject SendTransaction(string From, string To, decimal amount, string Password)
        {
            throw new System.NotImplementedException();
        }

        public ReturnObject SendMultiTransaction(string From, string[] To, decimal amount)
        {
            throw new System.NotImplementedException();
        }

        public ReturnObject SignData(string data, string privateKey)
        {
            throw new System.NotImplementedException();
        }

        public ReturnObject CreateNewAddress(string WalletId, string password = "")
        {
            throw new System.NotImplementedException();
        }

        public Task<ReturnObject> SendTransactionAsysn(string From, string To, decimal amount)
        {
            throw new System.NotImplementedException();
        }
    }
}