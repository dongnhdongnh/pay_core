using System.Threading.Tasks;
using System;
namespace Vakapay.DashboardServices
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Start App");
            Task currencyTask = Task.Factory.StartNew(() => CurrencyConverter.GetCurrencyConverter());
            Task portfolioTask = Task.Factory.StartNew(() => PorfolioHistory.RunPortfolioHistory());
            Task coinmarketTask = Task.Factory.StartNew(() => ScanCoinmarket.RunScanCoinmarket());

            Task.WaitAll(currencyTask, portfolioTask, coinmarketTask);
     
        }
    }
}