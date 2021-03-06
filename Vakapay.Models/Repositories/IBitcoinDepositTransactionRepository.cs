﻿using System;
using System.Collections.Generic;
using Vakapay.Models.Entities.BTC;
using Vakapay.Models.Repositories.Base;

namespace Vakapay.Models.Repositories
{
    public interface IBitcoinDepositTransactionRepository : IRepositoryBlockchainTransaction<BitcoinDepositTransaction>, IDisposable
    {
        List<BitcoinDepositTransaction> FindWhere(BitcoinDepositTransaction objectTransaction);
        BitcoinDepositTransaction FindOneWhere(BitcoinDepositTransaction objectTransaction);
    }
}