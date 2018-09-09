using System.Data;
using Vakapay.Models.Domains;
using Vakapay.Models.Repositories;

namespace Vakapay.BlockchainBusiness
{
    public abstract class BlockchainBusiness
    {
        protected readonly IVakapayRepositoryFactory vakapayRepositoryFactory;
        protected  IDbConnection DbConnection { get; set; }
        
        public BlockchainBusiness(IVakapayRepositoryFactory _vakapayRepositoryFactory, bool isNewConnection = true)
        {
            vakapayRepositoryFactory = _vakapayRepositoryFactory;
            DbConnection = isNewConnection
                ? vakapayRepositoryFactory.GetDbConnection()
                : vakapayRepositoryFactory.GetOldConnection();
        }
        
        public ReturnObject SendTransaction(IBlockchainTransaction blockchainTransaction)
        {
            return null;
        }
        
    }
}