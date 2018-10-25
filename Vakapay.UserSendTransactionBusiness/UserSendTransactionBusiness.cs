using System;
using System.Data;
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
            if (wallet == null)
            {
                return new ReturnObject()
                {
                    Status = Status.STATUS_ERROR,
                    Message = $"UserId {sendTransaction.UserId} with {sendTransaction.Currency} wallet is not found!"
                };
            }
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
                insertTrx.Rollback();
                return insertRes;
            }

            var internalTransactions = new InternalWithdrawTransaction()
            {
                SenderUserId = sendTransaction.UserId,
                ReceiverUserId = receiver.Id,
                Amount = sendTransaction.Amount,
                Currency = sendTransaction.Currency,
                Idem = sendTransaction.Idem,
            };

            insertRes = internalTransactionsRepository.Insert(internalTransactions);

            if (insertRes.Status == Status.STATUS_ERROR)
            {
                insertTrx.Rollback();
                return insertRes;
            }
            insertTrx.Commit();

            var sendTrx = _connectionDb.BeginTransaction();
            var senRes = SendInternalTransaction(internalTransactions);
            if (senRes.Status == Status.STATUS_ERROR )
            {
                sendTrx.Rollback();
                internalTransactionsRepository.Update(internalTransactions);
                return senRes;
            }

            var updateRes = internalTransactionsRepository.Update(internalTransactions);


            if (updateRes.Status == Status.STATUS_ERROR)
            {
                sendTrx.Rollback();
                return new ReturnObject()
                {
                    Status = Status.STATUS_ERROR,
                    Message = "Fail to update send status"
                };
            }




            sendTrx.Commit();

            return new ReturnObject()
            {
                Status = Status.STATUS_SUCCESS,
                Message = "Transaction Sent!"
            };
        }

//        public async Task<ReturnObject> SendInternalTransaction()
//        {
//            var internalTransactionsRepository = new InternalTransactionsRepository(_connectionDb);
//            var rowPending = internalTransactionsRepository.FindRowPending();
//
//            if (rowPending?.Id == null)
//                return new ReturnObject
//                {
//                    Status = Status.STATUS_SUCCESS,
//                    Message = "Pending internal transaction not found"
//                };
//
//            if (_connectionDb.State != ConnectionState.Open)
//                _connectionDb.Open();
//
//            var dbTransaction = _connectionDb.BeginTransaction();
//            try
//            {
//                var lockResult = await internalTransactionsRepository.LockForProcess(rowPending);
//                if (lockResult.Status == Status.STATUS_ERROR)
//                {
//                    dbTransaction.Rollback();
//                    return new ReturnObject
//                    {
//                        Status = Status.STATUS_SUCCESS,
//                        Message = "Cannot Lock For Process"
//                    };
//                }
//
//                dbTransaction.Commit();
//            }
//            catch (Exception e)
//            {
//                dbTransaction.Rollback();
//                return new ReturnObject
//                {
//                    Status = Status.STATUS_ERROR,
//                    Message = e.ToString()
//                };
//            }
//
//            //update Version to Model
//            rowPending.Version += 1;
//
//            var transactionSend = _connectionDb.BeginTransaction();
//            try
//            {
//                var sendResult = SendInternalTransaction(rowPending);
//
//                rowPending.Status = sendResult.Status;
//                rowPending.UpdatedAt = (int) CommonHelper.GetUnixTimestamp();
//                rowPending.IsProcessing = 0;
//
//                var updateResult = await internalTransactionsRepository.SafeUpdate(rowPending);
//                if (updateResult.Status == Status.STATUS_ERROR)
//                {
//                    transactionSend.Rollback();
//                    return new ReturnObject
//                    {
//                        Status = Status.STATUS_ERROR,
//                        Message = "Cannot update wallet status"
//                    };
//                }
//
//                transactionSend.Commit();
//                return updateResult;
//            }
//            catch (Exception e)
//            {
//                // release lock
//                transactionSend.Rollback();
//                var releaseResult = internalTransactionsRepository.ReleaseLock(rowPending);
//                Console.WriteLine(JsonHelper.SerializeObject(releaseResult));
//                throw;
//            }
//        }

        private ReturnObject SendInternalTransaction(InternalWithdrawTransaction transaction)
        {
            try
            {
                var walletRepository = _vakapayRepositoryFactory.GetWalletRepository(_connectionDb);
                var walletBusiness = new WalletBusiness.WalletBusiness(_vakapayRepositoryFactory,false);

                var senderWallet = walletRepository.FindByUserAndNetwork(transaction.SenderUserId, transaction.Currency);
                var receiverWallet = walletRepository.FindByUserAndNetwork(transaction.ReceiverUserId, transaction.Currency);

                if (senderWallet == null )
                {
                    transaction.Status = Status.STATUS_ERROR;
                    return new ReturnObject()
                    {
                        Status = Status.STATUS_ERROR,
                        Message = "Cannot find sender wallet"
                    };
                }

                if (receiverWallet == null)
                {
                    transaction.Status = Status.STATUS_ERROR;
                    return new ReturnObject()
                    {
                        Status = Status.STATUS_ERROR,
                        Message = "Cannot find receiver wallet"
                    };
                }

                if (senderWallet.Balance < transaction.Amount)
                {
                    transaction.Status = Status.STATUS_ERROR;
                    return new ReturnObject()
                    {
                        Status = Status.STATUS_ERROR,
                        Message = "sender balance is smaller than transaction amount"
                    };
                }

//                senderWallet.Balance -= transaction.Amount;
//                receiverWallet.Balance += transaction.Amount;

                var updateBalanceRes =
                    walletRepository.UpdateBalanceWallet(-transaction.Amount, senderWallet.Id,
                        senderWallet.Version); //TODO dangerous code

                if (updateBalanceRes.Status == Status.STATUS_ERROR)
                {
                    transaction.Status = Status.STATUS_ERROR;
                    return updateBalanceRes;
                }
                
                updateBalanceRes = walletRepository.UpdateBalanceWallet(transaction.Amount, receiverWallet.Id, senderWallet.Version);

                if (updateBalanceRes.Status == Status.STATUS_ERROR)
                {
                    transaction.Status = Status.STATUS_ERROR;
                    return updateBalanceRes;
                }

                transaction.Status = Status.STATUS_SUCCESS;

                return new ReturnObject()
                {
                    Status = Status.STATUS_SUCCESS
                };
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                transaction.Status = Status.STATUS_ERROR;
                return new ReturnObject()
                {
                    Status = Status.STATUS_ERROR,
                    Message = e.Message
                };
            }
        }
    }
}