using System;
using System.Collections.Generic;
using System.Data;
using Newtonsoft.Json;
using Vakapay.Commons.Constants;
using Vakapay.Commons.Helpers;
using Vakapay.Models.Domains;
using Vakapay.Models.Entities;
using Vakapay.Models.Repositories;

namespace Vakapay.UserBusiness
{
    public class UserBusiness
    {
        private readonly IVakapayRepositoryFactory _vakapayRepositoryFactory;

        private readonly IDbConnection _connectionDb;

        //private WalletBusiness.WalletBusiness _walletBusiness;

        public UserBusiness(IVakapayRepositoryFactory vakapayRepositoryFactory, bool isNewConnection = true)
        {
            _vakapayRepositoryFactory = vakapayRepositoryFactory;

            //_walletBusiness = new WalletBusiness.WalletBusiness(vakapayRepositoryFactory);
            _connectionDb = isNewConnection
                ? _vakapayRepositoryFactory.GetDbConnection()
                : _vakapayRepositoryFactory.GetOldConnection();
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
                var userRepository = _vakapayRepositoryFactory.GetUserRepository(_connectionDb);
                var userCheck = userRepository.FindById(idUser);
                if (userCheck == null)
                {
                    return new ReturnObject
                    {
                        Status = Status.STATUS_ERROR,
                        Message = "Can't User"
                    };
                }

                var logRepository = _vakapayRepositoryFactory.GetUserActionLogRepository(_connectionDb);

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
        /// <param name="source"></param>
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
                    CreatedAt = (int)CommonHelper.GetUnixTimestamp()
                };

                var userRepository = _vakapayRepositoryFactory.GetUserRepository(_connectionDb);
                var userCheck = userRepository.FindById(log.UserId);
                if (userCheck == null)
                {
                    return new ReturnObject
                    {
                        Status = Status.STATUS_ERROR,
                        Message = "Can't User"
                    };
                }

                var logRepository = _vakapayRepositoryFactory.GetUserActionLogRepository(_connectionDb);

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
        public ReturnObject GetApiKeys(string idUser, int offset = 0, int limit = 8)
        {
            try
            {
                var userRepository = _vakapayRepositoryFactory.GetUserRepository(_connectionDb);
                var userCheck = userRepository.FindById(idUser);
                if (userCheck == null)
                {
                    return new ReturnObject
                    {
                        Status = Status.STATUS_ERROR,
                        Message = "Can't User"
                    };
                }

                var apiRepository = _vakapayRepositoryFactory.GetApiKeyRepository(_connectionDb);

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
                            _vakapayRepositoryFactory.GetEthereumWithdrawTransactionRepository(_connectionDb);
                        output = ethereumRepo.FindById(id);

                        break;
                    case CryptoCurrency.VAKA:
                        var vakaRepo =
                            _vakapayRepositoryFactory.GetVakacoinWithdrawTransactionRepository(_connectionDb);
                        output = vakaRepo.FindById(id);
                        break;
                    case CryptoCurrency.BTC:
                        var bitcoinRepo =
                            _vakapayRepositoryFactory.GetBitcoinWithdrawTransactionRepository(_connectionDb);
                        output = bitcoinRepo.FindById(id);
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
                var apiRepository = _vakapayRepositoryFactory.GetApiKeyRepository(_connectionDb);

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
                var userRepository = _vakapayRepositoryFactory.GetUserRepository(_connectionDb);
                var userCheck = userRepository.FindById(model.UserId);
                var apirRepository = _vakapayRepositoryFactory.GetApiKeyRepository(_connectionDb);
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
                    model.Status = 1;
                    model.KeyApi = CommonHelper.RandomString(16);
                    model.Secret = CommonHelper.RandomString(32);
                    model.CreatedAt = (int)CommonHelper.GetUnixTimestamp();
                    model.UpdatedAt = (int)CommonHelper.GetUnixTimestamp();
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
                    model.UpdatedAt = (int)CommonHelper.GetUnixTimestamp();
                    var resultEdit = apirRepository.Update(model);

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
                var userRepository = _vakapayRepositoryFactory.GetUserRepository(_connectionDb);
                var userCheck = userRepository.FindById(confirmedDevices.UserId);
                if (userCheck == null)
                {
                    return new ReturnObject
                    {
                        Status = Status.STATUS_ERROR,
                        Message = "Can't User"
                    };
                }

                var logRepository = _vakapayRepositoryFactory.GetConfirmedDevicesRepository(_connectionDb);

                confirmedDevices.Id = CommonHelper.GenerateUuid();
                confirmedDevices.SignedIn = (int)CommonHelper.GetUnixTimestamp();
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
                var userRepository = _vakapayRepositoryFactory.GetUserRepository(_connectionDb);

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
                var userRepository = _vakapayRepositoryFactory.GetUserRepository(_connectionDb);

                var search =
                    new Dictionary<string, string>
                    {
                        {"Email", userModel.Email}
                    };

                var userCheck = userRepository.FindWhere(userRepository.QuerySearch(search));
                var time = (int)CommonHelper.GetUnixTimestamp();

                if (userCheck == null)
                {
                    //login first
                    try
                    {
                        userModel.Id = CommonHelper.GenerateUuid();
                        userModel.Status = Status.STATUS_ACTIVE;
                        userModel.FullName = userModel.FirstName + " " + userModel.LastName;
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
                var sendSmsRepository = new SendSmsBusiness.SendSmsBusiness(_vakapayRepositoryFactory, false);

                var newSms = new SmsQueue
                {
                    Status = Status.STATUS_PENDING,
                    To = user.PhoneNumber,
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
                var userRepository = _vakapayRepositoryFactory.GetUserRepository(_connectionDb);

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
                var userRepository = _vakapayRepositoryFactory.GetUserRepository(_connectionDb);
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
                var confirmedDevicesRepository = _vakapayRepositoryFactory.GetConfirmedDevicesRepository(_connectionDb);
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
                var confirmedDevicesRepository = _vakapayRepositoryFactory.GetConfirmedDevicesRepository(_connectionDb);

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
                var confirmedDevicesRepository = _vakapayRepositoryFactory.GetConfirmedDevicesRepository(_connectionDb);
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
                var logRepository = _vakapayRepositoryFactory.GetUserActionLogRepository(_connectionDb);
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

        /// <summary>
        /// DeleteApiKeyById
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ReturnObject DeleteApikeyById(string id)
        {
            try
            {
                var apiKeyRepository = _vakapayRepositoryFactory.GetApiKeyRepository(_connectionDb);
                var resultObject = apiKeyRepository.Delete(id);
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

        // find ApiKey by id
        public ApiKey GetApiKeyById(string id)
        {
            try
            {
                var apiKeyRepository = _vakapayRepositoryFactory.GetApiKeyRepository(_connectionDb);

                var apiKey = apiKeyRepository.FindById(id);
                return apiKey;
            }
            catch (Exception e)
            {
                return null;
            }
        }
    }
}