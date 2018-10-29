using System;
using System.Threading;
using Vakapay.Commons.Helpers;
using Vakapay.Models.Repositories;
using Vakapay.Repositories.Mysql;

namespace Vakapay.SendEmail
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            var repositoryConfig = new RepositoryConfiguration
            {
                ConnectionString = AppSettingHelper.GetDbConnection()
            };

            var persistenceFactory = new VakapayRepositoryMysqlPersistenceFactory(repositoryConfig);
            var sendMailBusiness = new SendMailBusiness.SendMailBusiness(persistenceFactory);

            while (true)
            {
                try
                {
                    var result = sendMailBusiness.SendEmailAsync(AppSettingHelper.GetElasticMailUrl(),
                        AppSettingHelper.GetElasticApiKey(), AppSettingHelper.GetElasticFromAddress(),
                        AppSettingHelper.GetElasticFromName());
                    Console.WriteLine(JsonHelper.SerializeObject(result.Result));
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }

                Thread.Sleep(1000);
            }
        }
    }
}