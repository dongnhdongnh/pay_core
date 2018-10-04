using System;
using System.Collections.Generic;
using System.Data;
using Newtonsoft.Json;
using NLog;
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
        /// <param name="log"></param>
        /// <returns></returns>
        public ReturnObject AddActionLog(UserActionLog log)
        {
            try
            {
                var userRepository = vakapayRepositoryFactory.GetUserRepository(ConnectionDb);
                var userCheck = userRepository.FindById(log.UserId);
                if (userCheck == null)
                {
                    return new ReturnObject
                    {
                        Status = Status.StatusError,
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
                    Status = Status.StatusError,
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
                        Status = Status.StatusError,
                        Message = "Can't User"
                    };
                }

                return userRepository.Update(user);
            }
            catch (Exception e)
            {
                return new ReturnObject
                {
                    Status = Status.StatusError,
                    Message = e.Message
                };
            }
        }

        /// <summary>
        /// created User and wallet when login first
        /// </summary>
        /// <param name="email"></param>
        /// <param name="phone"></param>
        /// <param name="fullName"></param>
        /// <returns></returns>
        public ReturnObject Login(IWalletBusiness walletBusiness, User userModel)
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
                    userModel.Id = CommonHelper.GenerateUuid();
                    userModel.Status = Status.StatusActive;
                    userModel.CreatedAt = time;
                    userModel.UpdatedAt = time;

                    //created new user
                    var resultCreatedUser = userRepository.Insert(userModel);

                    if (resultCreatedUser.Status == Status.StatusError)
                        return new ReturnObject
                        {
                            Status = Status.StatusError,
                            Message = "Fail insert to userRepository"
                        };

                    // created wallet
                    var resultCreateWallet = walletBusiness.MakeAllWalletForNewUser(userModel);

                    if (resultCreateWallet.Status == Status.StatusError)
                        return new ReturnObject
                        {
                            Status = Status.StatusError,
                            Message = "Fail insert wallet"
                        };

                    return new ReturnObject
                    {
                        Status = Status.StatusSuccess,
                        Data = JsonConvert.SerializeObject(userModel)
                    };
                }

                //Update data user
                userCheck.FullName = userModel.FullName;
                userCheck.PhoneNumber = userModel.PhoneNumber;
                userCheck.Birthday = userModel.Birthday;
                userCheck.UpdatedAt = time;
                //updated user
                var resultUpdatedUser = userRepository.Update(userCheck);


                if (resultUpdatedUser.Status == Status.StatusError)
                    return new ReturnObject
                    {
                        Status = Status.StatusError,
                        Message = "Fail update to userRepository"
                    };

                return new ReturnObject
                {
                    Status = Status.StatusSuccess,
                    Data = JsonConvert.SerializeObject(userCheck)
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
        }


        // find UserInfo by id
        public User getUserByID(string id)
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
        public User getUserInfo(Dictionary<string, string> search)
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
    }
}