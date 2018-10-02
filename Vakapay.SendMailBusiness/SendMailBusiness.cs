﻿ using System;
using System.Collections.Specialized;
using System.Data;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NLog;
using Vakapay.Commons.Helpers;
 using Vakapay.Models;
 using Vakapay.Models.Domains;
using Vakapay.Models.Entities;
using Vakapay.Models.Repositories;
 using Vakapay.Models.Repositories.Base;
 using Vakapay.Repositories.Mysql;

namespace Vakapay.SendMailBusiness
{
    public class SendMailBusiness
    {
        private readonly IVakapayRepositoryFactory _vakapayRepositoryFactory;

        private readonly IDbConnection _connectionDb;

        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public SendMailBusiness(IVakapayRepositoryFactory vakapayRepositoryFactory, bool isNewConnection = true)
        {
            _vakapayRepositoryFactory = vakapayRepositoryFactory;
            _connectionDb = isNewConnection
                ? vakapayRepositoryFactory.GetDbConnection()
                : vakapayRepositoryFactory.GetOldConnection();
        }

        public virtual async Task<ReturnObject> CreateEmailQueueAsync(EmailQueue emailQueue)
        {
            try
            {
                var sendEmailRepository = _vakapayRepositoryFactory.GetSendEmailRepository(_connectionDb);

                // save to DB
                var result = sendEmailRepository.Insert(emailQueue);

                return result;
            }
            catch (Exception e)
            {
                _logger.Error(e);
                return new ReturnObject
                {
                    Status = Status.StatusError,
                    Message = e.ToString()
                };
            }
        }

        public virtual async Task<ReturnObject> SendEmailAsync(string apikey, string from, string fromName,
            string apiAddress)
        {
            var sendEmailRepository = _vakapayRepositoryFactory.GetSendEmailRepository(_connectionDb);
            var pendingEmail = sendEmailRepository.FindPendingEmail();

            if (pendingEmail?.Id == null)
                return new ReturnObject
                {
                    Status = Status.StatusSuccess,
                    Message = "Pending email not found"
                };

            if (_connectionDb.State != ConnectionState.Open)
                _connectionDb.Open();

            //begin first email
            var transctionScope = _connectionDb.BeginTransaction();
            try
            {
                var lockResult = await sendEmailRepository.LockForProcess(pendingEmail);
                if (lockResult.Status == Status.StatusError)
                {
                    transctionScope.Rollback();
                    return new ReturnObject
                    {
                        Status = Status.StatusSuccess,
                        Message = "Cannot Lock For Process"
                    };
                }

                transctionScope.Commit();
            }
            catch (Exception e)
            {
                transctionScope.Rollback();
                return new ReturnObject
                {
                    Status = Status.StatusError,
                    Message = e.ToString()
                };
            }

            //update Version to Model
            pendingEmail.Version += 1;

            var transactionSend = _connectionDb.BeginTransaction();
            try
            {
                var sendResult = await SendEmail(pendingEmail, apikey, from, fromName, apiAddress);
                if (sendResult.Status == Status.StatusError)
                {
                    return new ReturnObject
                    {
                        Status = Status.StatusError,
                        Message = "Cannot Send email"
                    };
                }

                pendingEmail.Status = sendResult.Status;
                pendingEmail.UpdatedAt = CommonHelper.GetUnixTimestamp();
                pendingEmail.InProcess = 0;

                var updateResult = await sendEmailRepository.SafeUpdate(pendingEmail);
                if (updateResult.Status == Status.StatusError)
                {
                    return new ReturnObject
                    {
                        Status = Status.StatusError,
                        Message = "Cannot update email status"
                    };
                }

                transactionSend.Commit();
                return updateResult;
            }
            catch (Exception e)
            {
                // release lock
                var releaseResult = sendEmailRepository.ReleaseLock(pendingEmail);
                Console.WriteLine(JsonHelper.SerializeObject(releaseResult));
                transactionSend.Rollback();
                throw;
            }
        }

        public async Task<ReturnObject> SendEmail(EmailQueue emailQueue, string apikey, string from, string fromName,
            string apiAddress)
        {
            string emailBody = CreateEmailBody(emailQueue);
            if (emailBody == null)
                return new ReturnObject
                {
                    Status = Status.StatusError,
                    Message = "Cannot find template"
                };
            var values = new NameValueCollection
            {
                {"apikey", apikey},
                {"from", from},
                {"fromName", fromName},
                {"to", emailQueue.ToEmail},
                {"subject", emailQueue.Subject},
                {"bodyHtml", emailBody},
                {"isTransactional", "true"}
            };

            var address = apiAddress;

            using (var client = new WebClient())
            {
                try
                {
                    byte[] apiResponse = client.UploadValues(address, values);

                    var result = JsonConvert.DeserializeObject<JObject>(Encoding.UTF8.GetString(apiResponse));

                    var status = (bool) result["success"] ? Status.StatusSuccess : Status.StatusError;

                    return new ReturnObject
                    {
                        Status = status,
                        Message = Encoding.UTF8.GetString(apiResponse)
                    };
                }
                catch (Exception ex)
                {
                    return new ReturnObject
                    {
                        Status = Status.StatusError,
                        Message = ex.Message
                    };
                }
            }
        }

        private BlockchainTransaction GetTransaction(EmailQueue emailQueue)
        {
            switch (emailQueue.NetworkName)
            {
                 case NetworkName.BTC:
                     switch (emailQueue.Template)
                     {
                         case EmailTemplate.SENT:
                             return _vakapayRepositoryFactory.GetBitcoinWithdrawTransactionRepository(_connectionDb)
                                 .FindById(emailQueue.TransactionId);
                         case EmailTemplate.RECEIVED:
                             return _vakapayRepositoryFactory.GetBitcoinDepositTransactionRepository(_connectionDb)
                                 .FindById(emailQueue.TransactionId);
                         case EmailTemplate.NEW_DEVICE:
                             break;
                         case EmailTemplate.VERIFY:
                             break;
                         default:
                             throw new ArgumentOutOfRangeException();
                     }
                     break;
                case NetworkName.ETH:
                    switch (emailQueue.Template)
                    {
                        case EmailTemplate.SENT:
                            return _vakapayRepositoryFactory.GetEthereumWithdrawTransactionRepository(_connectionDb)
                                .FindById(emailQueue.TransactionId);
                        case EmailTemplate.RECEIVED:
                            return _vakapayRepositoryFactory.GetEthereumDepositeTransactionRepository(_connectionDb)
                                .FindById(emailQueue.TransactionId);
                        case EmailTemplate.NEW_DEVICE:
                            break;
                        case EmailTemplate.VERIFY:
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                    break;
                case NetworkName.VAKA:
                    switch (emailQueue.Template)
                    {
                        case EmailTemplate.SENT:
                            return _vakapayRepositoryFactory.GetVakacoinWithdrawTransactionRepository(_connectionDb)
                                .FindById(emailQueue.TransactionId);
                        case EmailTemplate.RECEIVED:
                            return _vakapayRepositoryFactory.GetVakacoinDepositTransactionRepository(_connectionDb)
                                .FindById(emailQueue.TransactionId);
                        case EmailTemplate.NEW_DEVICE:
                            break;
                        case EmailTemplate.VERIFY:
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                    break;
            }

            return null;
        }
        
        private string GetReceivedAddress(EmailQueue emailQueue)
        {
            try
            {
                return GetTransaction(emailQueue).ToAddress;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        
        private string CreateEmailBody(EmailQueue emailQueue)
        {
            try
            {
                string body = string.Empty;
                string directory = Directory.GetParent(Directory.GetCurrentDirectory()) + "/MailTemplate/" +
                                   EmailConfig.TemplateFiles[emailQueue.Template];
                
                using (StreamReader reader = new StreamReader(directory))
                {
                    body = reader.ReadToEnd();
                }

                body = body.Replace("{vakapayUrl}", EmailConfig.VakapayUrl);
                body = body.Replace("{logoImgUrl}", EmailConfig.LogoImgUrl);
                body = body.Replace("{mailImgUrl}", EmailConfig.MailImgUrl);
                body = body.Replace("{checkImgUrl}", EmailConfig.CheckImgUrl);
                body = body.Replace("{hrImgUrl}", EmailConfig.HrImgUrl);
                body = body.Replace("{deviceImgUrl}", EmailConfig.DeviceImgUrl);

                switch (emailQueue.Template)
                {
                    case EmailTemplate.NEW_DEVICE:
                        body = body.Replace("{location}", emailQueue.DeviceLocation);
                        body = body.Replace("{ip}", emailQueue.DeviceIP);
                        body = body.Replace("{browser}", emailQueue.DeviceBrowser);
                        body = body.Replace("{authorizeUrl}", emailQueue.DeviceAuthorizeUrl);
                        break;
                        
                    case EmailTemplate.SENT:
                        body = body.Replace("{signInUrl}", emailQueue.SignInUrl);
                        body = body.Replace("{toAddress}", GetReceivedAddress(emailQueue));
                        body = body.Replace("{amount}", emailQueue.GetAmount());
                        break;
                    case EmailTemplate.RECEIVED:
                        body = body.Replace("{signInUrl}", emailQueue.SignInUrl);
                        body = body.Replace("{networkName}", emailQueue.NetworkName);
                        body = body.Replace("{amount}", emailQueue.GetAmount());
                        body = body.Replace("{numberOfConfirmation}",
                            EmailConfig.GetNumberOfNeededConfirmation(emailQueue.NetworkName));
                        break;
                        
                    case EmailTemplate.VERIFY:
                        body = body.Replace("{verifyEmailUrl}", emailQueue.VerifyUrl);
                        break;
                }

                return body;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }
    }
}