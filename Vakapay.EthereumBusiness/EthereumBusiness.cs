using System;
using System.Data;
using Vakapay.Commons.Helpers;
using Vakapay.Models.Domains;
using Vakapay.Models.Entities;
using Vakapay.Models.Repositories;

namespace Vakapay.EthereumBusiness
{
    using BlockchainBusiness;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using Vakapay.Models.Entities.ETH;

    public class EthereumBusiness : BlockchainBusiness
    {
        private EthereumRpc ethereumRpc { get; set; }

        public EthereumBusiness(IVakapayRepositoryFactory _vakapayRepositoryFactory, bool isNewConnection = true, string endPoint = "") :
            base(_vakapayRepositoryFactory, isNewConnection)
        {
            ethereumRpc = new EthereumRpc(endPoint);
        }
        public ReturnObject RunSendTransaction()
        {
            try
            {
                var ethereumwithdrawRepo = VakapayRepositoryFactory.GetEthereumWithdrawTransactionRepository(DbConnection);
                List<EthereumWithdrawTransaction> pendings = ethereumwithdrawRepo.FindBySql(ethereumwithdrawRepo.Query_Search("Status", Status.StatusPending));
                Console.WriteLine(pendings.Count);
                if (pendings.Count <= 0) throw new Exception("NO PENING");
                else
                {
                    foreach (EthereumWithdrawTransaction pending in pendings)
                    {
                        SendTransaction(pending);
                    }
                }
                return new ReturnObject();
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
        public ReturnObject SendTransaction(EthereumWithdrawTransaction blockchainTransaction)
        {
            try
            {
                var _rpcResult = ethereumRpc.SendTransactionWithPassphrase(blockchainTransaction.FromAddress, blockchainTransaction.ToAddress, blockchainTransaction.Amount, "password");
                if (_rpcResult.Status == Status.StatusError)
                    return _rpcResult;
                EthRPCJson.Getter _getter = new EthRPCJson.Getter(_rpcResult.Data);
                var ethereumwithdrawRepo = VakapayRepositoryFactory.GetEthereumWithdrawTransactionRepository(DbConnection);
                blockchainTransaction.Status = Status.StatusCompleted;
                blockchainTransaction.UpdatedAt = CommonHelper.GetUnixTimestamp().ToString();
                EthereumWithdrawTransaction blockchainTransactionWhere = new EthereumWithdrawTransaction()
                {
                    Id = blockchainTransaction.Id,
                    Amount = blockchainTransaction.Amount,
                    Fee = blockchainTransaction.Fee
                };
                return ethereumwithdrawRepo.ExcuteSQL(ethereumwithdrawRepo.Query_Update(blockchainTransaction, blockchainTransactionWhere));

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

        public ReturnObject FakePendingTransaction(EthereumWithdrawTransaction blockchainTransaction)
        {
            try
            {


                //var _rpcResult = ethereumRpc.SendTransactionWithPassphrase(blockchainTransaction.FromAddress, blockchainTransaction.ToAddress, blockchainTransaction.Amount, "password");
                //if (_rpcResult.Status == Status.StatusError)
                //    return _rpcResult;
                // EthRPCJson.Getter _getter = new EthRPCJson.Getter(_rpcResult.Data);
                var ethereumwithdrawRepo = VakapayRepositoryFactory.GetEthereumWithdrawTransactionRepository(DbConnection);
                blockchainTransaction.Id = CommonHelper.GenerateUuid();
                // blockchainTransaction.Hash = (String)_getter.result;
                blockchainTransaction.Status = Status.StatusPending;
                blockchainTransaction.CreatedAt = CommonHelper.GetUnixTimestamp().ToString();
                blockchainTransaction.UpdatedAt = CommonHelper.GetUnixTimestamp().ToString();

                return ethereumwithdrawRepo.Insert(blockchainTransaction);

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
                if (ResultMakeAddress.Status == Status.StatusError)
                    return ResultMakeAddress;

                EthRPCJson.Getter _getter = new EthRPCJson.Getter(ResultMakeAddress.Data);

                var ethereumAddressRepo = VakapayRepositoryFactory.GetEthereumAddressRepository(DbConnection);

                //TODO Encrypt Password Before save
                var ResultAddEthereumAddress = ethereumAddressRepo.Insert(new EthereumAddress
                {
                    Status = Status.StatusActive,
                    // Address = ResultMakeAddress.Data,
                    Address = (String)_getter.result,
                    //Address = (String)_getter.result,
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

            }

        }

        bool isScanning = false;
        public void AutoScanBlock()
        {
            Thread scanThread = new Thread(DoAutoScanBlock);
            scanThread.Start();


        }
        void DoAutoScanBlock()
        {
            while (true)

            {

                Console.WriteLine("is scan=" + isScanning);
                if (!isScanning)
                    ScanBlock();
                Thread.Sleep(5000);

            }
        }

        public int ScanBlock()
        {
            Console.WriteLine("START NEW SCAN");
            isScanning = true;
            long _time = CommonHelper.GetUnixTimestamp();
            int LastBlock = -1;
            int.TryParse(CacheHelper.GetCacheString(CacheHelper.CacheKey.KEY_ETH_LASTSCANBLOCK), out LastBlock);
            if (LastBlock < 0)
                LastBlock = 0;

            int blockNumber = 0;
            var _result = ethereumRpc.GetBlockNumber();
            if (_result.Status == Status.StatusError)
            {
                isScanning = false;
                return 0;
            }
            EthRPCJson.Getter _getter = new EthRPCJson.Getter(_result.Data);
            if (!_getter.result.ToString().HexToInt(out blockNumber))
            {
                isScanning = false;
                return 0;
            }
            Console.WriteLine("block number " + blockNumber);


            //	String _query = String.Format("SELECT * FROM vakapay.ethereumwithdrawtransaction Where Status='{0}'", Status.StatusPending);
            var ethereumwithdrawRepo = VakapayRepositoryFactory.GetEthereumWithdrawTransactionRepository(DbConnection);
            List<EthereumWithdrawTransaction> _pending = ethereumwithdrawRepo.FindBySql(ethereumwithdrawRepo.Query_Search("Status", Status.StatusPending));
            if (_pending.Count <= 0)
            {
                Console.WriteLine("No pending");
                isScanning = false;
                return 0;
            }






            List<EthRPCJson.BlockInfor> blocks = new List<EthRPCJson.BlockInfor>();
            Console.WriteLine("SCAN FROM " + LastBlock + "___" + blockNumber);
            for (int i = LastBlock; i <= blockNumber; i++)
            {
                if (i < 0) continue;
                _result = ethereumRpc.FindBlockByNumber(i);
                if (_result.Status == Status.StatusError)
                {
                    isScanning = false;
                    return 0;
                }
                if (_result.Data == null)
                    continue;
                _getter = new EthRPCJson.Getter(_result.Data);
                EthRPCJson.BlockInfor _block = JsonHelper.DeserializeObject<EthRPCJson.BlockInfor>(_getter.result.ToString());
                if (_block.transactions.Length > 0)
                {
                    blocks.Add(_block);
                }
                //	Console.WriteLine(JsonHelper.SerializeObject(_getter.result));
            }
            CacheHelper.SetCacheString(CacheHelper.CacheKey.KEY_ETH_LASTSCANBLOCK, blockNumber.ToString());
            if (blocks.Count <= 0)
            {
                Console.WriteLine("NO BLOCK");
                isScanning = false;
                return 0;
            }
            //check pending and update:
            var ethereumWithdrawRepo = VakapayRepositoryFactory.GetEthereumWithdrawTransactionRepository(DbConnection);
            foreach (EthRPCJson.BlockInfor _block in blocks)
            {
                if (_pending.Count <= 0)
                {
                    //SCAN DONE:
                    isScanning = false;
                    return 0;
                }
                for (int i = _pending.Count - 1; i >= 0; i--)
                {
                    EthereumWithdrawTransaction _currentPending = _pending[i];
                    EthRPCJson.TransactionInfor _trans = _block.transactions.SingleOrDefault(x => x.hash.Equals(_currentPending.Hash));
                    if (_trans != null)
                    {
                        Console.WriteLine("HELLO " + _currentPending.Hash);
                        _currentPending.BlockNumber = _trans.blockNumber;
                        _currentPending.UpdatedAt = CommonHelper.GetUnixTimestamp().ToString();
                        _currentPending.Status = Status.StatusCompleted;
                        ethereumWithdrawRepo.Update(_currentPending);
                        _pending.RemoveAt(i);
                    }

                }
            }

            _time = CommonHelper.GetUnixTimestamp() - _time;
            Console.WriteLine(blocks.Count + ",Time=" + _time);
            isScanning = false;
            return blocks.Count;

        }
    }
}
