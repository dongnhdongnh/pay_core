using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Vakapay.Commons.Helpers;
using Vakapay.Models.Domains;
using Vakapay.Models.Entities;
using Vakapay.Models.Repositories;

namespace Vakapay.WalletBusiness
{
    public class WalletBusiness
    {
        private readonly IVakapayRepositoryFactory vakapayRepositoryFactory;

        private readonly IDbConnection ConnectionDb;

        public WalletBusiness(IVakapayRepositoryFactory _vakapayRepositoryFactory, bool isNewConnection = true) 
        {
            vakapayRepositoryFactory = _vakapayRepositoryFactory;
            ConnectionDb = isNewConnection
                ? vakapayRepositoryFactory.GetDbConnection()
                : vakapayRepositoryFactory.GetOldConnection();
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
                
                if(ConnectionDb.State != ConnectionState.Open)
                    ConnectionDb.Open();
                var userRepository = vakapayRepositoryFactory.GetUserRepository(ConnectionDb);
                var userCheck = userRepository.FindById(user.Id);
                if(userCheck == null)
                    return new ReturnObject
                    {
                        Status = Status.StatusError,
                        Message = "User Not Found"
                    };
                /*//var ethereum = new EthereumBusiness.EthereumBusiness(vakapayRepositoryFactory);
                /*var resultMakeaddress = ethereum.CreateNewAddAddress();
                if (resultMakeaddress.Status == Status.StatusError)
                    return resultMakeaddress;#1#
                var address = resultMakeaddress.Data;*/
                var wallet = new Wallet
                {
                    Id = CommonHelper.GenerateUuid(),
                    Address = null,
                    Balance = 0,
                    Version = 0,
                    CreatedAt = 0,
                    NetworkName = blockchainNetwork.Name,
                    UpdatedAt = 0,
                    UserId = user.Id
                    
                };

                var walletRepo = vakapayRepositoryFactory.GetWalletRepository(ConnectionDb);

                var resultMakeWallet = walletRepo.Insert(wallet);
                return resultMakeWallet;

            }
            catch(Exception e)
            {
                throw e;

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
        /// <summary>
        /// This function will make withdraw from wallet
        /// </summary>
        /// <param name="wallet"></param>
        /// <param name="toAddress"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        public ReturnObject Withdraw(Wallet wallet, string toAddress, decimal amount)
        {
            /*
             * 1. Validate User status
             * 2. Validate Network status
             * 3. Validate amount
             * 4. Update Wallet Balance
             * 5. Make new transaction withdraw pending
             *
             *
             * 
             */
            return null;
        }

        public ReturnObject UpdateBalance(string toAddress, decimal addedBlance, string networkName)
        {
            Console.WriteLine("update blance for "+ toAddress + ": "+addedBlance);
            return new ReturnObject
            {
                Status = "Success",
                Message = ""
            };
        }

        public List<Wallet> GetAllWallet()
        {
            try
            {
                if(ConnectionDb.State != ConnectionState.Open)
                    ConnectionDb.Open();
                var walletRepository = vakapayRepositoryFactory.GetWalletRepository(ConnectionDb);
                
                var result = walletRepository.FindBySql("SELECT * FROM wallet");
                return result;
            }
            catch(Exception e)
            {
                Console.WriteLine(e);
                throw e;
            }
        }
        
        
        public bool CheckExistedAddress(String addr)
        {
            try
            {
                var wallet = FindByAddress(addr);
                if (wallet != null)
                    return true;
                else
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            } 
        }

        public Wallet FindByAddress(string addr)
        {
            try
            {
                if(ConnectionDb.State != ConnectionState.Open)
                    ConnectionDb.Open();
                var walletRepository = vakapayRepositoryFactory.GetWalletRepository(ConnectionDb);
                var result = walletRepository.FindBySql($"SELECT * FROM wallet WHERE Address = '{addr}'");

                if (result.Count > 0 && result.Any() )
                {
                    return result[0];
                }               
                return null;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }
    }
}