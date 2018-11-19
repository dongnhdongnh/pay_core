using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using Newtonsoft.Json.Linq;
using NLog;
using NLog.Fluent;
using Vakapay.BitcoinBusiness;
using Vakapay.BlockchainBusiness;
using Vakapay.Commons.Constants;
using Vakapay.Commons.Helpers;
using Vakapay.Cryptography;
using Vakapay.EthereumBusiness;
using Vakapay.Models.Domains;
using Vakapay.Models.Entities;
using Vakapay.Models.Entities.BTC;
using Vakapay.Models.Entities.ETH;
using Vakapay.Models.Entities.VAKA;
using Vakapay.Models.Repositories;
using Vakapay.Repositories.Mysql;
using Vakapay.VakacoinBusiness;

namespace Vakapay.WalletBusiness
{
    public class WalletBusiness : IWalletBusiness
    {
        EthereumBusiness.EthereumBusiness ethereumBussiness;
        BitcoinBusiness.BitcoinBusiness bitcoinBussiness;
        VakacoinBusiness.VakacoinBusiness vakacoinBussiness;
        SendMailBusiness.SendMailBusiness sendMailBusiness;
        UserBusiness.UserBusiness userBusiness;
        PortfolioHistoryBusiness.PortfolioHistoryBusiness portfolioHistoryBusiness;
        private readonly IVakapayRepositoryFactory _vakapayRepositoryFactory;

        private readonly IDbConnection _connectionDb;

        private static Logger logger = LogManager.GetCurrentClassLogger();

        public WalletBusiness(IVakapayRepositoryFactory vakapayRepositoryFactory, bool isNewConnection = true)
        {
            _vakapayRepositoryFactory = vakapayRepositoryFactory;
            _connectionDb = isNewConnection
                ? _vakapayRepositoryFactory.GetDbConnection()
                : _vakapayRepositoryFactory.GetOldConnection();

            ethereumBussiness = new EthereumBusiness.EthereumBusiness(vakapayRepositoryFactory, false);
            bitcoinBussiness = new BitcoinBusiness.BitcoinBusiness(vakapayRepositoryFactory, false);
            vakacoinBussiness = new VakacoinBusiness.VakacoinBusiness(vakapayRepositoryFactory, false);
            sendMailBusiness = new SendMailBusiness.SendMailBusiness(_vakapayRepositoryFactory, false);
            userBusiness = new UserBusiness.UserBusiness(_vakapayRepositoryFactory, false);
            portfolioHistoryBusiness =
                new PortfolioHistoryBusiness.PortfolioHistoryBusiness(_vakapayRepositoryFactory, false);
            //(PortfolioHistoryBusiness.PortfolioHistoryBusiness)_vakapayRepositoryFactory.GetPortfolioHistoryRepository(_connectionDb);
        }

        /// <summary>
        /// //Find All Blockchain network active
        /// each Blockchain network, make one wallet
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public ReturnObject MakeAllWalletForNewUser(User user)
        {
            try
            {
                foreach (string blockchainName in CryptoCurrency.ALL_NETWORK)
                {
                    ReturnObject result = CreateNewWallet(user, blockchainName);
                    Console.WriteLine("Call "+user.Id+"_" + blockchainName);
                    if (result.Status == Status.STATUS_ERROR)
                    {
                      //  Console.WriteLine("ERROR" + JsonHelper.SerializeObject(result));
                       // return result;
                        continue;
                    }
                }

                return new ReturnObject
                {
                    Status = Status.STATUS_SUCCESS,
                    Message = "Create all wallet done"
                };
            }
            catch (Exception e)
            {
                return new ReturnObject
                {
                    Status = Status.STATUS_ERROR,
                    Message = e.Message
                };
            }
        }

        /// <summary>
        /// make new wallet for user
        /// </summary>
        /// <param name="user"></param>
        /// <param name="blockchainNetwork"></param>
        /// <returns></returns>
        public ReturnObject CreateNewWallet(User user, string blockchainNetwork)
        {
            //validate user already have wallet or not
            //Call Blockchain bussiness for create new address
            //save to address and wallet
            //commit transaction
            try
            {
                if (_connectionDb.State != ConnectionState.Open)
                {
                    _connectionDb.Open();
                }

                //                if (checkExisted)
                //                {
                //                    var userRepository = vakapayRepositoryFactory.GetUserRepository(ConnectionDb);
                //                    var userCheck = userRepository.FindById(user.Id);
                //                    if (userCheck == null)
                //                    {
                //                        return new ReturnObject
                //                        {
                //                            Status = Status.StatusError,
                //                            Message = "User Not Found"
                //                        };
                //                    }
                //                }

                var walletRepo = _vakapayRepositoryFactory.GetWalletRepository(_connectionDb);
                var existUserNetwork =
                    walletRepo.FindByUserAndNetwork(user.Id,
                        blockchainNetwork);
                if (existUserNetwork != null)
                {
                    Console.WriteLine("existed "+user.Id +"_ "+blockchainNetwork);
                    return new ReturnObject
                    {
                        Status = Status.STATUS_ERROR,
                        Message = "User with NetworkName have already existed:" +
                                  JsonHelper.SerializeObject(existUserNetwork)
                    };
                }
                /*//var ethereum = new EthereumBusiness.EthereumBusiness(vakapayRepositoryFactory);
                /*var resultMakeaddress = ethereum.CreateNewAddAddress();
                if (resultMakeaddress.Status == Status.StatusError)
                    return resultMakeaddress;#1#
                var address = resultMakeaddress.Data;*/

                var wallet = new Wallet
                {
                    Balance = 0,
                    IsProcessing = 0,
                    Status = Status.STATUS_PENDING,
                    Version = 0,
                    Currency = blockchainNetwork,
                    UserId = user.Id
                };
                Console.WriteLine("create wallet "+user.Id+"_"+ blockchainNetwork);
                var resultMakeWallet = walletRepo.Insert(wallet);
                return resultMakeWallet;
            }
            catch (Exception e)
            {
                return new ReturnObject
                {
                    Status = Status.STATUS_ERROR,
                    Message = e.Message
                };
            }

            //return null;
        }

        public bool CheckExistedAddress(string toAddress)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Find all wallet of user
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public List<Wallet> LoadAllWalletByUser(User user)
        {
            //find wallet by usser
            try
            {
                if (_connectionDb.State != ConnectionState.Open)
                    _connectionDb.Open();
                var walletRepository = _vakapayRepositoryFactory.GetWalletRepository(_connectionDb);
                var wallet = walletRepository.FindAllWalletByUser(user);
                if (wallet != null)
                    return wallet;

                return null;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
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
        public ReturnObject Withdraw(Wallet wallet, string toAddress, decimal amount, decimal pricePerCoin, string description = null)
        {
            /*
             * 1. Validate User status
             * 2. Validate Network status
             * 3. Validate toAddress
             * 4. Validate amount
             * 5. Update Wallet Balance
             * 6. Make new transaction withdraw pending
             *
             *
             *
             */

            try
            {
                if (wallet == null)
                {
                    logger.Error("Wallet input is null");
                    return new ReturnObject()
                    {
                        Status = Status.STATUS_ERROR,
                        Message = "Wallet input is null"
                    };
                }

                if (_connectionDb.State != ConnectionState.Open)
                {
                    _connectionDb.Open();
                }

                var fromAddress = GetSenderAddress(wallet, toAddress, amount);

                if (string.IsNullOrEmpty(fromAddress))
                {
                    logger.Error("Can not get sender address!");
                    return new ReturnObject()
                    {
                        Status = Status.STATUS_ERROR,
                        Message = "Can not get sender address!"
                    };
                }

                var walletRepository = _vakapayRepositoryFactory.GetWalletRepository(_connectionDb);
                var userRepository = _vakapayRepositoryFactory.GetUserRepository(_connectionDb);

                // 1. Validate User status
                var walletById = walletRepository.FindById(wallet.Id);

                if (walletById == null)
                {
                    return new ReturnObject()
                    {
                        Status = Status.STATUS_ERROR,
                        Message = "User wallet found in data base"
                    };
                }

                var userCheck = userRepository.FindById(walletById.UserId);
                if (userCheck == null ||
                    userCheck.Status != Status.STATUS_ACTIVE // ||
                                                             // !walletById.Currency.Equals(walletByAddress.Currency))
                )
                {
                    return new ReturnObject
                    {
                        Status = Status.STATUS_ERROR,
                        Message = "User Not Found || Not Active" // || Not same Network"
                    };
                }

                //                // 2. validate Network status ==> not validate Network status as check if node is running,
                //                var validateNetworks = ValidateNetworkStatus(wallet.Currency);
                //                if (validateNetworks.Status == Status.STATUS_ERROR)
                //                {
                //                    return validateNetworks;
                //                }

                // 3. Validate toAddress
                if (ValidateAddress(toAddress, wallet.Currency) == false)
                {
                    return new ReturnObject
                    {
                        Status = Status.STATUS_ERROR,
                        Message = wallet.Currency + ": To Address is not valid!"
                    };
                }

                // 4. Validate amount
                var free = GetFee(wallet.Currency);

                if (walletById.Balance < amount + free)
                {
                    return new ReturnObject()
                    {
                        Status = Status.STATUS_ERROR,
                        Message = "Can't transfer bigger than wallet balance"
                    };
                }

                //                var withdrawTrx = ConnectionDb.BeginTransaction();
                /*
                 * Should we BeginTransaction a transaction here and rollback if error happens?
                 * But it is dangerous because if insert task is slow and user can double send their balance before
                 * transaction being committed.
                 */

                // 5. Update Wallet Balance
                var updateWallet = UpdateBalance(-(amount + free), wallet.Id, wallet.Version);
                if (updateWallet == null || updateWallet.Status == Status.STATUS_ERROR)
                {
                    //                    withdrawTrx.Rollback();
                    return new ReturnObject()
                    {
                        Status = Status.STATUS_ERROR,
                        Message = "Fail update balance in walletDB"
                    };
                }

                // double check balance of wallet is valid: check after update balance
                walletById = walletRepository.FindById(wallet.Id);
                if (walletById.Balance < 0)
                {
                    return new ReturnObject()
                    {
                        Status = Status.STATUS_ERROR,
                        Message = $"Balance in {walletById.Currency} wallet after UpdateBalance is smaller than 0!!"
                    };
                }


                //Make new transaction withdraw pending by
                var insertWithdraw = InsertToWithdrawTable(new BlockchainTransaction
                {
                    UserId = walletById.UserId,
                    FromAddress = fromAddress,
                    ToAddress = toAddress,
                    Fee = free,
                    Amount = amount,
                    PricePerCoin = pricePerCoin,
                    Description = description
                }, walletById.Currency);

                return insertWithdraw;
            }
            catch (Exception e)
            {
                logger.Error(e.Message);
                return new ReturnObject
                {
                    Status = Status.STATUS_ERROR,
                    Message = e.Message
                };
            }
        }

        private ReturnObject InsertToWithdrawTable(BlockchainTransaction blockchainTransaction, string currency)
        {
            switch (currency)
            {
                case CryptoCurrency.BTC:
                    var btcWithdrawTransaction =
                        _vakapayRepositoryFactory.GetBitcoinWithdrawTransactionRepository(_connectionDb);
                    return btcWithdrawTransaction.Insert(
                        blockchainTransaction.ToDelivered<BitcoinWithdrawTransaction>());

                case CryptoCurrency.ETH:
                    var etherWithdrawTransaction =
                        _vakapayRepositoryFactory.GetEthereumWithdrawTransactionRepository(_connectionDb);
                    return etherWithdrawTransaction.Insert(blockchainTransaction
                        .ToDelivered<EthereumWithdrawTransaction>());

                case CryptoCurrency.VAKA:
                    var vakaWithdrawTransaction =
                        _vakapayRepositoryFactory.GetVakacoinWithdrawTransactionRepository(_connectionDb);
                    return vakaWithdrawTransaction.Insert(
                        blockchainTransaction.ToDelivered<VakacoinWithdrawTransaction>());

                default:
                    return new ReturnObject()
                    {
                        Status = Status.STATUS_ERROR,
                        Message = "Currency not defined!"
                    };
            }
        }

        private ReturnObject ValidateNetworkStatus(string walletNetworkName)
        {
            //            throw new NotImplementedException();

            /*
             * 1. Validate Bitcoin Network Status
             * 2. Validate Ethereum Network Status
             * 3. Validate Vakacoin Network Status
             *
             * Return network error in result.Message
             */

            // 3. Validate Vakacoin Network Status

            try
            {
                ReturnObject getInfoResult;
                switch (walletNetworkName)
                {
                    case CryptoCurrency.BTC:
                        var bitcoinRpc = new BitcoinRpc(AppSettingHelper.GetBitcoinNode(),
                            AppSettingHelper.GetBitcoinRpcAuthentication());

                        getInfoResult = bitcoinRpc.GetInfo();

                        if (getInfoResult.Status == Status.STATUS_ERROR)
                        {
                            return new ReturnObject()
                            {
                                Status = Status.STATUS_ERROR,
                                Message = "Bitcoin network error: " + getInfoResult.Message
                            };
                        }

                        break;

                    case CryptoCurrency.ETH:
                        var ethRpc = new EthereumRpc(AppSettingHelper.GetEthereumNode());
                        var blockNumber = ethRpc.GetBlockNumber();

                        if (blockNumber.Status == Status.STATUS_ERROR)
                        {
                            return new ReturnObject()
                            {
                                Status = Status.STATUS_ERROR,
                                Message = "Ethereum network error: " + blockNumber.Message
                            };
                        }

                        break;

                    case CryptoCurrency.VAKA:
                        var vakacoinRpc = new VakacoinRpc(AppSettingHelper.GetVakacoinNode());
                        getInfoResult = vakacoinRpc.GetInfo();

                        if (getInfoResult.Status == Status.STATUS_ERROR)
                        {
                            return new ReturnObject()
                            {
                                Status = Status.STATUS_ERROR,
                                Message = "Vakacoin network error: " + getInfoResult.Message
                            };
                        }

                        break;
                    default:
                        return new ReturnObject()
                        {
                            Status = Status.STATUS_ERROR,
                            Message = "Undefined network name!"
                        };
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            return new ReturnObject()
            {
                Status = Status.STATUS_SUCCESS
            };
        }

        /// <summary>
        /// Get Fee: return miner fee plus service fee
        /// </summary>
        /// <param name="walletNetworkName"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        private decimal GetFee(string walletNetworkName)
        {
            // throw new NotImplementedException(); //TODO  must implement
            switch (walletNetworkName)
            {
                // TODO fake:
                case CryptoCurrency.BTC:
                    return (decimal)0.0005;
                case CryptoCurrency.ETH:
                    return (decimal)0.0005;
                case CryptoCurrency.VAKA:
                    return 0;
                default:
                    throw new Exception("Undefined network name!");
            }
        }

        /// <summary>
        /// Get sender: TODO This function will return a master wallet address or account to send coin to user request address
        /// </summary>
        /// <param name="wallet"></param>
        /// <param name="toAddress"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        private string GetSenderAddress(Wallet wallet, string toAddress, decimal amount)
        {
            //            throw new NotImplementedException(); //TODO  must implement
            //TODO fake
            return GetAddresses(wallet.Id, wallet.Currency)[0];
        }

        public Wallet FindWalletByAddressAndNetworkName(string Address, string networkName)
        {
            if (_connectionDb.State != ConnectionState.Open)
                _connectionDb.Open();
            var walletRepository = _vakapayRepositoryFactory.GetWalletRepository(_connectionDb);
            var wallet = walletRepository.FindByAddressAndNetworkName(Address, networkName);
            return wallet;
        }

        public ReturnObject UpdateBalanceDeposit(string toAddress, decimal addedBalance, string networkName)
        {
            try
            {
                //  userID = "";
                if (_connectionDb.State != ConnectionState.Open)
                    _connectionDb.Open();
               
                var wallet = FindByAddressAndNetworkName(toAddress, networkName);

                if (wallet == null)
                {
                    return new ReturnObject
                    {
                        Status = Status.STATUS_ERROR,
                        Message = "Update fail, Address not exists"
                    };
                }

                if (!wallet.Currency.Equals(networkName))
                {
                    return new ReturnObject
                    {
                        Status = Status.STATUS_ERROR,
                        Message = "Not same network"
                    };
                }

                //                wallet.Balance += addedBalance;
                //                wallet.Version += 1;
                //                var result = walletRepository.Update(wallet);
                var result = UpdateBalance(addedBalance, wallet.Id, wallet.Version);
                if (result.Status == Status.STATUS_ERROR)
                {
                    return result;
                }
                else
                {
                    User user = userBusiness.GetUserById(wallet.UserId);
                    //  userID = user.Id;
                    if (user != null)
                    {
                        portfolioHistoryBusiness.InsertWithPrice(user.Id);
                        //send mail:
                        EmailQueue email = new EmailQueue()
                        {
                            ToEmail = user.Email,
                            NetworkName = wallet.Currency,
                            Amount = addedBalance,
                            Template = EmailTemplate.Received,
                            Subject = EmailConfig.SUBJECT_SENT_OR_RECEIVED,
                            //							Content = networkName + "+" + addedBlance
                        };
                        result = sendMailBusiness.CreateEmailQueueAsync(email).Result;
                    }
                }

                return result;
            }
            catch (Exception e)
            {
                //     userID = "";
                return new ReturnObject
                {
                    Status = Status.STATUS_ERROR,
                    Message = e.Message
                };
            }
        }

        public Wallet GetWalletById(String id)
        {
            try
            {
                if (_connectionDb.State != ConnectionState.Open)
                    _connectionDb.Open();
                var walletRepository = _vakapayRepositoryFactory.GetWalletRepository(_connectionDb);
                var wallet = walletRepository.FindById(id);
                return wallet;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        public List<Wallet> GetAllWallet()
        {
            try
            {
                if (_connectionDb.State != ConnectionState.Open)
                    _connectionDb.Open();
                var walletRepository = _vakapayRepositoryFactory.GetWalletRepository(_connectionDb);

                var result = walletRepository.FindBySql($"SELECT * FROM {SimpleCRUD.GetTableName(typeof(Wallet))}");
                return result;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw e;
            }
        }

        //public List<Wallet> GetWalletByID()
        //{ }

        public bool CheckExistedAddress(string address, string networkName)
        {
            try
            {
                if (_connectionDb.State != ConnectionState.Open)
                    _connectionDb.Open();
                var walletRepository = _vakapayRepositoryFactory.GetWalletRepository(_connectionDb);
                var wallet = walletRepository.FindByAddressAndNetworkName(address, networkName);

                return wallet != null;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
        }

        public List<string> GetAddresses(string walletId, string networkName)
        {
            try
            {
                if (_connectionDb.State != ConnectionState.Open)
                    _connectionDb.Open();
                var walletRepository = _vakapayRepositoryFactory.GetWalletRepository(_connectionDb);
                return walletRepository.GetStringAddresses(walletId, networkName);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        // check wallet exists or not. Then update balance for this
        public bool CheckExistedAndUpdateByAddress(string addr, decimal amount, string networkName)
        {
            try
            {
                if (_connectionDb.State != ConnectionState.Open)
                    _connectionDb.Open();

                var wallet = FindByAddressAndNetworkName(addr, networkName);

                //check existed
                if (wallet == null)
                    return false;

                var id = wallet.Id;
                var version = wallet.Version;

                var result = UpdateBalance(amount, id, version);
                if (result.Status == Status.STATUS_SUCCESS)
                    return true;

                // can't update Balance
                logger.Error("Error when update Balance: Address = " + addr + "; Amount = " + amount +
                             "; NetworkName = " + networkName);
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
                if (_connectionDb.State != ConnectionState.Open)
                    _connectionDb.Open();
                var walletRepository = _vakapayRepositoryFactory.GetWalletRepository(_connectionDb);
                var result = walletRepository.UpdateBalanceWallet(amount, id, version);

                return result;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return new ReturnObject
                {
                    Status = Status.STATUS_ERROR,
                    Message = e.Message
                };
            }
        }

        /// <summary>
        /// Get history of wallet
        /// </summary>
        /// <param name="numberData"></param>
        /// <param name="userId"></param>
        /// <param name="currencyName"></param>
        /// <param name="offset"></param>
        /// <param name="limit"></param>
        /// <param name="orderBy"></param>
        /// <param name="search"></param>
        /// <returns></returns>
        public List<BlockchainTransaction> GetHistory(out int numberData, string userId, string currencyName,
            int offset = -1, int limit = -1, string[] orderBy = null, string search = null)
        {
            numberData = -1;
            List<BlockchainTransaction> output = new List<BlockchainTransaction>();
            // Console.WriteLine(wallet.Currency);

            switch (currencyName)
            {
                case CryptoCurrency.ETH:

                    output = ethereumBussiness.GetAllHistory(out numberData, userId, currencyName, offset, limit,
                        orderBy, search);
                    break;
                case CryptoCurrency.VAKA:
                    output = vakacoinBussiness.GetAllHistory(out numberData, userId, currencyName, offset, limit,
                        orderBy, search);
                    break;
                case CryptoCurrency.BTC:
                    output = bitcoinBussiness.GetAllHistory(out numberData, userId, currencyName, offset, limit,
                        orderBy, search);
                    break;
            }

            //  Console.WriteLine("get history " + wallet.Currency + "_count=_" + output.Count);
            return output;
        }

        public Wallet FindByAddressAndNetworkName(string addr, string networkName)
        {
            var walletRepository = _vakapayRepositoryFactory.GetWalletRepository(_connectionDb);
            var wallets = walletRepository.FindByAddressAndNetworkName(addr, networkName);

            return wallets;
        }

        public string FindEmailByAddressAndNetworkName(string addr, string networkName)
        {
            var userRepository = _vakapayRepositoryFactory.GetUserRepository(_connectionDb);
            var wallet = FindByAddressAndNetworkName(addr, networkName);

            var user = userRepository.FindById(wallet.UserId);

            if (user?.Id == null)
                return null;

            return user.Email;
        }

        public static bool ValidateAddress(string address, string networkName)
        {
            switch (networkName)
            {
                case CryptoCurrency.BTC:
                    return BlockchainHeper.IsBitcoinAddress(address);

                case CryptoCurrency.ETH:
                    return BlockchainHeper.IsEthereumAddress(address);

                case CryptoCurrency.VAKA:
                    var vakacoinRpc = new VakacoinRpc(AppSettingHelper.GetVakacoinNode());
                    return vakacoinRpc.CheckAccountExist(address);
                default:
                    throw new Exception("Network name not define!");
            }
        }

        /// <summary>
        /// Create Address for wallet when wallet status is pending
        /// </summary>
        /// <returns></returns>
        public async Task<ReturnObject> CreateAddressAsync()
        {
            var walletRepository = _vakapayRepositoryFactory.GetWalletRepository(_connectionDb);
            var pendingWallet = walletRepository.FindRowPending();

            if (pendingWallet?.Id == null)
                return new ReturnObject
                {
                    Status = Status.STATUS_SUCCESS,
                    Message = "Pending wallet not found"
                };

            if (_connectionDb.State != ConnectionState.Open)
                _connectionDb.Open();

            var dbTransaction = _connectionDb.BeginTransaction();
            try
            {
                var lockResult = await walletRepository.LockForProcess(pendingWallet);
                if (lockResult.Status == Status.STATUS_ERROR)
                {
                    dbTransaction.Rollback();
                    return new ReturnObject
                    {
                        Status = Status.STATUS_SUCCESS,
                        Message = "Cannot Lock For Process"
                    };
                }

                dbTransaction.Commit();
            }
            catch (Exception e)
            {
                dbTransaction.Rollback();
                return new ReturnObject
                {
                    Status = Status.STATUS_ERROR,
                    Message = e.ToString()
                };
            }

            //update Version to Model
            pendingWallet.Version += 1;

            var transactionSend = _connectionDb.BeginTransaction();
            try
            {
                var sendResult = await CreateAddressForWallet(pendingWallet);
                //                if (sendResult.Status == Status.STATUS_ERROR) // Not return error, update row.status = ERROR
                //                {
                //                    return sendResult;
                //                }
                //if (sendResult.Status == Status.STATUS_ERROR)
                //{
                //    throw new Exception(sendResult.Message);
                //}
                pendingWallet.Status = sendResult.Status;
                pendingWallet.UpdatedAt = (int)CommonHelper.GetUnixTimestamp();
                pendingWallet.IsProcessing = 0;
                pendingWallet.AddressCount += 1;

                var updateResult = await walletRepository.SafeUpdate(pendingWallet);
                if (updateResult.Status == Status.STATUS_ERROR)
                {
                    transactionSend.Rollback();
                    return new ReturnObject
                    {
                        Status = Status.STATUS_ERROR,
                        Message = updateResult.Message
                    };
                }

                transactionSend.Commit();
                return updateResult;
            }
            catch (Exception e)
            {
                // release lock
                transactionSend.Rollback();
                var releaseResult = walletRepository.ReleaseLock(pendingWallet);
                Console.WriteLine(JsonHelper.SerializeObject(releaseResult));
                throw;
            }
        }

        public async Task<ReturnObject> CreateAddressAsync(Wallet wallet)
        {
            var walletRepository = _vakapayRepositoryFactory.GetWalletRepository(_connectionDb);

            //update Version to Model
            wallet.Version += 1;

            var transactionSend = _connectionDb.BeginTransaction();
            try
            {
                var sendResult = await CreateAddressForWallet(wallet);
                if (sendResult.Status == Status.STATUS_ERROR)
                {
                    return sendResult;
                }

                wallet.Status = sendResult.Status;
                wallet.UpdatedAt = (int)CommonHelper.GetUnixTimestamp();
                wallet.IsProcessing = 0;
                wallet.AddressCount += 1;

                var updateResult = await walletRepository.SafeUpdate(wallet);
                if (updateResult.Status == Status.STATUS_ERROR)
                {
                    transactionSend.Rollback();
                    return new ReturnObject
                    {
                        Status = Status.STATUS_ERROR,
                        Message = "Cannot update wallet status"
                    };
                }

                updateResult.Data = sendResult.Data;

                transactionSend.Commit();
                return updateResult;
            }
            catch (Exception e)
            {
                // release lock
                transactionSend.Rollback();
                var releaseResult = walletRepository.ReleaseLock(wallet);
                Console.WriteLine(JsonHelper.SerializeObject(releaseResult));
                throw;
            }
        }

        public async Task<ReturnObject> CreateAddressForWallet(Wallet pendingWallet)
        {
            try
            {
                var pass = CommonHelper.RandomString(15);
                ReturnObject res = new ReturnObject()
                {
                    Status = Status.STATUS_ERROR,
                    Message = "No currency " + pendingWallet.Currency
                }; ;
                switch (pendingWallet.Currency)
                {
                    case CryptoCurrency.ETH:
                        Console.WriteLine("make eth");
                        //  var ethereumBusiness = new EthereumBusiness.EthereumBusiness(_vakapayRepositoryFactory, false);
                        res = ethereumBussiness.CreateAddressAsync(
                            new EthereumAddressRepository(_connectionDb),
                            new EthereumRpc(AppSettingHelper.GetEthereumNode()),
                            pendingWallet.Id, pass).Result;
                        break;

                    case CryptoCurrency.BTC:
                        Console.WriteLine("make btc");
                        //   var bitcoinBusiness = new BitcoinBusiness.BitcoinBusiness(_vakapayRepositoryFactory, false);
                        res = bitcoinBussiness.CreateAddressAsync(
                            new BitcoinAddressRepository(_connectionDb),
                            new BitcoinRpc(AppSettingHelper.GetBitcoinNode(),
                                AppSettingHelper.GetBitcoinRpcAuthentication()),
                            pendingWallet.Id, pass).Result;
                        break;

                    case CryptoCurrency.VAKA:
                        Console.WriteLine("make vaka");
                        //    var vakaBusiness = new VakacoinBusiness.VakacoinBusiness(_vakapayRepositoryFactory, false);
                        res = vakacoinBussiness.CreateAddressAsync(
                            new VakacoinAccountRepository(_connectionDb),
                            new VakacoinRpc(AppSettingHelper.GetVakacoinNode()),
                            pendingWallet.Id, pass).Result;
                        break;
                }

                return res;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return new ReturnObject()
                {
                    Status = Status.STATUS_ERROR,
                    Message = e.Message
                };
            }
        }

        public Wallet FindByUserAndNetwork(string userId, string networkName)
        {
            try
            {
                var walletRepository = _vakapayRepositoryFactory.GetWalletRepository(_connectionDb);
                var wallets = walletRepository.FindByUserAndNetwork(userId, networkName);
                return wallets;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        public List<string> DistinctUserId()
        {
            try
            {
                var walletRepo = _vakapayRepositoryFactory.GetWalletRepository(_connectionDb);
                return walletRepo.DistinctUserId();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        public ReturnObject ValidateWithdrawAmount(string userId,
            bool sendByBlockchainAddress, decimal amount, string sendTransactionFee, string currency)
        {
            try
            {
                // Validate amount
                if (amount <= 0)
                {
                    return new ReturnObject()
                    {
                        Status = Status.STATUS_ERROR,
                        Message = "Withdraw balance must be positive."
                    };
                }

                var walletRepository = new WalletRepository(_connectionDb);

                var wallet = walletRepository.FindByUserAndNetwork(userId, currency);

                if (sendByBlockchainAddress == true)
                {
                    if (amount + GetFee(currency) > wallet.Balance)
                    {
                        return new ReturnObject()
                        {
                            Status = Status.STATUS_ERROR,
                            Message = "Withdraw balance + fee is larger than wallet balance."
                        };
                    }
                }
                else
                {
                    if (amount > wallet.Balance)
                    {
                        return new ReturnObject()
                        {
                            Status = Status.STATUS_ERROR,
                            Message = "Withdraw balance larger than wallet balance."
                        };
                    }
                }

                return new ReturnObject()
                {
                    Status = Status.STATUS_SUCCESS
                };
            }
            catch (Exception e)
            {
                return new ReturnObject()
                {
                    Status = Status.STATUS_ERROR,
                    Message = "Validate amount throw exception."
                };
            }
        }

        public List<BlockchainAddress> GetAddressesFull(out int numberData, string walletId, string networkName,
            int offset = 0, int limit = 8,
            string filter = "", string sort = "")
        {
            numberData = -1;
            try
            {
                if (_connectionDb.State != ConnectionState.Open)
                    _connectionDb.Open();
                var walletRepository = _vakapayRepositoryFactory.GetWalletRepository(_connectionDb);
                return walletRepository.GetAddressesLimit(out numberData, walletId, networkName, offset, limit,
                    filter);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        public BlockchainAddress GetAddressesInfo(string id, string networkName)
        {
            try
            {
                if (_connectionDb.State != ConnectionState.Open)
                    _connectionDb.Open();
                var walletRepository = _vakapayRepositoryFactory.GetWalletRepository(_connectionDb);
                return walletRepository.GetAddressesInfo(id, networkName);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        public ReturnObject UpdateAddress(string id, string networkName, string label)
        {
            try
            {
                switch (networkName)
                {
                    case CryptoCurrency.BTC:
                        var bitcoinAddressRepository = new BitcoinAddressRepository(_connectionDb);
                        BitcoinAddress bitcoinAddress = bitcoinAddressRepository.FindById(id);
                        bitcoinAddress.Label = label;
                        return bitcoinAddressRepository.Update(bitcoinAddress);

                    case CryptoCurrency.ETH:
                        var ethereumAddressRepository = new EthereumAddressRepository(_connectionDb);
                        EthereumAddress ethereumAddress = ethereumAddressRepository.FindById(id);
                        ethereumAddress.Label = label;
                        return ethereumAddressRepository.Update(ethereumAddress);

                    case CryptoCurrency.VAKA:
                        var vakacoinAddressRepository = new VakacoinAccountRepository(_connectionDb);
                        VakacoinAccount vakacoinAccount = vakacoinAddressRepository.FindById(id);
                        vakacoinAccount.Label = label;
                        return vakacoinAddressRepository.Update(vakacoinAccount);
                    default:
                        return new ReturnObject
                        {
                            Status = Status.STATUS_ERROR
                        };
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return new ReturnObject
                {
                    Status = Status.STATUS_ERROR
                };
            }
        }


    }
}