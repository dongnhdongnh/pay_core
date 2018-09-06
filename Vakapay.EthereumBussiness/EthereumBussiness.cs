using Vakapay.Models.Domains;
using Vakapay.Models.Entities;
using Vakapay.Models.Repositories;

namespace Vakapay.EthereumBussiness
{
    public class EthereumBussiness
    {
        private readonly IVakapayRepositoryFactory vakapayRepositoryFactory;

        public EthereumBussiness(IVakapayRepositoryFactory _vakapayRepositoryFactory)
        {
            vakapayRepositoryFactory = _vakapayRepositoryFactory;
        }
        public ReturnObject SendTransaction(EthereumWithdrawTransaction blockchainTransaction)
        {
            return null;
        }
        
        /// <summary>
        /// call RPC Ethereum to make new address
        /// save address to database
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        public ReturnObject CreateNewAddAddress(string password)
        {
            return null;
        }
    }
}