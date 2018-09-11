using System;
using System.Net;
using Newtonsoft.Json.Linq;
using Vakapay.Commons.Helpers;
using Vakapay.Models.Domains;
using Vakapay.Models.Entities;
using Vakapay.Models.Repositories;

namespace Vakapay.BitcoinBusiness
{
    using BlockchainBusiness;

    public class BitcoinBusiness : BlockchainBusiness
    {
        private BitcoinRpc bitcoinRpc { get; set; }


        public BitcoinBusiness(IVakapayRepositoryFactory _vakapayRepositoryFactory, bool isNewConnection = true) :
            base(_vakapayRepositoryFactory, isNewConnection)
        {
            bitcoinRpc = new BitcoinRpc("http://127.0.0.1:18443");
            bitcoinRpc.Credentials = new NetworkCredential("bitcoinrpc", "wqfgewgewi");
        }

        // <summary>
        // Returns a new bitcoin address for receiving payments.
        // If [account] is specified (recommended), it is added to the address book so payments 
        // received with the address will be credited to [account].
        // </summary>
        // <param name="a_account"></param>
        // bitcoin ver 16 GetNewAddress(Account); ver 17 GetNewAddress(Label)
        public ReturnObject CreateNewAddAddress(string Account = "", string WalletId = "")
        {
            try
            {
                var address = bitcoinRpc.GetNewAddress(Account);
                Console.WriteLine(address);
                if (WalletId == "")
                {
                    //khoi tao wallet
                    var wallet = new Wallet
                    {
                        Id = CommonHelper.GenerateUuid(),
                    };

                    WalletId = wallet.Id;
                }
               

                //add database vakaxa
                var bitcoinAddressRepo = vakapayRepositoryFactory.GetBitcoinAddressRepository(DbConnection);
                var bcAddress = new BitcoinAddress
                {
                    Id = CommonHelper.GenerateUuid(),
                    Address = address,
                    WalletId = WalletId,
                    CreatedAt = (int)CommonHelper.GetUnixTimestamp(),
                    UpdatedAt = (int)CommonHelper.GetUnixTimestamp()
                };

                var ResultAddBitcoinAddress = bitcoinAddressRepo.Insert(bcAddress);
                //
                return ResultAddBitcoinAddress;
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
        /// Amount is a real and is rounded to the nearest 0.01. Returns the transaction ID if successful.
        /// </summary>
        /// <param name="fromAccount"></param>
        /// <param name="toAddress"></param>
        /// <param name="amount"></param>
        /// <param name="minconf"></param>
        /// <param name="comment"></param>
        /// <param name="commentTo"></param>
        public ReturnObject SendFrom(string fromAccount,
            string toAddress,
            float amount,
            int minconf = 1,
            string comment = "",
            string commentTo = "")
        {
            try
            {
                var idTransaction = bitcoinRpc.SendFrom(fromAccount, toAddress, amount, minconf, comment, commentTo);
                var transactionInfo = bitcoinRpc.GetTransaction(idTransaction);
                Console.WriteLine(transactionInfo);
                //add database vakaxa
                var bitcoinAddressRepo = vakapayRepositoryFactory.GetEthereumAddressRepository(DbConnection);

                //
                return new ReturnObject
                {
                    Status = Status.StatusSuccess,
                    Data = idTransaction
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
        /// Send coins to address. Returns txid.
        /// </summary>
        /// <param name="address"></param>
        /// <param name="amount"></param>
        /// <param name="comment"></param>
        /// <param name="commentTo"></param>
        public ReturnObject SendToAddress(string address,
            float amount,
            string comment = "",
            string commentTo = "")
        {
            try
            {
                var idTransaction = bitcoinRpc.SendToAddress(address, amount, comment, commentTo);

                //add database vakaxa
                var bitcoinAddressRepo = vakapayRepositoryFactory.GetEthereumAddressRepository(DbConnection);

                //
                return new ReturnObject
                {
                    Status = Status.StatusSuccess,
                    Data = idTransaction
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
        /// If [account] is not specified, returns the server's total available balance.
        /// If [account] is specified, returns the balance in the account.
        /// </summary>
        /// <param name="account"></param>
        /// <param name="minconf"></param>
        public ReturnObject GetBalance(string account = "", int minconf = 1)
        {
            try
            {
                var balance = bitcoinRpc.GetBalance(account, minconf);

                return new ReturnObject
                {
                    Status = Status.StatusSuccess,
                    Data = balance.ToString()
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
        
        public JArray GetlistWallets()
        {
            try
            {
                Console.WriteLine(1);
                return bitcoinRpc.ListWallets();
            }
            catch (Exception e)
            {
                return new JArray {e.Message};
            }
        }
    }
}