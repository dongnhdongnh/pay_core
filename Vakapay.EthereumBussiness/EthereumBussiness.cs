using System;
using System.Data;
using Vakapay.Commons.Helpers;
using Vakapay.Models.Domains;
using Vakapay.Models.Entities;
using Vakapay.Models.Repositories;

namespace Vakapay.EthereumBussiness
{
    public class EthereumBussiness
    {
        private readonly IVakapayRepositoryFactory vakapayRepositoryFactory;
        private EthereumRpc ethereumRpc { get; set; }
        private  IDbConnection DbConnection { get; set; }

        public EthereumBussiness(IVakapayRepositoryFactory _vakapayRepositoryFactory, bool isNewConnection = true)
        {
            vakapayRepositoryFactory = _vakapayRepositoryFactory;
            ethereumRpc = new EthereumRpc("http://endpoint");
            DbConnection = isNewConnection
                ? vakapayRepositoryFactory.GetDbConnection()
                : vakapayRepositoryFactory.GetOldConnection();
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
        public ReturnObject CreateNewAddAddress(string walletId)
        {
            try
            {
                string password = CommonHelper.RandomString(15);
                var ResultMakeAddress = ethereumRpc.CreateAddress(password);
                if(ResultMakeAddress.Status == Status.StatusError)
                    return ResultMakeAddress;
               

                var ethereumAddressRepo = vakapayRepositoryFactory.GetEthereumAddressRepository(DbConnection);
                
                //TODO Encrypt Password Before save
                var ResultAddEthereumAddress = ethereumAddressRepo.Insert(new EthereumAddress
                {
                    Status = Status.StatusActive,
                    Address = ResultMakeAddress.Data,
                    CreatedAt = (int)CommonHelper.GetUnixTimestamp(),
                    Id = CommonHelper.GenerateUuid(),
                    Password = password,
                    UpdatedAt = (int)CommonHelper.GetUnixTimestamp(),
                    WalletId = walletId
                    
                });

                return ResultAddEthereumAddress;


            }
            catch (Exception e)
            {
                return new ReturnObject
                {
                    Status = Status.StatusError,
                    Message = e.Message
                };
                ;
            }
            
        }
    }
}