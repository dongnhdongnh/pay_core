using System;
using System.Data;
using Microsoft.Extensions.Logging;
using NLog;
using Vakapay.Commons.Constants;
using Vakapay.Commons.Helpers;
using Vakapay.Models.Domains;
using Vakapay.Models.Entities;
using Vakapay.Models.Repositories;
using Vakapay.Repositories.Mysql;

namespace Vakapay.UserSendTransactionBusiness
{
    public class UserSendTransactionBusiness
    {
        private readonly IVakapayRepositoryFactory _vakapayRepositoryFactory;

        private readonly IDbConnection _connectionDb;

        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public UserSendTransactionBusiness(IVakapayRepositoryFactory vakapayRepositoryFactory, bool isNewConnection = true)
        {
            _vakapayRepositoryFactory = vakapayRepositoryFactory;
            _connectionDb = isNewConnection
                ? vakapayRepositoryFactory.GetDbConnection()
                : vakapayRepositoryFactory.GetOldConnection();
        }

        public ReturnObject AddSendTransaction(UserSendTransaction sendTransaction)
        {
            try
            {
                if (!string.IsNullOrEmpty(sendTransaction.Idem))
                {
                    var sendTransactionRepository = new UserSendTransactionRepository(_connectionDb);
                    var idem = sendTransactionRepository.FindExistedIdem(sendTransaction);
                    if (idem != null)
                    {
                        return new ReturnObject()
                        {
                            Status = Status.STATUS_ERROR,
                            Message = $"Transaction with idem = {sendTransaction.Idem} is already existed!"
                        };
                    }
                }

                if (CommonHelper.IsValidEmail(sendTransaction.To))
                {
                    return AddSendTransactionToEmailAddress(sendTransaction);
                }
                else
                {
                    return AddSendTransactionToBlockchainAddress(sendTransaction);
                }
            }
            catch (Exception e)
            {
                _logger.Error(e.Message);
                return new ReturnObject()
                {
                    Status = Status.STATUS_ERROR,
                    Message = e.Message
                };
            }
        }

        private ReturnObject AddSendTransactionToBlockchainAddress(UserSendTransaction sendTransaction)
        {
            var walletRepository = new WalletRepository(_connectionDb);
            var walletBusiness =
                new WalletBusiness.WalletBusiness(_vakapayRepositoryFactory, false);

            var wallet = walletRepository.FindByUserAndNetwork(sendTransaction.UserId, sendTransaction.Currency);
            var res = walletBusiness.Withdraw(wallet, sendTransaction.To, sendTransaction.Amount);
            return res;
        }

        private ReturnObject AddSendTransactionToEmailAddress(UserSendTransaction sendTransaction)
        {
            if (_connectionDb.State != ConnectionState.Open)
                _connectionDb.Open();
            var sendTransactionRepository = new UserSendTransactionRepository(_connectionDb);
            var internalTransactionsRepository = new InternalTransactionsRepository(_connectionDb);

            var userRepository = new UserRepository(_connectionDb);
            var receiver = userRepository.FindByEmailAddress(sendTransaction.To);

            if (receiver == null)
            {
                return new ReturnObject()
                {
                    Status = Status.STATUS_ERROR,
                    Message = "Email address not found in Vakapay system"
                };
            }

            var insertTrx = _connectionDb.BeginTransaction();

            var insertRes = sendTransactionRepository.Insert(sendTransaction);

            if (insertRes.Status == Status.STATUS_ERROR)
            {
                return insertRes;
            }

            insertRes = internalTransactionsRepository.Insert(new InternalWithdrawTransaction()
            {
                SenderUserId = sendTransaction.UserId,
                ReceiverUserId = receiver.Id,
                Amount = sendTransaction.Amount,
                Currency = sendTransaction.Currency,
                Idem = sendTransaction.Idem,
                IsProcessing = 0,
                Version = 0,
                Status = Status.STATUS_PENDING,
                CreatedAt = CommonHelper.GetUnixTimestamp(),
                UpdatedAt = CommonHelper.GetUnixTimestamp(),
            });

            if (insertRes.Status == Status.STATUS_ERROR)
            {
                insertTrx.Rollback();
                return insertRes;
            }
            insertTrx.Commit();

            return new ReturnObject()
            {
                Status = Status.STATUS_SUCCESS,
                Message = "Inserted to transaction database!"
            };
        }
    }
}