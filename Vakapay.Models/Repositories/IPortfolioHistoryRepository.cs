using System.Collections.Generic;
using Vakapay.Models.Domains;
using Vakapay.Models.Entities;
using Vakapay.Models.Repositories.Base;

namespace Vakapay.Models.Repositories
{
    public interface IPortfolioHistoryRepository : IRepositoryBase<PortfolioHistory>
    {
        List<PortfolioHistory> FindByUserId(string userId, int from, int to);
        ReturnObject InsertWithPrice(string userId, string vkcPrice, string btcPrice, string ethPrice, string eosPrice);
    }
}