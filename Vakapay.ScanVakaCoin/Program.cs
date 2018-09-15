using System;
using System.Collections;
using System.IO;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using Vakapay.Models.Domains;
using Vakapay.Models.Repositories;
using Vakapay.Repositories.Mysql;
using Vakapay.VakacoinBusiness;

namespace Vakapay.ScanVakaCoin
{
    class Program
    {
        private VakapayRepositoryMysqlPersistenceFactory PersistenceFactory;
        private VakacoinBusiness.VakacoinBusiness vakacoinBusiness;
        private WalletBusiness.WalletBusiness walletBusiness;
        private VakacoinRpc rpc;
        private int lastBlock;
        private ArrayList errorBlocks;

        static void Main(string[] args)
        {
            Program pr = new Program();
            pr.Init();
            pr.Processing();
            pr.RescanErrorBlock();
//            pr.ReceivedProcess(pr.rpc.GetAllTransactionsInBlock("16512179"));
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
            PersistenceFactory = new VakapayRepositoryMysqlPersistenceFactory(repositoryConfig);

            vakacoinBusiness = new VakacoinBusiness.VakacoinBusiness(PersistenceFactory);
            walletBusiness = new WalletBusiness.WalletBusiness(PersistenceFactory);

            rpc = new VakacoinRpc(Configuration.GetSection("EndpointUrl").Value);

            //lastBlock = GetFromCache()
            lastBlock = 0;
            errorBlocks = new ArrayList();
            
//            var cache = new System.Runtime.Caching.MemoryCache("ScanVakacoinCache");
            
        }

        private void Processing()
        {
            while (true)
            {
                int headBlock = rpc.GetHeadBlockNumber();
                if (headBlock > lastBlock)
                {
                    if (lastBlock == 0)
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
                var transactions = rpc.GetAllTransactionsInBlock(headBlock.ToString());
                ReceivedProcess(transactions);
                lastBlock = headBlock;
            }
            catch (Exception e)
            {
                errorBlocks.Add(headBlock);
                Console.WriteLine(e);
                throw;
            }
        }

        //Run program in not first time
        private void Run(int headBlock)
        {
            int tmp = lastBlock+1;
            for (int processingBlock = tmp; processingBlock <= headBlock; processingBlock++)
            {
                try
                {
                    var transactions = rpc.GetAllTransactionsInBlock(processingBlock.ToString());
                    ReceivedProcess(transactions);
                    lastBlock = processingBlock;
                }
                catch (Exception e)
                {
                    errorBlocks.Add(processingBlock);
                    Console.WriteLine("Error when process block " + processingBlock, e);
                    throw;
                }
            }
        }

        private void RescanErrorBlock()
        {
            while (true)
            {
                if (errorBlocks != null && errorBlocks.Count > 0)
                {
                    foreach (int block in errorBlocks)
                    {
                        try
                        {
                            var transactions = rpc.GetAllTransactionsInBlock(block.ToString());
                            ReceivedProcess(transactions);
                            errorBlocks.Remove(block);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("Error when process block " + block + " again", e);
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
                var jsonTrx = JObject.Parse(transaction.ToString())["transaction"];
//                Console.WriteLine(jsonTrx.ToString());
                
                //check name of action in transaction
                var actionName = jsonTrx["actions"][0]["name"];
                if (actionName == null)
                    continue;
                if (actionName.ToString() != "transfer")
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
                        if (!walletBusiness.CheckExistedAddress(to))
                        {
                            Console.WriteLine(to+ " is not exist in Wallet!!!");
                            continue;
                        }

                        string from = jsonTrx["actions"][0]["data"]["from"].ToString();
                        decimal amount = decimal.Parse(_quantity.Split(" ")[0]);
                        string transactionTime = jsonTrx["expiration"].ToString();
                        string status = "success";

                        try
                        {
                            // save to VakacoinTransactionHistory
                            ReturnObject result =
                                vakacoinBusiness.CreateTransactionHistory(from, to, amount, transactionTime, status);

                            //update Balance in Wallet
                            walletBusiness.UpdateBalance(to, amount, symbol);

                            Console.WriteLine(to + " was received " + amount + " " + symbol);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                            throw;
                        }
                    }
                }
            }
        }
    }
}