using System.Collections.Generic;
using Vakapay.Models.Domains;
using Vakapay.Models.Entities;
using Vakapay.Models.Repositories.Base;

namespace Vakapay.Models.Repositories
{
    public interface IUserRepository : IRepositoryBase<User>
    {
        string FindEmailByBitcoinAddress(string bitcoinAddress);
        string QuerySearch(Dictionary<string, string> models);
        User FindWhere(string sql);
        string FindEmailBySendTransaction(BlockchainTransaction transaction);
        User FindByEmailAddress(string emailAddress);

        List<User> FindAllUser();
    }
}