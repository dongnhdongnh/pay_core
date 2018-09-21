using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using NLog;
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

        private static Logger logger = LogManager.GetCurrentClassLogger();

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
                if (ConnectionDb.State != ConnectionState.Open)
                {
                    ConnectionDb.Open();
                }
                var userRepository = vakapayRepositoryFactory.GetUserRepository(ConnectionDb);
                var userCheck = userRepository.FindById(user.Id);
                if (userCheck == null)
                {
                    return new ReturnObject
                    {
                        Status = Status.StatusError,
                        Message = "User Not Found"
                    };
                }
                
                var walletRepo = vakapayRepositoryFactory.GetWalletRepository(ConnectionDb);
                var existUserNetwork =
                    walletRepo.FindByUserAndNetwork(user.Id,
                        blockchainNetwork.Name);
                if (existUserNetwork != null)
                {
                    return new ReturnObject
                    {
                        Status = Status.StatusError,
                        Message = "User with NetworkName have already existed"
                    };
                }   
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
                    CreatedAt = (int) CommonHelper.GetUnixTimestamp(),
                    NetworkName = blockchainNetwork.Name,
                    UpdatedAt = (int) CommonHelper.GetUnixTimestamp(),
                    UserId = user.Id
                };
                var resultMakeWallet = walletRepo.Insert(wallet);
                return resultMakeWallet;
            }
            catch (Exception e)
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
            
            try
            {
                if (ConnectionDb.State != ConnectionState.Open)
                {
                    ConnectionDb.Open();
                }
                
                var walletRepository = vakapayRepositoryFactory.GetWalletRepository(ConnectionDb);
                var userRepository =
                    vakapayRepositoryFactory.GetUserRepository(ConnectionDb);
                var etherWithdrawTransaction =
                    vakapayRepositoryFactory.GetEthereumWithdrawTransactionRepository(ConnectionDb);
                var btcWithdrawTransaction =
                    vakapayRepositoryFactory.GeBitcoinRawTransactionRepository(ConnectionDb);
                var vakaWithdrawTransaction =
                    vakapayRepositoryFactory.GetVakacoinWithdrawTransactionRepository(ConnectionDb);
                

                //Validate User status
                //and validate Network status
                var walletById = walletRepository.FindById(wallet.Id);
                var walletByAddress = walletRepository.FindByAddress(toAddress);

                if (walletByAddress == null || walletById == null)
                {
                    return new ReturnObject
                    {
                        Status = Status.StatusError,
                        Message = "Address not exists"
                    };
                }
                
                var userCheck = userRepository.FindById(walletById.UserId);
                if (userCheck == null || 
                    userCheck.Status != Status.StatusActive || 
                    !walletById.NetworkName.Equals(walletByAddress.NetworkName))
                    return new ReturnObject
                    {
                        Status = Status.StatusError,
                        Message = "User Not Found || Not Active || Not same Network"
                    };
                
                // Validate amount
                if (walletById.Balance < amount)
                {
                    return new ReturnObject()
                    {
                        Status = Status.StatusError,
                        Message = "Can't transfer bigger than wallet balance"
                    };
                }

                //Update Wallet Balance
                walletById.Balance -= amount;
                walletById.UpdatedAt = (int) CommonHelper.GetUnixTimestamp();
                var updateWallet = walletRepository.Update(walletById);
                if (updateWallet == null || updateWallet.Status == Status.StatusError)
                {
                    return new ReturnObject()
                    {
                        Status = Status.StatusError,
                        Message = "Fail update balance in walletDB"
                    };
                }


                //Make new transaction withdraw pending by
                //insert into ethereumwithdrawtransaction database
                if (walletById.NetworkName.Equals(NetworkName.ETH))
                {
                    var etherWithdraw = new EthereumWithdrawTransaction()
                    {
                        Id = CommonHelper.GenerateUuid(),
                        Status = Status.StatusPending,
                        FromAddress = walletById.Address,
                        ToAddress = toAddress,
                        Amount = amount,
                        CreatedAt = CommonHelper.GetUnixTimestamp(),
                        UpdatedAt = CommonHelper.GetUnixTimestamp(),
                        NetworkName = NetworkName.ETH,
                        InProcess = 0,
                        Version = 0
                    };
                    var insertWithdraw = etherWithdrawTransaction.Insert(etherWithdraw);
                    if (insertWithdraw == null ||
                        insertWithdraw.Status == Status.StatusError)
                    {
                        return new ReturnObject()
                        {
                            Status = Status.StatusError,
                            Message = "Fail insert to ethereumwithdrawtransaction"
                        };
                    }
                }
                
                if (walletById.NetworkName.Equals(NetworkName.BTC))
                {
                    var btcWithdraw = new BitcoinWithdrawTransaction()
                    {
                        Id = CommonHelper.GenerateUuid(),
                        Status = Status.StatusPending,
                        FromAddress = walletById.Address,
                        ToAddress = toAddress,
                        Amount = amount,
                        CreatedAt = CommonHelper.GetUnixTimestamp(),
                        UpdatedAt = CommonHelper.GetUnixTimestamp(),
                        NetworkName = NetworkName.BTC,
                        InProcess = 0,
                        Version = 0
                    };
                    var insertWithdraw = btcWithdrawTransaction.Insert(btcWithdraw);
                    if (insertWithdraw == null ||
                        insertWithdraw.Status == Status.StatusError)
                    {
                        return new ReturnObject()
                        {
                            Status = Status.StatusError,
                            Message = "Fail insert to BitcoinWithdrawTransaction"
                        };
                    }
                }
                
                if (walletById.NetworkName.Equals(NetworkName.VAKA))
                {
                    var vakaWithdraw = new VakacoinWithdrawTransaction()
                    {
                        Id = CommonHelper.GenerateUuid(),
                        Status = Status.StatusPending,
                        FromAddress = walletById.Address,
                        ToAddress = toAddress,
                        Amount = amount,
                        CreatedAt = CommonHelper.GetUnixTimestamp(),
                        UpdatedAt = CommonHelper.GetUnixTimestamp(),
                        NetworkName = NetworkName.VAKA,
                        InProcess = 0,
                        Version = 0
                    };
                    var insertWithdraw = vakaWithdrawTransaction.Insert(vakaWithdraw);
                    if (insertWithdraw == null ||
                        insertWithdraw.Status == Status.StatusError)
                    {
                        return new ReturnObject()
                        {
                            Status = Status.StatusError,
                            Message = "Fail insert to VakaWithdrawTransaction"
                        };
                    }
                }

                return null;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        
        public ReturnObject UpdateAddressForWallet(string walletId, string address)
        {
            try
            {
                var walletRepository = vakapayRepositoryFactory.GetWalletRepository(ConnectionDb);
                var whereUpdateAddr = walletRepository.FindById(walletId);

                //update address for walletId
                whereUpdateAddr.Address = address;
                whereUpdateAddr.UpdatedAt = (int) CommonHelper.GetUnixTimestamp();
                var walletUpdate = walletRepository.Update(whereUpdateAddr);
                if (walletUpdate.Status == Status.StatusError)
                    return new ReturnObject
                    {
                        Status = Status.StatusError,
                        Message = "Update wallet address fail"
                    };
                return new ReturnObject
                {
                    Status = Status.StatusSuccess,
                    Message = "Add address to wallet complete"
                };
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
        
        public ReturnObject UpdateBalance(string toAddress, decimal addedBlance, string networkName)
        {
            try
            {
                if (ConnectionDb.State != ConnectionState.Open)
                    ConnectionDb.Open();
                var walletRepository = vakapayRepositoryFactory.GetWalletRepository(ConnectionDb);
                var wallet = walletRepository.FindByAddress(toAddress);
                if (wallet == null)
                {
                    return new ReturnObject
                    {
                        Status = Status.StatusError,
                        Message = "Update fail, Address not exists"
                    };
                }

                if (!wallet.NetworkName.Equals(networkName))
                {
                    return new ReturnObject
                    {
                        Status = Status.StatusError,
                        Message = "Not same network"
                    };
                }

                wallet.Balance += addedBlance;
                wallet.Version += 1;
                wallet.UpdatedAt = (int) CommonHelper.GetUnixTimestamp();
                var result = walletRepository.Update(wallet);
                return result;
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

        public List<Wallet> GetAllWallet()
        {
            try
            {
                if (ConnectionDb.State != ConnectionState.Open)
                    ConnectionDb.Open();
                var walletRepository = vakapayRepositoryFactory.GetWalletRepository(ConnectionDb);

                var result = walletRepository.FindBySql("SELECT * FROM wallet");
                return result;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw e;
            }
        }

        public bool CheckExistedAddress(String addr)
        {
            try
            {
                if (ConnectionDb.State != ConnectionState.Open)
                    ConnectionDb.Open();
                var walletRepository = vakapayRepositoryFactory.GetWalletRepository(ConnectionDb);
                var wallet = walletRepository.FindByAddress(addr);
                if (wallet != null)
                    return true;

                return false;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
        }

        // check wallet exists or not. Then update balance for this
        public bool CheckExistedAndUpdateByAddress(string addr, decimal amount, string networkName)
        {
            try
            {
                if (ConnectionDb.State != ConnectionState.Open)
                    ConnectionDb.Open();
                var walletRepository = vakapayRepositoryFactory.GetWalletRepository(ConnectionDb);
                var wallet = walletRepository.FindByAddressAndNetworkName(addr, networkName).SingleOrDefault();

                //check existed
                if (wallet == null)
                    return false;

                var id = wallet.Id;
                var version = wallet.Version;

                var result = UpdateBalance(amount, id, version);
                if (result.Status == Status.StatusSuccess)
                    return true;

                // can't update Balance
                logger.Error("Error when update Balance: Address = " + addr + "; Amount = " + amount + "; NetworkName = " + networkName);
                return false;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
        }

        private ReturnObject UpdateBalance(decimal amount, string id, int version)
        {
            try
            {
                if (ConnectionDb.State != ConnectionState.Open)
                    ConnectionDb.Open();
                var walletRepository = vakapayRepositoryFactory.GetWalletRepository(ConnectionDb);
                var result = walletRepository.UpdateBalanceWallet(amount, id, version);

                return result;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }
    }
}