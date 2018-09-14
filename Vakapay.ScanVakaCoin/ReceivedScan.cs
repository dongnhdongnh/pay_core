using System;
using System.Collections;
using Newtonsoft.Json.Linq;
using Vakapay.Models.Domains;
using Vakapay.VakacoinBusiness;

namespace Vakapay.ScanVakaCoin
{
    public class ReceivedScan
    {
        public void ReceivedProcess(VakacoinBusiness.VakacoinBusiness vakaBusiness, ArrayList transactions)
        {
            foreach (var transaction in transactions)
            {
                var jsonTrx = JObject.Parse(transaction.ToString())["transaction"];
                Console.WriteLine(jsonTrx.ToString());
                var quantity = jsonTrx["actions"][0]["data"]["quantity"];
                if (quantity == null)
                {
                    continue;
                }

                string _quantity = quantity.ToString();
                if (!String.IsNullOrEmpty(_quantity))
                {
                    if (_quantity.Split(" ")[1] == "EOS" || _quantity.Split(" ")[1] == "VAKA")
                    {
                        string from = jsonTrx["actions"][0]["data"]["from"].ToString();
                        string to = jsonTrx["actions"][0]["data"]["to"].ToString();
                        decimal amount = decimal.Parse(_quantity.Split(" ")[0]);
                        string transactionTime = jsonTrx["expiration"].ToString();
                        string status = "success";
                        try
                        {
                            ReturnObject result =
                                vakaBusiness.CreateTransactionHistory(from, to, amount, transactionTime, status);
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