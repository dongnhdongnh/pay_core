using System;
using System.Data;
using System.Threading.Tasks;
using NLog;
using Vakapay.BlockchainBusiness;
using Vakapay.BlockchainBusiness.Base;
using Vakapay.Commons.Helpers;
using Vakapay.Models.Domains;
using Vakapay.Models.Entities;
using Vakapay.Models.Repositories;

namespace Vakapay.BitcoinBusiness
{
    public class BitcoinBusiness : AbsBlockchainBusiness
    {
        public BitcoinBusiness(IVakapayRepositoryFactory vakapayRepositoryFactory, bool isNewConnection = true) :
            base(vakapayRepositoryFactory, isNewConnection)
        {
            // <summary>
        }

        public ReturnObject FakePendingTransaction(BitcoinWithdrawTransaction blockchainTransaction)
        {
            try
            {
                var bitcoinWithDrawRepo =
                    VakapayRepositoryFactory.GeBitcoinRawTransactionRepository(DbConnection);
                blockchainTransaction.Id = CommonHelper.GenerateUuid();
                blockchainTransaction.Status = Status.StatusPending;
                blockchainTransaction.CreatedAt = (int) CommonHelper.GetUnixTimestamp();
                blockchainTransaction.UpdatedAt = (int) CommonHelper.GetUnixTimestamp();
                return bitcoinWithDrawRepo.Insert(blockchainTransaction);
            }
            catch (Exception e)
            {
                return new ReturnObject
                {
                    Status = Status.StatusError,
                    Message = e.Message
                };
            }
        }
    }
}