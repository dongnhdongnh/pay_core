using System.Threading.Tasks;

namespace Vakapay.DashboardServices
{
    class Program
    {
        static void Main(string[] args)
        {
            Task currencyTask = Task.Factory.StartNew(() => CurrencyConverter.GetCurrencyConverter());
            Task portfolioTask = Task.Factory.StartNew(() => PorfolioHistory.RunPortfolioHistory());
            Task coinmarketTask = Task.Factory.StartNew(() => ScanCoinmarket.RunScanCoinmarket());

            Task.WaitAll(currencyTask, portfolioTask, coinmarketTask);
        }
    }
}