using System.Data;
using Vakapay.Models.Domains;
using Vakapay.Models.Repositories;

namespace Vakapay.BlockchainBusiness
{
    public abstract class BlockchainBusiness
    {
        protected readonly IVakapayRepositoryFactory VakapayRepositoryFactory;
        protected  IDbConnection DbConnection { get; set; }
        
        public BlockchainBusiness(IVakapayRepositoryFactory _vakapayRepositoryFactory, bool isNewConnection = true)
        {
            VakapayRepositoryFactory = _vakapayRepositoryFactory;
            DbConnection = isNewConnection
                ? VakapayRepositoryFactory.GetDbConnection()
                : VakapayRepositoryFactory.GetOldConnection();
        }
        
        public ReturnObject SendTransaction(BlockchainTransaction blockchainTransaction)
        {
            return null;
        }
        
    }
}