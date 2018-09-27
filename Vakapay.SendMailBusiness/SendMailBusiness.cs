using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NLog;
using Vakapay.Commons.Helpers;
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
                sendEmailRepository.Insert(emailQueue);

                return new ReturnObject
                {
                    Status = Status.StatusSuccess,
                    Message = "Email was inserted to EmailQueue!"
                };
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

        public virtual async Task<ReturnObject> SendEmailAsync(string apikey, string from, string fromName, string apiAddress)
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

        public async Task<ReturnObject> SendEmail(EmailQueue emailQueue, string apikey, string from, string fromName, string apiAddress)
        {
            var values = new NameValueCollection
            {
                {"apikey", apikey},
                {"from", from},
                {"fromName", fromName},
                {"to", emailQueue.ToEmail},
                {"subject", emailQueue.Subject},
                {"bodyHtml", emailQueue.Content},
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
    }
}