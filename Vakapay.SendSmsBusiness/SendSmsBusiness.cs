using System;
using System.Collections.Specialized;
using System.Data;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NLog;
using Vakapay.Commons.Constants;
using Vakapay.Commons.Helpers;
using Vakapay.Models.Domains;
using Vakapay.Models.Entities;
using Vakapay.Models.Repositories;

namespace Vakapay.SendSmsBusiness
{
    public class SendSmsBusiness
    {
        private readonly IVakapayRepositoryFactory _vakapayRepositoryFactory;

        private readonly IDbConnection _connectionDb;

        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public SendSmsBusiness(IVakapayRepositoryFactory vakapayRepositoryFactory, bool isNewConnection = true)
        {
            _vakapayRepositoryFactory = vakapayRepositoryFactory;
            _connectionDb = isNewConnection
                ? vakapayRepositoryFactory.GetDbConnection()
                : vakapayRepositoryFactory.GetOldConnection();
        }

        public ReturnObject CreateSmsQueueAsync(SmsQueue model)
        {
            try
            {
                var sendSmslRepository = _vakapayRepositoryFactory.GetSendSmsRepository(_connectionDb);

                // save to DB
                var result = sendSmslRepository.Insert(model);

                return result;
            }
            catch (Exception e)
            {
                _logger.Error(e);
                return new ReturnObject
                {
                    Status = Status.STATUS_ERROR,
                    Message = e.ToString()
                };
            }
        }

        public virtual async Task<ReturnObject> SendSmsAsync(string apiUrl, string apiKey)
        {
            var sendSmsRepository = _vakapayRepositoryFactory.GetSendSmsRepository(_connectionDb);
            var pendingSms = sendSmsRepository.FindRowPending();

            if (pendingSms?.Id == null)
                return new ReturnObject
                {
                    Status = Status.STATUS_SUCCESS,
                    Message = "Pending sms not found"
                };

            if (_connectionDb.State != ConnectionState.Open)
                _connectionDb.Open();

            //begin first sms
            var transactionScope = _connectionDb.BeginTransaction();
            try
            {
                var lockResult = await sendSmsRepository.LockForProcess(pendingSms);
                if (lockResult.Status == Status.STATUS_ERROR)
                {
                    transactionScope.Rollback();
                    return new ReturnObject
                    {
                        Status = Status.STATUS_SUCCESS,
                        Message = "Cannot Lock For Process"
                    };
                }

                transactionScope.Commit();
            }
            catch (Exception e)
            {
                transactionScope.Rollback();
                return new ReturnObject
                {
                    Status = Status.STATUS_ERROR,
                    Message = e.ToString()
                };
            }

            //update Version to Model
            pendingSms.Version += 1;

            var transactionSend = _connectionDb.BeginTransaction();
            try
            {
                var sendResult = await SendSms(pendingSms, apiUrl, apiKey);
                if (sendResult.Status == Status.STATUS_ERROR)
                {
                    return new ReturnObject
                    {
                        Status = Status.STATUS_ERROR,
                        Message = "Cannot Send sms"
                    };
                }

                pendingSms.Status = sendResult.Status;
                pendingSms.UpdatedAt = (int) CommonHelper.GetUnixTimestamp();
                pendingSms.IsProcessing = 0;

                var updateResult = await sendSmsRepository.SafeUpdate(pendingSms);
                if (updateResult.Status == Status.STATUS_ERROR)
                {
                    transactionSend.Rollback();
                    return new ReturnObject
                    {
                        Status = Status.STATUS_ERROR,
                        Message = "Cannot update sms status"
                    };
                }

                transactionSend.Commit();
                return updateResult;
            }
            catch (Exception e)
            {
                // release lock
                transactionSend.Rollback();
                var releaseResult = sendSmsRepository.ReleaseLock(pendingSms);
                Console.WriteLine(JsonHelper.SerializeObject(releaseResult));
                _logger.Error(e);
                throw;
            }
        }

        public async Task<ReturnObject> SendSms(SmsQueue model, string apiUrl, string apiKey)
        {
            var values = new NameValueCollection
            {
                {"apikey", apiKey},
                {"fromName", "Vakapay"},
                {"to", model.To},
                {"body", model.TextSend},
                {"isTransactional", "true"}
            };

            using (var client = new WebClient())
            {
                try
                {
                    byte[] apiResponse = client.UploadValues(apiUrl, values);

                    var result = JsonConvert.DeserializeObject<JObject>(Encoding.UTF8.GetString(apiResponse));

                    var status = (bool) result["success"] ? Status.STATUS_SUCCESS : Status.STATUS_ERROR;

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
                        Status = Status.STATUS_ERROR,
                        Message = ex.Message
                    };
                }
            }
        }
    }
}