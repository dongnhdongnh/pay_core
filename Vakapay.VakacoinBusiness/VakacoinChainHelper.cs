using System;
using System.Collections.Generic;
using NLog;
using Vakapay.BlockchainBusiness;
using Vakapay.Commons.Constants;
using Vakapay.Commons.Helpers;
using Vakapay.Models.Domains;
using Vakapay.Models.Entities;
using VakaSharp.Api.v1;
using VakaSharp.CustomTypes;
using Action = VakaSharp.Api.v1.Action;

namespace Vakapay.VakacoinBusiness
{
    public class VakacoinChainHelper : IChainHelper
    {
        private VakacoinRpc _rpcClient;
        private VakacoinBusiness _vakacoinBusiness;
        private IWalletBusiness _walletBusiness;
        private int _blockInterval;
        private SendMailBusiness.SendMailBusiness _sendMailBusiness;

        public const String TRANSACTION_STATUS_EXECUTED = "executed";
        public const String TRANSACTION_ACTION_TRANSFER = "transfer";

        public IBlockchainRpc RpcClient
        {
            get { return _rpcClient; }
            set { _rpcClient = (VakacoinRpc)value; }
        }

        public VakacoinChainHelper(int blockInterval, VakacoinRpc rpcClient,
            VakacoinBusiness vakacoinBusiness, IWalletBusiness walletBusiness,
            SendMailBusiness.SendMailBusiness sendMailBusiness)
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
            if (CacheHelper.HaveKey(
                String.Format(RedisCacheKey.KEY_SCANBLOCK_LASTSCANBLOCK,
                    CryptoCurrency.VAKA)
            ))
                uint.TryParse(
                    CacheHelper.GetCacheString(String.Format(RedisCacheKey.KEY_SCANBLOCK_LASTSCANBLOCK,
                        CryptoCurrency.VAKA)),
                    out startBlock
                );

            if (startBlock.Equals(0))
                startBlock = _rpcClient.GetLastIrreversibleBlockNum().GetValueOrDefault();

            while (true)
            {
                uint lastIrreversibleBlock = _rpcClient.GetLastIrreversibleBlockNum().GetValueOrDefault();
                logger.Info("Last Irreversible Block: " + lastIrreversibleBlock);

                // Parse transactions from current block to last trusted block
                for (uint blockNum = startBlock; blockNum <= lastIrreversibleBlock; blockNum++)
                {
                    CacheHelper.SetCacheString(String.Format(RedisCacheKey.KEY_SCANBLOCK_LASTSCANBLOCK,
                        "VAKA"), blockNum.ToString());
                    yield return _rpcClient.GetBlockByNumber(blockNum);
                }

                // Done above then next to continuous block
                startBlock = lastIrreversibleBlock + 1;
                System.Threading.Thread.Sleep(_blockInterval);
            }
        }

        public void ParseTransaction(object block)
        {
            GetBlockResponse blockResponse = (GetBlockResponse)block;
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
            TransferData transferData = JsonHelper.DeserializeObject<TransferData>(action.Data.ToString());
            if (String.IsNullOrEmpty(transferData.Quantity))
                return;
            if (transferData.Symbol() == CryptoCurrency.VAKA)
            {
                // If receiver doesn't exist in wallet table then stop
                if (!_walletBusiness.CheckExistedAndUpdateByAddress(transferData.To, transferData.Amount(),
                    transferData.Symbol()))
                    return;

                // Save to table in db
                _vakacoinBusiness.CreateDepositTransaction(packedTransaction.Id, (int)blockResponse.BlockNum,
                    transferData.Symbol(), transferData.Amount(), transferData.From, transferData.To, 0,
                    Status.STATUS_SUCCESS);

                //create pending email
                var createEmailResult = CreatePendingEmail(transferData);
                if (createEmailResult.Status == Status.STATUS_SUCCESS)
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
                    JsonHelper.DeserializeObject<PackedTransaction>(transactionReceipt.Trx.ToString());
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
                    Status = Status.STATUS_ERROR,
                    Message = "Cannot find email address of user!!"
                };

            var email = new EmailQueue
            {
                ToEmail = toEmail,
                SignInUrl = EmailConfig.SIGN_IN_URL,
                Amount = transferData.Amount(),
                Template = EmailTemplate.Received,
//                SentOrReceived = EmailConfig.SentOrReceived_Received,
                NetworkName = transferData.Symbol(),
                Subject = EmailConfig.SUBJECT_SENT_OR_RECEIVED,
                Status = Status.STATUS_PENDING,
                IsProcessing = 0,
                Version = 0
            };

            var result = _sendMailBusiness.CreateEmailQueueAsync(email);
            return result.Result;
        }
    }
}