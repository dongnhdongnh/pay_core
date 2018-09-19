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
    public class BitcoinBusinessNew : AbsBlockchainBusiness, IBlockchainBusiness
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public BitcoinBusinessNew(IVakapayRepositoryFactory vakapayRepositoryFactory, bool isNewConnection = true) :
            base(vakapayRepositoryFactory, isNewConnection)
        {
            // <summary>
        }

        // <summary>
        // Returns a new bitcoin address for receiving payments.
        // If [account] is specified (recommended), it is added to the address book so payments 
        // received with the address will be credited to [account].
        // </summary>
        // <param name="rpcClass"></param>
        // <param name="walletId"></param>
        // <param name="account"></param>
        // bitcoin ver 16 GetNewAddress(Account); ver 17 GetNewAddress(Label)
        public ReturnObject CreateNewAddAddress(IBlockchainRPC rpcClass, string walletId, string account = "")
        {
            try
            {
                var walletRepository = VakapayRepositoryFactory.GetWalletRepository(DbConnection);

                var walletCheck = walletRepository.FindById(walletId);

                if (walletCheck == null)
                    return new ReturnObject
                    {
                        Status = Status.StatusError,
                        Message = "Wallet Not Found"
                    };

                var results = rpcClass.CreateNewAddress(account);
                if (results.Status == Status.StatusError)
                    return results;

                var address = results.Data;

                //add database vakaxa
                var bitcoinAddressRepo = VakapayRepositoryFactory.GetBitcoinAddressRepository(DbConnection);
                var time = (int) CommonHelper.GetUnixTimestamp();
                var bcAddress = new BitcoinAddress
                {
                    Id = CommonHelper.GenerateUuid(),
                    Address = address,
                    Status = Status.StatusActive,
                    WalletId = walletId,
                    CreatedAt = time,
                    UpdatedAt = time
                };

                var resultAddBitcoinAddress = bitcoinAddressRepo.Insert(bcAddress);
                //
                return resultAddBitcoinAddress;
            }
            catch (Exception e)
            {
                Logger.Error(e, "CreateNewAddAddress exception");
                return new ReturnObject
                {
                    Status = Status.StatusError,
                    Message = e.Message
                };
            }
        }


        public ReturnObject SendTransaction(string From, string To, decimal amount)
        {
            throw new NotImplementedException();
        }

        public ReturnObject SendTransaction(string From, string To, decimal amount, string Password)
        {
            throw new NotImplementedException();
        }

        public ReturnObject SendMultiTransaction(string From, string[] To, decimal amount)
        {
            throw new NotImplementedException();
        }

        public ReturnObject SignData(string data, string privateKey)
        {
            throw new NotImplementedException();
        }

        public ReturnObject CreateNewAddress(string WalletId, string password = "")
        {
            throw new NotImplementedException();
        }

        public Task<ReturnObject> SendTransactionAsysn(string From, string To, decimal amount)
        {
            throw new NotImplementedException();
        }
    }
}