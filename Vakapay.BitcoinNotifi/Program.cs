using System;
using System.IO;

namespace Vakapay.BitcoinNotifi
{
    class Program
    {
        static void Main(string[] args)
        {
//            var btcBussines = new BitcoinBusiness.BitcoinBusiness();
            string data = "\n data transaction: " + args[0];
            File.AppendAllText("/home/tienchelsea92/Desktop/test.txt", "\n"+DateTime.Now.ToString() +"   "+ data);
        }
    }
}