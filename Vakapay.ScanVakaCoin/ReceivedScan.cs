using System;
using System.Collections;
using Newtonsoft.Json.Linq;
using Vakapay.Models.Domains;
using Vakapay.VakacoinBusiness;

namespace Vakapay.ScanVakaCoin
{
    public class ReceivedScan
    {
        public void ReceivedProcess(VakacoinBusiness.VakacoinBusiness vakaBusiness, ArrayList transactions,
            WalletBusiness.WalletBusiness walletBusiness)
        {
            // process each transaction in arraylist of transactions
            foreach (var transaction in transactions)
            {
                var jsonTrx = JObject.Parse(transaction.ToString())["transaction"];
//                Console.WriteLine(jsonTrx.ToString());

                //check quantity in transaction
                var quantity = jsonTrx["actions"][0]["data"]["quantity"];

                //if quantity == null, process next transaction in arraylist
                if (quantity == null)
                {
                    continue;
                }

                string _quantity = quantity.ToString();
                if (!String.IsNullOrEmpty(_quantity))
                {
                    
                    //check symbol in quantity
                    if (_quantity.Split(" ")[1] == "EOS" || _quantity.Split(" ")[1] == "VAKA")
                    {
                        string to = jsonTrx["actions"][0]["data"]["to"].ToString();
                        
                        // if receiver doesn't exist in wallet table, process next transaction
                        if (walletBusiness.CheckExistedAddress(to))
                        {
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
                                vakaBusiness.CreateTransactionHistory(from, to, amount, transactionTime, status);
                            
                            //update Balance in Wallet
                            walletBusiness.UpdateBalance(to, amount, _quantity.Split(" ")[1]);
                            
                            Console.WriteLine(to + " was received " + amount + " " + _quantity.Split(" ")[1]);
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