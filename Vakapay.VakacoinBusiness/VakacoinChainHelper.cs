using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using NLog;
using Vakapay.BlockchainBusiness;
using Vakapay.Commons.Helpers;
using Vakapay.Models.Domains;
using Vakapay.Models.Entities;
using Vakapay.VakacoinBusiness;
using VakaSharp.Api.v1;
using VakaSharp.CustomTypes;
using Action = VakaSharp.Api.v1.Action;

namespace Vakapay.ScanVakaCoin
{
    public class VakacoinChainHelper : IChainHelper
    {
        private VakacoinRPC _rpcClient;
        private VakacoinBusiness.VakacoinBusiness _vakacoinBusiness;
        private IWalletBusiness _walletBusiness;
        private int _blockInterval;
        private SendMailBusiness.SendMailBusiness _sendMailBusiness;

        public const String TRANSACTION_STATUS_EXECUTED = "executed";
        public const String TRANSACTION_ACTION_TRANSFER = "transfer";
        public readonly static String[] TRANSACTION_SYMBOL_ARRAY = {"EOS", "VAKA"};

        public IBlockchainRPC RpcClient
        {
            get { return _rpcClient; }
            set { _rpcClient = (VakacoinRPC) value; }
        }

        public VakacoinChainHelper(int blockInterval, VakacoinRPC rpcClient,
            VakacoinBusiness.VakacoinBusiness vakacoinBusiness, IWalletBusiness walletBusiness, SendMailBusiness.SendMailBusiness sendMailBusiness)
        {
            _blockInterval = blockInterval;
            _rpcClient = rpcClient;
            _vakacoinBusiness = vakacoinBusiness;
            _walletBusiness = walletBusiness;
            _sendMailBusiness = sendMailBusiness;
        }

        private static Logger logger = LogManager.GetCurrentClassLogger();

        public IEnumerable<object> StreamBlock(uint startBlock = 0)
        {
            if (startBlock.Equals(0))
                startBlock = _rpcClient.GetLastIrreversibleBlockNum().GetValueOrDefault();
            while (true)
            {
                uint lastIrreversibleBlock = _rpcClient.GetLastIrreversibleBlockNum().GetValueOrDefault();
                logger.Info("Last Irreversible Block: " + lastIrreversibleBlock);

                // Parse transactions from current block to last trusted block
                for (uint blockNum = startBlock; blockNum <= lastIrreversibleBlock; blockNum++)
                {
                    yield return _rpcClient.GetBlockByNumber(blockNum);
                }

                // Done above then next to continuous block
                startBlock = lastIrreversibleBlock + 1;
                System.Threading.Thread.Sleep(_blockInterval);
            }
        }

        public void ParseTransaction(object block)
        {
            GetBlockResponse blockResponse = (GetBlockResponse) block;
            foreach ((Action action, PackedTransaction packedTransaction) in GetListAction(blockResponse))
            {
                if (action.Data.Equals(null))
                    return;
                if (action.Name == TRANSACTION_ACTION_TRANSFER)
                    ParseValidTransferAction(action, packedTransaction, blockResponse);
            }
        }

        public void ParseValidTransferAction(Action action, PackedTransaction packedTransaction,
            GetBlockResponse blockResponse)
        {
            TransferData transferData = JsonConvert.DeserializeObject<TransferData>(action.Data.ToString());
            if (String.IsNullOrEmpty(transferData.Quantity))
                return;
            if (TRANSACTION_SYMBOL_ARRAY.Contains(transferData.Symbol()))
            {
                // If receiver doesn't exist in wallet table then stop
                if (!_walletBusiness.CheckExistedAndUpdateByAddress(transferData.To, transferData.Amount(),
                    transferData.Symbol()))
                    return;

                // Save to table in db
                _vakacoinBusiness.CreateDepositTransaction(packedTransaction.Id, (int) blockResponse.BlockNum,
                    transferData.Symbol(), transferData.Amount(), transferData.From, transferData.To, 0,
                    Status.StatusSuccess);
                
                //create pending email
                var createEmailResult = CreatePendingEmail(transferData);
                if (createEmailResult.Status == Status.StatusSuccess)
                    logger.Info("Create pending email success");
                else
                    logger.Error("Create Pending email error!!!" + createEmailResult.Message);

                logger.Info(String.Format("{0} was received {1}", transferData.To, transferData.Quantity));
            }
        }

        public IEnumerable<(Action, PackedTransaction)> GetListAction(GetBlockResponse block)
        {
            logger.Info(String.Format("Block number {0} produced on {1}", block.BlockNum, block.Timestamp));
            foreach (TransactionReceipt transactionReceipt in block.Transactions)
            {
                if (transactionReceipt.Status != TRANSACTION_STATUS_EXECUTED || transactionReceipt.Trx is String)
                    continue;

                PackedTransaction packedTransaction =
                    JsonConvert.DeserializeObject<PackedTransaction>(transactionReceipt.Trx.ToString());
                foreach (Action action in packedTransaction.Transaction.Actions)
                    yield return (action, packedTransaction);
            }
        }

        public ReturnObject CreatePendingEmail(TransferData transferData)
        {
            logger.Info("Create pending email");
            string address = transferData.To;
            string toEmail = _walletBusiness.FindEmailByAddressAndNetworkName(address, transferData.Symbol());
            
            if (toEmail == null)
                return new ReturnObject
                {
                    Status = Status.StatusError,
                    Message = "Cannot find email address of user!!"
                };
            
            var email = new EmailQueue
            {
                Id = CommonHelper.GenerateUuid(),
                ToEmail = toEmail,
                Content = "You have received " + transferData.Amount(),
                Subject = "Balance notification!",
                Status = Status.StatusPending,
                CreatedAt = CommonHelper.GetUnixTimestamp(),
                UpdatedAt = CommonHelper.GetUnixTimestamp(),
                InProcess = 0,
                Version = 0
            };

            var result = _sendMailBusiness.CreateEmailQueueAsync(email);
            return result.Result;
        }
    }
}