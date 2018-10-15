using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NLog;
using Vakapay.BlockchainBusiness;
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
        /// save action log user
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
                    IPGeographicalLocation.QueryGeographicalLocationAsync(ip);

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

        /*// Send code when do action, chua dung den
        public ReturnObject SendCode(User user, string action)
        {
            try
            {
                var userTokenRepository = vakapayRepositoryFactory.GetUserTokenRepository(ConnectionDb);
                var sendSmsRepository = vakapayRepositoryFactory.GetSendSmsRepository(ConnectionDb);

                var search =
                    new Dictionary<string, string>
                    {
                        {"UserId", user.Id},
                        {"Action", action},
                        {"status", Status.StatusPending}
                    };

                var tokenCheck = userTokenRepository.FindWhere(userTokenRepository.QuerySearch(search));

                if (tokenCheck != null)
                {
                    return new ReturnObject
                    {
                        Status = Status.StatusSuccess,
                        Message = "Success"
                    };
                }

                var transactionScope = ConnectionDb.BeginTransaction();
                try
                {
                    //save token
                    var newToken = new UserToken
                    {
                        Id = CommonHelper.GenerateUuid(),
                        Status = Status.StatusPending,
                        Action = action,
                        CreatedAt = (int) CommonHelper.GetUnixTimestamp(),
                        UserId = user.Id,
                        Token = CommonHelper.RandomNumber(6)
                    };

                    var resultAdd = userTokenRepository.Insert(newToken);

                    if (resultAdd.Status == Status.StatusError)
                    {
                        transactionScope.Rollback();
                        return new ReturnObject
                        {
                            Status = Status.StatusError,
                            Message = "Fail insert to userTokenRepository"
                        };
                    }

                    var newSms = new SmsQueue
                    {
                        Id = CommonHelper.GenerateUuid(),
                        Status = Status.StatusPending,
                        To = user.PhoneNumber,
                        CreatedAt = (int) CommonHelper.GetUnixTimestamp(),
                        TextSend = "Vakaxa security code is: " + newToken.Token,
                    };

                    var resultSms = sendSmsRepository.Insert(newSms);

                    if (resultSms.Status == Status.StatusError)
                    {
                        transactionScope.Rollback();
                        return new ReturnObject
                        {
                            Status = Status.StatusError,
                            Message = "Fail insert to sendSms " + resultSms.Message
                        };
                    }
                }
                catch (Exception e)
                {
                    transactionScope.Rollback();
                    return new ReturnObject
                    {
                        Status = Status.StatusError,
                        Message = e.Message
                    };
                }

                transactionScope.Commit();
                return new ReturnObject
                {
                    Status = Status.StatusSuccess,
                    Message = "Success"
                };
            }
            catch (Exception e)
            {
                return new ReturnObject
                {
                    Status = Status.StatusError,
                    Message = e.Message
                };
            }
        }*/

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
                Console.WriteLine(e.ToString());
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