using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper;
using Newtonsoft.Json.Linq;
using NLog;
using Vakapay.BitcoinBusiness;
using Vakapay.Commons.Helpers;
using Vakapay.Configuration;
using Vakapay.Cryptography;
using Vakapay.Models.Domains;
using Vakapay.Models.Entities;
using Vakapay.Models.Repositories;
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
        private readonly IVakapayRepositoryFactory vakapayRepositoryFactory;

        private readonly IDbConnection ConnectionDb;

        private static Logger logger = LogManager.GetCurrentClassLogger();

        public WalletBusiness(IVakapayRepositoryFactory _vakapayRepositoryFactory, bool isNewConnection = true)
        {
            vakapayRepositoryFactory = _vakapayRepositoryFactory;
            ConnectionDb = isNewConnection
                ? vakapayRepositoryFactory.GetDbConnection()
                : vakapayRepositoryFactory.GetOldConnection();

            ethereumBussiness = new EthereumBusiness.EthereumBusiness(_vakapayRepositoryFactory);
            bitcoinBussiness = new BitcoinBusiness.BitcoinBusiness(_vakapayRepositoryFactory);
            vakacoinBussiness = new VakacoinBusiness.VakacoinBusiness(_vakapayRepositoryFactory);
            sendMailBusiness = new SendMailBusiness.SendMailBusiness(vakapayRepositoryFactory);
            userBusiness = new UserBusiness.UserBusiness(vakapayRepositoryFactory);
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
                foreach (string blockchainName in NetworkName.AllNetwork)
                {
                    ReturnObject _result = CreateNewWallet(user, blockchainName);
                    if (_result.Status == Status.StatusError)
                    {
                        return _result;
                    }
                }
                return new ReturnObject
                {
                    Status = Status.StatusSuccess,
                    Message = "Create all wallet done"
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
                if (ConnectionDb.State != ConnectionState.Open)
                {
                    ConnectionDb.Open();
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

                var walletRepo = vakapayRepositoryFactory.GetWalletRepository(ConnectionDb);
                var existUserNetwork =
                    walletRepo.FindByUserAndNetwork(user.Id,
                        blockchainNetwork);
                if (existUserNetwork != null)
                {
                    return new ReturnObject
                    {
                        Status = Status.StatusError,
                        Message = "User with NetworkName have already existed:" + JsonHelper.SerializeObject(existUserNetwork)
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
//                    Address = null, //Comment Id: NoAddressNeeded
                    Balance = 0,
                    Version = 0,
                    CreatedAt = (int)CommonHelper.GetUnixTimestamp(),
                    NetworkName = blockchainNetwork,
                    UpdatedAt = (int)CommonHelper.GetUnixTimestamp(),
                    UserId = user.Id
                };
                var resultMakeWallet = walletRepo.Insert(wallet);
                return resultMakeWallet;
            }
            catch (Exception e)
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
        public List<Wallet> LoadAllWalletByUser(User user)
        {
            //find wallet by usser
            try
            {
                if (ConnectionDb.State != ConnectionState.Open)
                    ConnectionDb.Open();
                var walletRepository = vakapayRepositoryFactory.GetWalletRepository(ConnectionDb);
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
        public ReturnObject Withdraw(Wallet wallet, string toAddress, decimal amount)
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
                if (ConnectionDb.State != ConnectionState.Open)
                {
                    ConnectionDb.Open();
                }

                string fromAddress = GetSenderAddress(wallet, toAddress, amount);

                var walletRepository = vakapayRepositoryFactory.GetWalletRepository(ConnectionDb);
                var userRepository =
                    vakapayRepositoryFactory.GetUserRepository(ConnectionDb);
                var etherWithdrawTransaction =
                    vakapayRepositoryFactory.GetEthereumWithdrawTransactionRepository(ConnectionDb);
                var btcWithdrawTransaction =
                    vakapayRepositoryFactory.GetBitcoinWithdrawTransactionRepository(ConnectionDb);
                var vakaWithdrawTransaction =
                    vakapayRepositoryFactory.GetVakacoinWithdrawTransactionRepository(ConnectionDb);


                // 1. Validate User status
                var walletById = walletRepository.FindById(wallet.Id);

                var userCheck = userRepository.FindById(walletById.UserId);
                if (userCheck == null ||
                    userCheck.Status != Status.StatusActive )
                {
                    return new ReturnObject
                    {
                        Status = Status.StatusError,
                        Message = "User Not Found || Not Active"
                    };
                }

                // 2. TODO validate Network status
                var validateNetworks = ValidateNetworkStatus(wallet.NetworkName);
                if ( validateNetworks.Status == Status.StatusError)
                {
                    return validateNetworks;
                }

                // 3. Validate toAddress
                if (ValidateAddress(toAddress, wallet.NetworkName) == false)
                {
                    return new ReturnObject
                    {
                        Status = Status.StatusError,
                        Message = wallet.NetworkName + ": To Address is not valid!"
                    };
                }

                var free = GetFee(wallet.NetworkName);

                // 4. Validate amount
                if (walletById.Balance < amount + free)
                {
                    return new ReturnObject()
                    {
                        Status = Status.StatusError,
                        Message = "Can't transfer bigger than wallet balance"
                    };
                }

                // 5. Update Wallet Balance
                var updateWallet = UpdateBalance(-(amount + free), wallet.Id, wallet.Version );
                if (updateWallet == null || updateWallet.Status == Status.StatusError)
                {
                    return new ReturnObject()
                    {
                        Status = Status.StatusError,
                        Message = "Fail update balance in walletDB"
                    };
                }

                // 6. Make new transaction withdraw pending by
                //insert into EthereumWithdrawTransaction database
                ReturnObject insertWithdraw = null;
                if (walletById.NetworkName.Equals(NetworkName.ETH))
                {
                    var etherWithdraw = new EthereumWithdrawTransaction()
                    {
                        Id = CommonHelper.GenerateUuid(),
                        Status = Status.StatusPending,
                        FromAddress = fromAddress,
                        ToAddress = toAddress,
                        Fee = free,
                        Amount = amount,
                        CreatedAt = CommonHelper.GetUnixTimestamp(),
                        UpdatedAt = CommonHelper.GetUnixTimestamp(),
                        InProcess = 0,
                        Version = 0
                    };
                    insertWithdraw = etherWithdrawTransaction.Insert(etherWithdraw);
                    if (insertWithdraw == null ||
                        insertWithdraw.Status == Status.StatusError)
                    {
                        return new ReturnObject()
                        {
                            Status = Status.StatusError,
                            Message = "Fail insert to EthereumWithdrawTransaction"
                        };
                    }
                }

                if (walletById.NetworkName.Equals(NetworkName.BTC))
                {
                    var btcWithdraw = new BitcoinWithdrawTransaction()
                    {
                        Id = CommonHelper.GenerateUuid(),
                        Status = Status.StatusPending,
                        FromAddress = fromAddress,
                        ToAddress = toAddress,
                        Fee = free,
                        Amount = amount,
                        CreatedAt = CommonHelper.GetUnixTimestamp(),
                        UpdatedAt = CommonHelper.GetUnixTimestamp(),
                        InProcess = 0,
                        Version = 0
                    };
                    insertWithdraw = btcWithdrawTransaction.Insert(btcWithdraw);
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
                        FromAddress = fromAddress,
                        ToAddress = toAddress,
                        Fee = free,
                        Amount = amount,
                        CreatedAt = CommonHelper.GetUnixTimestamp(),
                        UpdatedAt = CommonHelper.GetUnixTimestamp(),
                        InProcess = 0,
                        Version = 0
                    };
                    insertWithdraw = vakaWithdrawTransaction.Insert(vakaWithdraw);
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

                return insertWithdraw;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return new ReturnObject
                {
                    Status = Status.StatusError,
                    Message = e.Message
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
                    case NetworkName.BTC:
                        var bitcoinRpcAccount = VakapayConfiguration.GetBitcoinRpcAccount();
                        var bitcoinRpc = new BitcoinRpc(VakapayConfiguration.GetBitcoinNode(), bitcoinRpcAccount.Username,
                            bitcoinRpcAccount.Password);

                        getInfoResult = bitcoinRpc.GetInfo();

                        if (getInfoResult.Status != Status.StatusSuccess)
                        {
                            return getInfoResult;
                        }
                        break;

                    case NetworkName.ETH:
                        break;//TODO

                    case NetworkName.VAKA:
                        var vakacoinRpc = new VakacoinRPC(VakapayConfiguration.GetVakacoinNode());
                        getInfoResult = vakacoinRpc.GetInfo();

                        if (getInfoResult.Status != Status.StatusSuccess)
                        {
                            return getInfoResult;
                        }
                        break;
                    default:
                        return new ReturnObject()
                        {
                            Status = Status.StatusError,
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
                Status = Status.StatusSuccess
            };
        }

        private decimal GetFee(string walletNetworkName)
        {
            // throw new NotImplementedException(); //TODO  must implement
            switch (walletNetworkName)
            {
                // TODO fake:
                case NetworkName.BTC:
                    return (decimal) 0.0005;
                case NetworkName.ETH:
                    return (decimal) 0.0005;
                case NetworkName.VAKA:
                    return 0;
                default:
                    throw new Exception("Undefined network name!");
            }
        }

        private string GetSenderAddress(Wallet wallet, string toAddress, decimal amount)
        {
//            throw new NotImplementedException(); //TODO  must implement
            //TODO fake
            return GetAddresses(wallet.Id, wallet.NetworkName)[0];
        }

//        public ReturnObject UpdateAddressForWallet(string walletId, string address)
//        {
//            try
//            {
//                var walletRepository = vakapayRepositoryFactory.GetWalletRepository(ConnectionDb);
//                var whereUpdateAddr = walletRepository.FindById(walletId);
//
//                //update address for walletId
//                whereUpdateAddr.Address = address;
//                whereUpdateAddr.UpdatedAt = (int)CommonHelper.GetUnixTimestamp();
//                var walletUpdate = walletRepository.Update(whereUpdateAddr);
//                if (walletUpdate.Status == Status.StatusError)
//                    return new ReturnObject
//                    {
//                        Status = Status.StatusError,
//                        Message = "Update wallet address fail"
//                    };
//                return new ReturnObject
//                {
//                    Status = Status.StatusSuccess,
//                    Message = "Add address to wallet complete"
//                };
//            }
//            catch (Exception e)
//            {
//                return new ReturnObject
//                {
//                    Status = Status.StatusError,
//                    Message = e.Message
//                };
//            }
//        }

        public ReturnObject UpdateBalance(string toAddress, decimal addedBlance, string networkName)
        {
            try
            {
                if (ConnectionDb.State != ConnectionState.Open)
                    ConnectionDb.Open();
                var walletRepository = vakapayRepositoryFactory.GetWalletRepository(ConnectionDb);
                var wallet = walletRepository.FindByAddressAndNetworkName(toAddress, networkName);
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
                wallet.UpdatedAt = (int)CommonHelper.GetUnixTimestamp();
                var result = walletRepository.Update(wallet);
                if (result.Status == Status.StatusError)
                {
                    return result;
                }
                else
                {
                    User user = userBusiness.getUserByID(wallet.UserId);
                    if (user != null)
                    {
                        var currentTime = CommonHelper.GetUnixTimestamp();
                        //send mail:
                        EmailQueue _email = new EmailQueue()
                        {
                            Id = CommonHelper.GenerateUuid(),
                            ToEmail = user.Email,
                            NetworkName = wallet.NetworkName,
                            Amount = addedBlance,
                            Template = EmailTemplate.Received,
                            Subject = EmailConfig.Subject_SentOrReceived,
                            CreatedAt = currentTime,
                            UpdatedAt = currentTime
                            //							Content = networkName + "+" + addedBlance
                        };
                        sendMailBusiness.CreateEmailQueueAsync(_email);
                    }
                }
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
        public Wallet GetWalletByID(String id)
        {
            try
            {
                if (ConnectionDb.State != ConnectionState.Open)
                    ConnectionDb.Open();
                var walletRepository = vakapayRepositoryFactory.GetWalletRepository(ConnectionDb);
                var wallet = walletRepository.FindById(id);
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
        public List<Wallet> GetAllWallet()
        {
            try
            {
                if (ConnectionDb.State != ConnectionState.Open)
                    ConnectionDb.Open();
                var walletRepository = vakapayRepositoryFactory.GetWalletRepository(ConnectionDb);

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
                if (ConnectionDb.State != ConnectionState.Open)
                    ConnectionDb.Open();
                var walletRepository = vakapayRepositoryFactory.GetWalletRepository(ConnectionDb);
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
                if (ConnectionDb.State != ConnectionState.Open)
                    ConnectionDb.Open();
                var walletRepository = vakapayRepositoryFactory.GetWalletRepository(ConnectionDb);
                return walletRepository.GetAddresses(walletId, networkName);
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
                if (ConnectionDb.State != ConnectionState.Open)
                    ConnectionDb.Open();

                var wallet = FindByAddressAndNetworkName(addr, networkName);

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
                return new ReturnObject
                {
                    Status = Status.StatusError,
                    Message = e.Message
                };
            }
        }

        /// <summary>
        /// Get history of wallet
        /// </summary>
        /// <param name="wallet">Get from DB</param>
        /// <param name="offet">-1 for not config</param>
        /// <param name="limit">-1 for not config</param>
        /// <param name="orderBy">null for not config</param>
        public void GetHistory(Wallet wallet, int offet = -1, int limit = -1, string[] orderBy = null)
        {
            List<BlockchainTransaction> output = new List<BlockchainTransaction>();
            switch (wallet.NetworkName)
            {
                case NetworkName.ETH:
                    output = ethereumBussiness.GetWithdrawHistory(offet, limit, orderBy);
                    break;
                case NetworkName.VAKA:
                    output = vakacoinBussiness.GetWithdrawHistory(offet, limit, orderBy);
                    break;
                case NetworkName.BTC:
                    output = bitcoinBussiness.GetWithdrawHistory(offet, limit, orderBy);
                    break;
                default:
                    break;
            }

            Console.WriteLine("get history " + wallet.NetworkName + "_count=_" + output.Count);

        }

        public Wallet FindByAddressAndNetworkName(string addr, string networkName)
        {
            var walletRepository = vakapayRepositoryFactory.GetWalletRepository(ConnectionDb);
            var wallets = walletRepository.FindByAddressAndNetworkName(addr, networkName);

            if (wallets == null)
                return null;

            return wallets;
        }

        public string FindEmailByAddressAndNetworkName(string addr, string networkName)
        {
            var userRepository = vakapayRepositoryFactory.GetUserRepository(ConnectionDb);
            var wallet = FindByAddressAndNetworkName(addr, networkName);

            var user = userRepository.FindById(wallet.UserId);

            if (user?.Id == null)
                return null;

            return user.Email;
        }

        public ReturnObject SetHasAddressForWallet(string walletId)
        {
            try
            {
                var walletRepository = vakapayRepositoryFactory.GetWalletRepository(ConnectionDb);
                var whereUpdateAddr = walletRepository.FindById(walletId);

                //update HasAddress for walletId
                whereUpdateAddr.HasAddress = true;

                whereUpdateAddr.UpdatedAt = (int)CommonHelper.GetUnixTimestamp();
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

        public bool ValidateAddress(string address, string networkName)
        {
            switch (networkName)
            {
                case NetworkName.BTC:
                    var bitcoinRpcAccount = VakapayConfiguration.GetBitcoinRpcAccount();
                    var bitcoinRpc = new BitcoinRpc(VakapayConfiguration.GetBitcoinNode(), bitcoinRpcAccount.Username,
                        bitcoinRpcAccount.Password);
                    var result = bitcoinRpc.ValidateAddress(address);
                    var jsonResult = JObject.Parse(result.Data);
                    return jsonResult["isvalid"].Value<bool>();

                case NetworkName.ETH:
                    return BlockchainHeper.IsEthereumAddress(address);

                case NetworkName.VAKA:
                    var vakacoinRpc = new VakacoinRPC(VakapayConfiguration.GetVakacoinNode());
                    return vakacoinRpc.CheckAccountExist(address);
            }

            return true;
        }
    }
}