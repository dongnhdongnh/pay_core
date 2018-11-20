using System.Collections.Generic;
using Vakapay.Models.Domains;
using Vakapay.Models.Entities;
using Vakapay.Models.Repositories.Base;
using System;
namespace Vakapay.Models.Repositories
{
    public interface IPortfolioHistoryRepository : IRepositoryBase<PortfolioHistory>,IDisposable
    {
        List<PortfolioHistory> FindByUserId(string userId, long from, long to);
        ReturnObject InsertWithPrice(string userId, string vkcPrice, string btcPrice, string ethPrice);
    }
}