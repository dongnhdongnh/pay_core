using System.Collections.Generic;
using Vakapay.Models.Entities;
using Vakapay.Models.Repositories.Base;

namespace Vakapay.Models.Repositories
{
    public interface IBitcoinDepositTransactioRepository : IRepositoryBlockchainTransaction<BitcoinDepositTransaction>
    {
        List<BitcoinDepositTransaction> FindWhere(BitcoinDepositTransaction objectTransaction);
        BitcoinDepositTransaction FindOneWhere(BitcoinDepositTransaction objectTransaction);
    }
}