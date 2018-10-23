﻿using System;
using System.IO;
using System.Threading;
using Microsoft.Extensions.Configuration;
using Vakapay.Commons.Helpers;
using Vakapay.Models.Repositories;
using Vakapay.Repositories.Mysql;

namespace Vakapay.SendSms
{
    internal static class Program
    {
        private static void Main()
        {
            var apiKey = AppSettingHelper.Get("Elastic:ApiKey");
            var apiAddress = AppSettingHelper.Get("Elastic:SmsUrl");


            var repositoryConfig = new RepositoryConfiguration
            {
                ConnectionString = AppSettingHelper.GetDBConnection()
            };

            var persistenceFactory = new VakapayRepositoryMysqlPersistenceFactory(repositoryConfig);
            var sendSmsBusiness = new SendSmsBusiness.SendSmsBusiness(persistenceFactory);

            while (true)
            {
                try
                {
                    var result = sendSmsBusiness.SendSmsAsync(apiAddress, apiKey);
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