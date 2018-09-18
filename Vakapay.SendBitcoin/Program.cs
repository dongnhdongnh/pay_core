using System;
using System.IO;
using System.Timers;
using Microsoft.Extensions.Configuration;
using Vakapay.Models.Domains;
using Vakapay.Models.Repositories;
using Vakapay.Repositories.Mysql;
using NLog;

namespace Vakapay.SendBitcoin
{
    class Program
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        private static Timer _timer;
        private static BitcoinBusiness.BitcoinBusiness _btcBusiness;
        private const int TimeInterval = 1000;

        static void Main()
        {
            try
            {
                var builder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("Configs.json");
                IConfiguration configuration = builder.Build();

                var repositoryConfig = new RepositoryConfiguration
                {
                    ConnectionString =
                        configuration.GetSection("ConnectionStrings").Value
                };

                var persistenceFactory = new VakapayRepositoryMysqlPersistenceFactory(repositoryConfig);

                var bitcoinConnect = new BitcoinRPCConnect
                {
                    Host = configuration.GetSection("EndpointUrl").Value,
                    UserName = configuration.GetSection("User").Value,
                    Password = configuration.GetSection("Password").Value
                };
                _btcBusiness = new BitcoinBusiness.BitcoinBusiness(persistenceFactory, bitcoinConnect);

                SetTimer();
                Console.ReadLine();
            }
            catch (Exception e)
            {
                logger.Error(e, "Sendbitcoin exception");
                throw;
            }
        }

        private static void SetTimer()
        {
            // Create a timer with a two second interval.
            _timer = new Timer(TimeInterval);
            // Hook up the Elapsed event for the timer. 
            _timer.Elapsed += OnTimedEvent;
            _timer.AutoReset = true;
            _timer.Enabled = true;
        }

        private static void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            Console.WriteLine("The Elapsed event was raised at {0:HH:mm:ss.fff}",
                e.SignalTime);

           var result =  _btcBusiness.RunSendTransaction();
            logger.Debug("RunSendTransaction result : " + JsonHelper.SerializeObject(result));
        }
    }
}