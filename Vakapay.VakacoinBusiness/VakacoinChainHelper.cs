using System;
using System.Collections.Generic;
using NLog;
using Vakapay.BlockchainBusiness;
using Vakapay.VakacoinBusiness;
using VakaSharp.Api.v1;

namespace Vakapay.ScanVakaCoin
{
    public class VakacoinChainHelper: IChainHelper
    {
        private VakacoinRPC _rpcClient;
        private int _blockInterval;

        public const String TRANSACTION_STATUS_EXECUTED = "executed";
        
        public IBlockchainRPC RpcClient
        {
            get { return _rpcClient; }
            set { _rpcClient = (VakacoinRPC) value; }
        }

        public VakacoinChainHelper(int blockInterval)
        {
            _blockInterval = blockInterval;
        }
        
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public IEnumerable<object> StreamBlock(uint startBlock = 0)
        {
            if (startBlock.Equals(0))
                startBlock = _rpcClient.GetLastIrreversibleBlockNum().GetValueOrDefault();
            while (true)
            {
                uint lastIrreversibleBlock = _rpcClient.GetLastIrreversibleBlockNum().GetValueOrDefault();
                logger.Info("Last Irreversible Block = " + lastIrreversibleBlock);
                
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
            ParseTransaction((GetBlockResponse) block);
        }

        public void ParseTransaction(GetBlockResponse block)
        {
            Console.WriteLine(block.BlockNum);
            foreach (var transaction in block.Transactions)
            {
                if (transaction.Status != TRANSACTION_STATUS_EXECUTED)
                {
                    continue;
                }
//                var jsonTrx = JObject.Parse(transaction.ToString())["transaction"];
//
//                //check name of action in transaction
//                var actionName = jsonTrx["actions"][0]["name"];
//                if (actionName == null)
//                    continue;
//                if (actionName.ToString() != "transfer")
//                    continue;
//
//                //check data is valid json
//                if (!JsonHelper.IsValidJson(jsonTrx["actions"][0]["data"].ToString()))
//                    continue;
//
//                //check quantity and to_address in transaction
//                var quantity = jsonTrx["actions"][0]["data"]["quantity"];
//                var _to = jsonTrx["actions"][0]["data"]["to"];
//
//                //if quantity == null, process next transaction in arraylist
//                if (quantity == null || _to == null)
//                {
//                    continue;
//                }
//                
//                var trxId = JObject.Parse(transaction.ToString())["id"].ToString();
//
//                string _quantity = quantity.ToString();
//                if (!String.IsNullOrEmpty(_quantity))
//                {
//                    //check symbol in quantity
//                    var symbol = _quantity.Split(" ")[1];
//                    if (symbol == "EOS" || symbol == "VAKA")
//                    {
//                        string to = _to.ToString();
//
//                        // if receiver doesn't exist in wallet table, process next transaction
//                        if (!_walletBusiness.CheckExistedAddress(to))
//                        {
//                            logger.Info(to + " is not exist in Wallet!!!");
//                            continue;
//                        }
//
//                        string from = jsonTrx["actions"][0]["data"]["from"].ToString();
//                        decimal amount = decimal.Parse(_quantity.Split(" ")[0]);
//                        string transactionTime = jsonTrx["expiration"].ToString();
//                        string status = "success";
//
//                        // save to VakacoinTransactionHistory
//                        ReturnObject result =
//                            _vakacoinBusiness.CreateTransactionHistory(trxId, from, to, Int32.Parse(block.BlockNum.ToString()), amount, transactionTime, status);
//
//                        //update Balance in Wallet
//                        _walletBusiness.UpdateBalance(to, amount, symbol);
//
//                        logger.Info(to + " was received " + amount + " " + symbol);
//                    }
//                }
            }
        }
    }
}