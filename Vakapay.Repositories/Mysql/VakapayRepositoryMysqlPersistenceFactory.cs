using System.Data;
using MySql.Data.MySqlClient;
using Vakapay.Models.Repositories;

namespace Vakapay.Repositories.Mysql
{
    public class VakapayRepositoryMysqlPersistenceFactory : IVakapayRepositoryFactory
    {
        public RepositoryConfiguration RepositoryConfiguration { get; }

        public IDbConnection Connection { get; set; }

        public VakapayRepositoryMysqlPersistenceFactory(RepositoryConfiguration repositoryConfiguration)
        {
            RepositoryConfiguration = repositoryConfiguration;
        }

        public IDbConnection GetDbConnection()
        {
            Connection = new MySqlConnection(RepositoryConfiguration.ConnectionString);
            return Connection;
        }

        public IDbConnection GetOldConnection()
        {
            return Connection ?? (Connection = new MySqlConnection(RepositoryConfiguration.ConnectionString));
        }

        public IWalletRepository GetWalletRepository(IDbConnection dbConnection)
        {
            return new WalletRepository(dbConnection);
        }

        public IUserRepository GetUserRepository(IDbConnection dbConnection)
        {
            return new UserRepository(dbConnection);
        }

        public IUserActionLogRepository GetUserActionLogRepository(IDbConnection dbConnection)
        {
            return new UserActionLogRepository(dbConnection);
        }

        public IWebSessionRepository GetWebSessionRepository(IDbConnection dbConnection)
        {
            return new WebSessionRepository(dbConnection);
        }

        public IApiKeyRepository GetApiKeyRepository(IDbConnection dbConnection)
        {
            return new ApiKeyRepository(dbConnection);
        }

        public IConfirmedDevicesRepository GetConfirmedDevicesRepository(IDbConnection dbConnection)
        {
            return new ConfirmedDevicesRepository(dbConnection);
        }

        public IEthereumAddressRepository GetEthereumAddressRepository(IDbConnection dbConnection)
        {
            return new EthereumAddressRepository(dbConnection);
        }

        public IBitcoinAddressRepository GetBitcoinAddressRepository(IDbConnection dbConnection)
        {
            return new BitcoinAddressRepository(dbConnection);
        }

        public IBitcoinWithdrawTransactionRepository GetBitcoinWithdrawTransactionRepository(IDbConnection dbConnection)
        {
            return new BitcoinWithdrawTransactionRepository(dbConnection);
        }

        public IVakacoinAccountRepository GetVakacoinAccountRepository(IDbConnection dbConnection)
        {
            return new VakacoinAccountRepository(dbConnection);
        }

        public IEthereumWithdrawTransactionRepository GetEthereumWithdrawTransactionRepository(
            IDbConnection dbConnection)
        {
            return new EthereumWithdrawnTransactionRepository(dbConnection);
        }

        public IEthereumDepositTransactionRepository GetEthereumDepositeTransactionRepository(
            IDbConnection dbConnection)
        {
            return new EthereumDepositTransactionRepository(dbConnection);
        }


        public IVakacoinWithdrawTransactionRepository GetVakacoinWithdrawTransactionRepository(
            IDbConnection dbConnection)
        {
            return new VakacoinWithdrawTransactionRepository(dbConnection);
        }

        public IVakacoinDepositTransactionRepository GetVakacoinDepositTransactionRepository(IDbConnection dbConnection)
        {
            return new VakacoinDepositTransactionRepository(dbConnection);
        }

        public IBitcoinDepositTransactionRepository GetBitcoinDepositTransactionRepository(IDbConnection dbConnection)
        {
            return new BitcoinDepositTransactionRepository(dbConnection);
        }

        public ISendEmailRepository GetSendEmailRepository(IDbConnection dbConnection)
        {
            return new SendEmailRepository(dbConnection);
        }

        public ISendSmsRepository GetSendSmsRepository(IDbConnection dbConnection)
        {
            return new SendSmsRepository(dbConnection);
        }

        public IInternalTransactionRepository GetInternalTransactionRepository(IDbConnection dbConnection)
        {
            return new InternalTransactionsRepository(dbConnection);
        }

        public IPortfolioHistoryRepository GetPortfolioHistoryRepository(IDbConnection dbConnection)
        {
            return new PortfolioHistoryRepository(dbConnection);
        }
    }
}