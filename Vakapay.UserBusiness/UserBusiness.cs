using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NLog;
using StackExchange.Redis;
using Vakapay.Commons.Constants;
using Vakapay.Commons.Helpers;
using Vakapay.Models.Domains;
using Vakapay.Models.Entities;
using Vakapay.Models.Repositories;

namespace Vakapay.UserBusiness
{
    public class UserBusiness
    {
        private readonly IVakapayRepositoryFactory vakapayRepositoryFactory;

        private readonly IDbConnection ConnectionDb;

        private static Logger logger = LogManager.GetCurrentClassLogger();

        //private WalletBusiness.WalletBusiness _walletBusiness;

        public UserBusiness(IVakapayRepositoryFactory _vakapayRepositoryFactory, bool isNewConnection = true)
        {
            vakapayRepositoryFactory = _vakapayRepositoryFactory;

            //_walletBusiness = new WalletBusiness.WalletBusiness(vakapayRepositoryFactory);
            ConnectionDb = isNewConnection
                ? vakapayRepositoryFactory.GetDbConnection()
                : vakapayRepositoryFactory.GetOldConnection();
        }

        /// <summary>
        /// save action log user
        /// </summary>
        /// <param name="idUser"></param>
        /// <param name="offset"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        public ReturnObject GetActionLog(string idUser, int offset, int limit)
        {
            try
            {
                var userRepository = vakapayRepositoryFactory.GetUserRepository(ConnectionDb);
                var userCheck = userRepository.FindById(idUser);
                if (userCheck == null)
                {
                    return new ReturnObject
                    {
                        Status = Status.STATUS_ERROR,
                        Message = "Can't User"
                    };
                }

                var logRepository = vakapayRepositoryFactory.GetUserActionLogRepository(ConnectionDb);

                var search =
                    new Dictionary<string, string>
                    {
                        {"UserId", idUser}
                    };

                var resultGetLog = logRepository.GetListLog(logRepository.QuerySearch(search), offset, limit);

                return new ReturnObject
                {
                    Status = Status.STATUS_SUCCESS,
                    Data = JsonHelper.SerializeObject(resultGetLog)
                };
            }
            catch (Exception e)
            {
                return new ReturnObject
                {
                    Status = Status.STATUS_ERROR,
                    Message = e.Message
                };
            }
        }

        /// <summary>
        /// get action log user
        /// </summary>
        /// <param name="description"></param>
        /// <param name="idUser"></param>
        /// <param name="actionLog"></param>
        /// <param name="ip"></param>
        /// <returns></returns>
        public ReturnObject AddActionLog(string description, string idUser, string actionLog, string ip,
            string source = "web")
        {
            try
            {
                //get location for ip
                var location =
                    IpGeographicalLocation.QueryGeographicalLocationAsync(ip);

                var log = new UserActionLog
                {
                    ActionName = actionLog,
                    Description = description,
                    Ip = ip,
                    UserId = idUser,
                    Location = !string.IsNullOrEmpty(location.Result.CountryName)
                        ? location.Result.City + "," + location.Result.CountryName
                        : "localhost",
                    Id = CommonHelper.GenerateUuid(),
                    Source = source,
                    CreatedAt = (int) CommonHelper.GetUnixTimestamp()
                };

                var userRepository = vakapayRepositoryFactory.GetUserRepository(ConnectionDb);
                var userCheck = userRepository.FindById(log.UserId);
                if (userCheck == null)
                {
                    return new ReturnObject
                    {
                        Status = Status.STATUS_ERROR,
                        Message = "Can't User"
                    };
                }

                var logRepository = vakapayRepositoryFactory.GetUserActionLogRepository(ConnectionDb);

                return logRepository.Insert(log);
            }
            catch (Exception e)
            {
                return new ReturnObject
                {
                    Status = Status.STATUS_ERROR,
                    Message = e.Message
                };
            }
        }

        /// <summary>
        /// Get Api key
        /// </summary>
        /// <param name="idUser"></param>
        /// <param name="offset"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        public ReturnObject GetApiKeys(string idUser, int offset = 0, int limit = 5)
        {
            try
            {
                var userRepository = vakapayRepositoryFactory.GetUserRepository(ConnectionDb);
                var userCheck = userRepository.FindById(idUser);
                if (userCheck == null)
                {
                    return new ReturnObject
                    {
                        Status = Status.STATUS_ERROR,
                        Message = "Can't User"
                    };
                }

                var apiRepository = vakapayRepositoryFactory.GetApiKeyRepository(ConnectionDb);

                var search =
                    new Dictionary<string, string>
                    {
                        {"UserId", idUser}
                    };

                var resultGetLog = apiRepository.GetListApiKey(apiRepository.QuerySearch(search), offset, limit);

                return new ReturnObject
                {
                    Status = Status.STATUS_SUCCESS,
                    Data = JsonHelper.SerializeObject(resultGetLog)
                };
            }
            catch (Exception e)
            {
                return new ReturnObject
                {
                    Status = Status.STATUS_ERROR,
                    Message = e.Message
                };
            }
        }

        /// <summary>
        /// Get Api key
        /// </summary>
        /// <param name="apiKey"></param>
        /// <returns></returns>
        public BlockchainTransaction GetWithdraw(string id, string currency)
        {
            try
            {
                BlockchainTransaction output = null;
                switch (currency)
                {
                    case CryptoCurrency.ETH:
                        var ethereumRepo =
                            vakapayRepositoryFactory.GetEthereumWithdrawTransactionRepository(ConnectionDb);
                        output = ethereumRepo.FindById(id);

                        break;
                    case CryptoCurrency.VAKA:
                        var vakaRepo =
                            vakapayRepositoryFactory.GetVakacoinWithdrawTransactionRepository(ConnectionDb);
                        output = vakaRepo.FindById(id);
                        break;
                    case CryptoCurrency.BTC:
                        var bitcoinRepo =
                            vakapayRepositoryFactory.GetBitcoinWithdrawTransactionRepository(ConnectionDb);
                        output = bitcoinRepo.FindById(id);
                        break;
                    default:
                        break;
                }

                return output;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        /// <summary>
        /// Get Api key
        /// </summary>
        /// <param name="apiKey"></param>
        /// <returns></returns>
        public ApiKey GetApiKeyByKey(string apiKey)
        {
            try
            {
                var apiRepository = vakapayRepositoryFactory.GetApiKeyRepository(ConnectionDb);

                var search =
                    new Dictionary<string, string>
                    {
                        {"KeyApi", apiKey}
                    };

                return apiRepository.FindWhere(apiRepository.QuerySearch(search));
            }
            catch (Exception e)
            {
                return null;
            }
        }


        //save apikey
        public ReturnObject SaveApiKey(ApiKey model)
        {
            try
            {
                var userRepository = vakapayRepositoryFactory.GetUserRepository(ConnectionDb);
                var userCheck = userRepository.FindById(model.UserId);
                var apirRepository = vakapayRepositoryFactory.GetApiKeyRepository(ConnectionDb);
                if (userCheck == null)
                {
                    return new ReturnObject
                    {
                        Status = Status.STATUS_ERROR,
                        Message = "Can't User"
                    };
                }

                if (string.IsNullOrEmpty(model.Id))
                {
                    model.Id = CommonHelper.GenerateUuid();
                    model.KeyApi = CommonHelper.RandomString(16);
                    model.Secret = CommonHelper.RandomString(32);
                    model.CreatedAt = (int) CommonHelper.GetUnixTimestamp();
                    model.UpdatedAt = (int) CommonHelper.GetUnixTimestamp();
                    var resultAdd = apirRepository.Insert(model);

                    return new ReturnObject
                    {
                        Status = resultAdd.Status,
                        Data = JsonHelper.SerializeObject(model),
                        Message = resultAdd.Message
                    };
                }
                else
                {
                    model.UpdatedAt = (int) CommonHelper.GetUnixTimestamp();
                    var resultEdit = apirRepository.Insert(model);

                    return new ReturnObject
                    {
                        Status = resultEdit.Status,
                        Data = JsonHelper.SerializeObject(model),
                        Message = resultEdit.Message
                    };
                }
            }
            catch (Exception e)
            {
                return null;
            }
        }

        /// <summary>
        /// save web session
        /// </summary>
        /// <param name="confirmedDevices"></param>
        /// <returns></returns>
        public ReturnObject SaveConfirmedDevices(ConfirmedDevices confirmedDevices)
        {
            try
            {
                var userRepository = vakapayRepositoryFactory.GetUserRepository(ConnectionDb);
                var userCheck = userRepository.FindById(confirmedDevices.UserId);
                if (userCheck == null)
                {
                    return new ReturnObject
                    {
                        Status = Status.STATUS_ERROR,
                        Message = "Can't User"
                    };
                }

                var logRepository = vakapayRepositoryFactory.GetConfirmedDevicesRepository(ConnectionDb);

                confirmedDevices.Id = CommonHelper.GenerateUuid();
                confirmedDevices.SignedIn = (int) CommonHelper.GetUnixTimestamp();
                return logRepository.Insert(confirmedDevices);
            }
            catch (Exception e)
            {
                return new ReturnObject
                {
                    Status = Status.STATUS_ERROR,
                    Message = e.Message
                };
            }
        }

        /// <summary>
        /// save user info
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public ReturnObject UpdateProfile(User user)
        {
            try
            {
                var userRepository = vakapayRepositoryFactory.GetUserRepository(ConnectionDb);

                var userCheck = userRepository.FindById(user.Id);
                if (userCheck == null)
                {
                    return new ReturnObject
                    {
                        Status = Status.STATUS_ERROR,
                        Message = "Can't User"
                    };
                }

                return userRepository.Update(user);
            }
            catch (Exception e)
            {
                return new ReturnObject
                {
                    Status = Status.STATUS_ERROR,
                    Message = e.Message
                };
            }
        }

        /// <summary>
        /// created User and wallet when login first
        /// </summary>
        /// <param name="userModel"></param>
        /// <returns></returns>
        public ReturnObject Login(User userModel)
        {
            try
            {
                var userRepository = vakapayRepositoryFactory.GetUserRepository(ConnectionDb);

                var search =
                    new Dictionary<string, string>
                    {
                        {"Email", userModel.Email}
                    };

                var userCheck = userRepository.FindWhere(userRepository.QuerySearch(search));
                var time = (int) CommonHelper.GetUnixTimestamp();

                if (userCheck == null)
                {
                    //login first
                    try
                    {
                        userModel.Id = CommonHelper.GenerateUuid();
                        userModel.Status = Status.STATUS_ACTIVE;
                        userModel.FullName = userModel.FirstName + " " + userModel.LastName;
                        userModel.CreatedAt = time;
                        userModel.UpdatedAt = time;
                        //created new user
                        var resultCreatedUser = userRepository.Insert(userModel);

                        if (resultCreatedUser.Status == Status.STATUS_ERROR)
                            return new ReturnObject
                            {
                                Status = Status.STATUS_ERROR,
                                Message = "Fail insert to userRepository"
                            };


                        return new ReturnObject
                        {
                            Status = Status.STATUS_SUCCESS,
                            Data = JsonHelper.SerializeObject(userModel),
                        };
                    }
                    catch (Exception e)
                    {
                        return new ReturnObject
                        {
                            Status = Status.STATUS_ERROR,
                        };
                    }
                }
                else
                {
                    //Update data user
                    userCheck.FullName = userModel.FullName;
                    userCheck.PhoneNumber = userModel.PhoneNumber;
                    userCheck.Birthday = userModel.Birthday;
                    userCheck.UpdatedAt = time;
                    //updated user
                    var resultUpdatedUser = userRepository.Update(userCheck);

                    if (resultUpdatedUser.Status == Status.STATUS_ERROR)
                        return new ReturnObject
                        {
                            Status = Status.STATUS_ERROR,
                            Message = "Fail update to userRepository"
                        };
                }

                return new ReturnObject
                {
                    Status = Status.STATUS_SUCCESS,
                    Data = JsonHelper.SerializeObject(userCheck)
                };
            }
            catch (Exception e)
            {
                return new ReturnObject
                {
                    Status = Status.STATUS_ERROR,
                    Message = e.Message
                };
            }
        }


        // Send code when do action
        public ReturnObject SendSms(User user, string code)
        {
            try
            {
                var sendSmsRepository = new SendSmsBusiness.SendSmsBusiness(vakapayRepositoryFactory, false);

                var newSms = new SmsQueue
                {
                    Id = CommonHelper.GenerateUuid(),
                    Status = Status.STATUS_PENDING,
                    To = user.PhoneNumber,
                    CreatedAt = (int) CommonHelper.GetUnixTimestamp(),
                    TextSend = "VaKaXaPay security code is: " + code,
                };

                var resultSms = sendSmsRepository.CreateSmsQueueAsync(newSms);

                if (resultSms.Status == Status.STATUS_ERROR)
                {
                    return new ReturnObject
                    {
                        Status = Status.STATUS_ERROR,
                        Message = "Fail insert to sendSms " + resultSms.Message
                    };
                }


                return new ReturnObject
                {
                    Status = Status.STATUS_SUCCESS,
                    Message = "Success"
                };
            }
            catch (Exception e)
            {
                return new ReturnObject
                {
                    Status = Status.STATUS_ERROR,
                    Message = e.Message
                };
            }
        }

        // find UserInfo by id
        public User GetUserById(string id)
        {
            try
            {
                var userRepository = vakapayRepositoryFactory.GetUserRepository(ConnectionDb);

                var user = userRepository.FindById(id);
                return user;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        // find UserInfo 
        // new Dictionary<string, string>
        //{
        //    {"Email", email}
        //};
        public User GetUserInfo(Dictionary<string, string> search)
        {
            try
            {
                var userRepository = vakapayRepositoryFactory.GetUserRepository(ConnectionDb);
                var user = userRepository.FindWhere(userRepository.QuerySearch(search));
                return user;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return null;
            }
        }

        // find ConfirmedDevices
        // new Dictionary<string, string>
        //{
        //    {"Ip", ip}
        //};
        public ConfirmedDevices GetConfirmedDevices(Dictionary<string, string> search)
        {
            try
            {
                var confirmedDevicesRepository = vakapayRepositoryFactory.GetConfirmedDevicesRepository(ConnectionDb);
                var confirmedDevices =
                    confirmedDevicesRepository.FindWhere(confirmedDevicesRepository.QuerySearch(search));
                return confirmedDevices;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return null;
            }
        }

        public ReturnObject GetListConfirmedDevices(string idUser, int offset, int limit,
            ConfirmedDevices checkConfirmedDevices)
        {
            try
            {
                var confirmedDevicesRepository = vakapayRepositoryFactory.GetConfirmedDevicesRepository(ConnectionDb);

                var search =
                    new Dictionary<string, string>
                    {
                        {"UserId", idUser}
                    };

                var resultGetLog =
                    confirmedDevicesRepository.GetListConfirmedDevices(confirmedDevicesRepository.QuerySearch(search),
                        offset, limit);


                if (!string.IsNullOrEmpty(checkConfirmedDevices.Id))
                {
                    foreach (var log in resultGetLog)
                    {
                        if (log.Id.Equals(checkConfirmedDevices.Id))
                            log.Current = 1;
                    }
                }

                return new ReturnObject
                {
                    Status = Status.STATUS_SUCCESS,
                    Data = JsonConvert.SerializeObject(resultGetLog)
                };
            }
            catch (Exception e)
            {
                return new ReturnObject
                {
                    Status = Status.STATUS_ERROR,
                    Message = e.Message
                };
            }
        }

        /// <summary>
        /// DeleteConfirmedDevicesById
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ReturnObject DeleteConfirmedDevicesById(string id)
        {
            try
            {
                var confirmedDevicesRepository = vakapayRepositoryFactory.GetConfirmedDevicesRepository(ConnectionDb);
                var resultObject = confirmedDevicesRepository.Delete(id);
                return resultObject;
            }
            catch (Exception e)
            {
                return new ReturnObject
                {
                    Status = Status.STATUS_ERROR,
                    Message = e.Message
                };
            }
        }

        /// <summary>
        /// DeleteActivityById
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ReturnObject DeleteActivityById(string id)
        {
            try
            {
                var logRepository = vakapayRepositoryFactory.GetUserActionLogRepository(ConnectionDb);
                var resultObject = logRepository.Delete(id);
                return resultObject;
            }
            catch (Exception e)
            {
                return new ReturnObject
                {
                    Status = Status.STATUS_ERROR,
                    Message = e.Message
                };
            }
        }
    }
}