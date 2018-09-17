using System;
using System.Collections;
using System.IO;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using NLog;
using Vakapay.Models.Domains;
using Vakapay.Models.Repositories;
using Vakapay.Repositories.Mysql;
using Vakapay.VakacoinBusiness;

namespace Vakapay.ScanVakaCoin
{
    class Program
    {
        private VakapayRepositoryMysqlPersistenceFactory _persistenceFactory;
        private VakacoinBusiness.VakacoinBusiness _vakacoinBusiness;
        private WalletBusiness.WalletBusiness _walletBusiness;
        private VakacoinRpc _rpc;
        private int _lastBlock;
        private ArrayList _errorBlocks;
        private System.Runtime.Caching.MemoryCache _cache;
        private static Logger logger = LogManager.GetCurrentClassLogger();

        static void Main(string[] args)
        {
            Program pr = new Program();
            pr.Init();
            pr.Processing();
            pr.RescanErrorBlock();
//            pr.ReceivedProcess(pr.rpc.GetAllTransactionsInBlock("16535578"));
        }

        private void Init()
        {
            //read configs from Configs.json
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("Configs.json");
            IConfiguration Configuration = builder.Build();

            var repositoryConfig = new RepositoryConfiguration
            {
                ConnectionString = Configuration.GetSection("ConnectionStrings").Value
            };
            _persistenceFactory = new VakapayRepositoryMysqlPersistenceFactory(repositoryConfig);

            _vakacoinBusiness = new VakacoinBusiness.VakacoinBusiness(_persistenceFactory);
            _walletBusiness = new WalletBusiness.WalletBusiness(_persistenceFactory);

            _rpc = new VakacoinRpc(Configuration.GetSection("EndpointUrl").Value);

            //lastBlock = GetFromCache()
            _lastBlock = 0;
            _errorBlocks = new ArrayList();

            _cache = new System.Runtime.Caching.MemoryCache("ScanVakacoinCache");
            if (_cache["lastBlock"] != null)
                _lastBlock = (int) _cache["lastBlock"];
        }

        private void Processing()
        {
            while (true)
            {
                int headBlock = _rpc.GetHeadBlockNumber();
                logger.Info("Head Block = "+headBlock);
                if (headBlock > _lastBlock)
                {
                    if (_lastBlock == 0)
                        FirstRun(headBlock);
                    else Run(headBlock);
                }

                System.Threading.Thread.Sleep(500);
            }
        }

        //Run program in first time
        private void FirstRun(int headBlock)
        {
            try
            {
                var transactions = _rpc.GetAllTransactionsInBlock(headBlock.ToString());
                ReceivedProcess(transactions);
                _lastBlock = headBlock;
                _cache.Set("lastBlock", _lastBlock, DateTimeOffset.MaxValue);
            }
            catch (Exception e)
            {
                _errorBlocks.Add(headBlock);
                logger.Error(e);
                throw;
            }
        }

        //Run program in not first time
        private void Run(int headBlock)
        {
            int tmp = _lastBlock + 1;
            for (int processingBlock = tmp; processingBlock <= headBlock; processingBlock++)
            {
                try
                {
                    logger.Info("Scan block "+processingBlock);
                    var transactions = _rpc.GetAllTransactionsInBlock(processingBlock.ToString());
                    ReceivedProcess(transactions);
                    _lastBlock = processingBlock;
                    _cache.Set("lastBlock", _lastBlock, DateTimeOffset.MaxValue);
                }
                catch (Exception e)
                {
                    _errorBlocks.Add(processingBlock);
                    logger.Error(e, " Error when process block ", processingBlock);
                    throw;
                }
            }
        }

        private void RescanErrorBlock()
        {
            while (true)
            {
                if (_errorBlocks != null && _errorBlocks.Count > 0)
                {
                    foreach (int block in _errorBlocks)
                    {
                        try
                        {
                            var transactions = _rpc.GetAllTransactionsInBlock(block.ToString());
                            ReceivedProcess(transactions);
                            _errorBlocks.Remove(block);
                        }
                        catch (Exception e)
                        {
                            logger.Error(e, " Error when process block " + block + " again");
                            throw;
                        }
                    }
                }

                System.Threading.Thread.Sleep(60000);
            }
        }

        private void ReceivedProcess(ArrayList transactions)
        {
            // process each transaction in arraylist of transactions
            foreach (var transaction in transactions)
            {
                try
                {
                    var jsonTrx = JObject.Parse(transaction.ToString())["transaction"];

                    //check name of action in transaction
                    var actionName = jsonTrx["actions"][0]["name"];
                    if (actionName == null)
                        continue;
                    if (actionName.ToString() != "transfer")
                        continue;

                    //check data is valid json
                    if (!JsonHelper.IsValidJson(jsonTrx["actions"][0]["data"].ToString()))
                        continue;

                    //check quantity and to_address in transaction
                    var quantity = jsonTrx["actions"][0]["data"]["quantity"];
                    var _to = jsonTrx["actions"][0]["data"]["to"];

                    //if quantity == null, process next transaction in arraylist
                    if (quantity == null || _to == null)
                    {
                        continue;
                    }

                    string _quantity = quantity.ToString();
                    if (!String.IsNullOrEmpty(_quantity))
                    {
                        //check symbol in quantity
                        var symbol = _quantity.Split(" ")[1];
                        if (symbol == "EOS" || symbol == "VAKA")
                        {
                            string to = _to.ToString();

                            // if receiver doesn't exist in wallet table, process next transaction
                            if (!_walletBusiness.CheckExistedAddress(to))
                            {
                                logger.Info(to + " is not exist in Wallet!!!");
                                continue;
                            }

                            string from = jsonTrx["actions"][0]["data"]["from"].ToString();
                            decimal amount = decimal.Parse(_quantity.Split(" ")[0]);
                            string transactionTime = jsonTrx["expiration"].ToString();
                            string status = "success";

                            // save to VakacoinTransactionHistory
                            ReturnObject result =
                                _vakacoinBusiness.CreateTransactionHistory(from, to, amount, transactionTime, status);

                            //update Balance in Wallet
                            _walletBusiness.UpdateBalance(to, amount, symbol);

                            logger.Info(to + " was received " + amount + " " + symbol);
                        }
                    }
                }
                catch (Exception e)
                {
                    logger.Error(e);
                    throw;
                }
            }
        }
    }
}