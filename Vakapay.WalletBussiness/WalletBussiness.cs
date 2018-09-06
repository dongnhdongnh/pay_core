using System;
using System.Collections.Generic;
using System.Data;
using Vakapay.Models.Domains;
using Vakapay.Models.Entities;
using Vakapay.Models.Repositories;

namespace Vakapay.WalletBussiness
{
    public class WalletBussiness
    {
        private readonly IVakapayRepositoryFactory vakapayRepositoryFactory;

        public WalletBussiness(IVakapayRepositoryFactory _vakapayRepositoryFactory)
        {
            vakapayRepositoryFactory = _vakapayRepositoryFactory;
        }
        
        /// <summary>
        /// //Find All Blockchain network active
        /// each Blockchain network, make one wallet
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public ReturnObject MakeAllWalletForNewUser(User user)
        {
            return null;
        }
        
        /// <summary>
        /// make new wallet for user
        /// </summary>
        /// <param name="user"></param>
        /// <param name="blockchainNetwork"></param>
        /// <returns></returns>
        public ReturnObject CreateNewWallet(User user, BlockchainNetwork blockchainNetwork)
        {
            //validate user already have wallet or not
            //Call Blockchain bussiness for create new address
            //save to address and wallet
            //commit transaction
            try
            {
                var dbConnection = vakapayRepositoryFactory.GetDbConnection();
                if(dbConnection.State != ConnectionState.Open)
                    dbConnection.Open();
                var userRepository = vakapayRepositoryFactory.GetUserRepository(dbConnection);
                var userCheck = userRepository.FindById(user.Id);
                if(userCheck == null)
                    return new ReturnObject
                    {
                        Status = "Error",
                        Message = "User Not Found"
                    };
                var ethereum = new EthereumBussiness.EthereumBussiness(vakapayRepositoryFactory);
                var resultMakeaddress = ethereum.CreateNewAddAddress("random_password");
                if (resultMakeaddress.Status == Status.StatusError)
                    return resultMakeaddress;
                var address = resultMakeaddress.Data;
                var wallet = new Wallet
                {
                    Id = "",
                    Address = address,
                    Balance = 0,
                    Version = 0,
                    CreatedAt = 0,
                    NetworkName = blockchainNetwork.Name,
                    UpdatedAt = 0,
                    UserId = user.Id
                    
                };

                var walletRepo = vakapayRepositoryFactory.GetWalletRepository(dbConnection);

                var resultMakeWallet = walletRepo.Insert(wallet);
                return resultMakeWallet;

            }
            catch(Exception e)
            {
                return new ReturnObject
                {
                    Status = Status.StatusError,
                    Message = e.Message
                };
            }

            
            
            //return null;
        }
        
        /// <summary>
        /// Find all wallet of user
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public ReturnObject LoadAllWalletByUser(User user)
        {
            //find wallet by usser
            return null;
        }

        public List<T> FindTransactionWallet<T>(User user, string networkName, int limit, int page, string orderBy)
        {
            return new List<T>();
        }
    }
}