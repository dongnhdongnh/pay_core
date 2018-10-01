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

        private string CreateEmailBody(EmailQueue emailQueue)
        {
            try
            {
                string body = string.Empty;
                string directory = Directory.GetParent(Directory.GetCurrentDirectory()) + "/MailTemplate/"+emailQueue.Template+".htm";
                
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
                    case Constants.TEMPLATE_EMAIL_NEW_DEVICE:
                        body = body.Replace("{location}", emailQueue.DeviceLocation);
                        body = body.Replace("{ip}", emailQueue.DeviceIP);
                        body = body.Replace("{browser}", emailQueue.DeviceBrowser);
                        body = body.Replace("{authorizeUrl}", emailQueue.DeviceAuthorizeUrl);
                        break;
                        
                    case Constants.TEMPLATE_EMAIL_SENT:
                        body = body.Replace("{signInUrl}", emailQueue.SignInUrl);
                        body = body.Replace("{networkName}", emailQueue.NetworkName);
                        body = body.Replace("{amount}", emailQueue.Amount + " " + emailQueue.NetworkName);
                        body = body.Replace("{sentOrReceived}", emailQueue.SentOrReceived);
                        body = body.Replace("{toOrFrom}", emailQueue.SentOrReceived == "sent" ? "to" : "from");
                        break;
                        
                    case Constants.TEMPLATE_EMAIL_VERIFY:
                        body = body.Replace("{verifyEmailUrl}", emailQueue.VerifyUrl);
                        break;
                }

                switch (emailQueue.NetworkName)
                {
                    case NetworkName.VAKA:
                        body = body.Replace("{numberOfConfirmation}", EmailConfig.VakaConfirmations.ToString());
                        break;
                    case NetworkName.ETH:
                        body = body.Replace("{numberOfConfirmation}", EmailConfig.EthConfirmations.ToString());
                        break;
                    case NetworkName.BTC:
                        body = body.Replace("{numberOfConfirmation}", EmailConfig.BtcConfirmations.ToString());
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