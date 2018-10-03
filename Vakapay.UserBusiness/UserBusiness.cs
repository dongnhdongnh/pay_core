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

                if (userCheck == null)
                {
                    //login first
                    var time = (int) CommonHelper.GetUnixTimestamp();
                    var newUser = new User
                    {
                        Id = CommonHelper.GenerateUuid(),
                        Email = userModel.Email,
                        PhoneNumber = userModel.PhoneNumber,
                        FullName = userModel.FullName,
                        Birthday = userModel.Birthday,
                        Status = Status.StatusActive,
                        CreatedAt = time,
                        UpdatedAt = time
                    };


                    //created new user
                    var resultCreatedUser = userRepository.Insert(newUser);

                    if (resultCreatedUser.Status == Status.StatusError)
                        return new ReturnObject
                        {
                            Status = Status.StatusError,
                            Message = "Fail insert to userRepository"
                        };

                    // created wallet
                    var resultCreatWallet = walletBusiness.MakeAllWalletForNewUser(newUser);

                    if (resultCreatWallet.Status == Status.StatusError)
                        return new ReturnObject
                        {
                            Status = Status.StatusError,
                            Message = "Fail insert wallet"
                        };

                    return new ReturnObject
                    {
                        Status = Status.StatusSuccess,
                        Data = JsonConvert.SerializeObject(newUser)
                    };
                }
                else
                {
                    userCheck.FullName = userModel.FullName;
                    userCheck.PhoneNumber = userModel.PhoneNumber;
                    userCheck.Birthday = userModel.Birthday;
                    //updated new user
                    var resultUpdatedUser = userRepository.Update(userCheck);


                    if (resultUpdatedUser.Status == Status.StatusError)
                        return new ReturnObject
                        {
                            Status = Status.StatusError,
                            Message = "Fail update to userRepository"
                        };
                }

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

                var search =
                    new Dictionary<string, string>
                    {
                        {"Id", id}
                    };

                var user = userRepository.FindWhere(userRepository.QuerySearch(search));
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