using System;
using System.Data;
using Vakapay.Commons.Helpers;
using Vakapay.Configuration;
using Vakapay.Models.Domains;
using Vakapay.Models.Repositories;

namespace Vakapay.PortfolioHistoryBusiness
{
    public class PortfolioHistoryBusiness
    {
        private IPortfolioHistoryRepository Repository { get; set; }
        public IDbConnection DbConnection { get; }

        public PortfolioHistoryBusiness(IVakapayRepositoryFactory vakapayRepositoryFactory, bool isNewConnection = true)
        {
            DbConnection = isNewConnection
                ? vakapayRepositoryFactory.GetOldConnection()
                : vakapayRepositoryFactory.GetDbConnection();
            Repository = vakapayRepositoryFactory.GetPortfolioHistoryRepository(DbConnection);
        }

        public ReturnObject InsertWithPrice(string userId)
        {
            string vkcPrice = CacheHelper.GetCacheString(String.Format(
                CoinmarketConfiguration.COINMARKET_PRICE_CACHEKEY, CoinmarketConfiguration.VAKACOIN,
                CoinmarketConfiguration.CURRENT));
            string btcPrice = CacheHelper.GetCacheString(String.Format(
                CoinmarketConfiguration.COINMARKET_PRICE_CACHEKEY, CoinmarketConfiguration.BITCOIN,
                CoinmarketConfiguration.CURRENT));
            string ethPrice = CacheHelper.GetCacheString(String.Format(
                CoinmarketConfiguration.COINMARKET_PRICE_CACHEKEY, CoinmarketConfiguration.ETHEREUM,
                CoinmarketConfiguration.CURRENT));
            string eosPrice = CacheHelper.GetCacheString(String.Format(
                CoinmarketConfiguration.COINMARKET_PRICE_CACHEKEY, CoinmarketConfiguration.EOS,
                CoinmarketConfiguration.CURRENT));
            
            return Repository.InsertWithPrice(userId, vkcPrice, btcPrice, ethPrice, eosPrice);
        }
    }
}