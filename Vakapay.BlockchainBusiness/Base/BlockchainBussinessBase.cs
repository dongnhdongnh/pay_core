using System;
using System.Data;
using Vakapay.Models.Repositories;

namespace Vakapay.BlockchainBusiness.Base
{
    public class BlockchainBussinessBase<T> : IDisposable
    {
        public IVakapayRepositoryFactory vakapayRepositoryFactory { get; }
        public IDbConnection DbConnection { get; }

        public BlockchainBussinessBase(IVakapayRepositoryFactory _vakapayRepositoryFactory, bool isNewConnection = true)
        {
            vakapayRepositoryFactory = _vakapayRepositoryFactory;
            DbConnection = isNewConnection
                ? vakapayRepositoryFactory.GetDbConnection()
                : vakapayRepositoryFactory.GetOldConnection();
        }


        public void Dispose()
        {
        }
    }
}