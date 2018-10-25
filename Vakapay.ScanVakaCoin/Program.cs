using Vakapay.Commons.Helpers;
using Vakapay.Models.Repositories;
using Vakapay.Repositories.Mysql;
using Vakapay.VakacoinBusiness;
using VakaSharp.Api.v1;

namespace Vakapay.ScanVakaCoin
{
    internal static class Program
    {
        public static void Main(string[] args)
        {
            var repositoryConfig = new RepositoryConfiguration
            {
                ConnectionString = AppSettingHelper.GetDBConnection(),
            };
            var persistenceFactory = new VakapayRepositoryMysqlPersistenceFactory(repositoryConfig);

            var helper = new VakacoinChainHelper(
                int.Parse(AppSettingHelper.GetVakacoinBlockInterval()),
                new VakacoinRPC(AppSettingHelper.GetVakacoinNode()),
                new VakacoinBusiness.VakacoinBusiness(persistenceFactory),
                new WalletBusiness.WalletBusiness(persistenceFactory),
                new SendMailBusiness.SendMailBusiness(persistenceFactory)
            );
            foreach (GetBlockResponse block in helper.StreamBlock())
            {
                helper.ParseTransaction(block);
            }
        }
    }
}