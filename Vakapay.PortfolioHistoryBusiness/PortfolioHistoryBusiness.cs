﻿using System;
using System.Collections.Generic;
using System.Data;
using Vakapay.Commons.Constants;
using Vakapay.Commons.Helpers;
using Vakapay.Models.Domains;
using Vakapay.Models.Entities;
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
                RedisCacheKey.COINMARKET_PRICE_CACHEKEY, DashboardConfig.VAKACOIN,
                DashboardConfig.CURRENT));
            string btcPrice = CacheHelper.GetCacheString(String.Format(
                RedisCacheKey.COINMARKET_PRICE_CACHEKEY, DashboardConfig.BITCOIN,
                DashboardConfig.CURRENT));
            string ethPrice = CacheHelper.GetCacheString(String.Format(
                RedisCacheKey.COINMARKET_PRICE_CACHEKEY, DashboardConfig.ETHEREUM,
                DashboardConfig.CURRENT));
            string eosPrice = CacheHelper.GetCacheString(String.Format(
                RedisCacheKey.COINMARKET_PRICE_CACHEKEY, DashboardConfig.EOS,
                DashboardConfig.CURRENT));
            
            return Repository.InsertWithPrice(userId, vkcPrice, btcPrice, ethPrice, eosPrice);
        }

        public List<PortfolioHistory> FindByUserId(string userId, long from, long to)
        {
            return Repository.FindByUserId(userId, from, to);
        }
    }
}