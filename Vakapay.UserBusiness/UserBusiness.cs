using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
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

        private WalletBusiness.WalletBusiness _walletBusiness;

        public UserBusiness(IVakapayRepositoryFactory _vakapayRepositoryFactory, bool isNewConnection = true)
        {
            vakapayRepositoryFactory = _vakapayRepositoryFactory;

            _walletBusiness = new WalletBusiness.WalletBusiness(vakapayRepositoryFactory);
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
        public ReturnObject Login(string email, string phone, string fullName = "")
        {
            try
            {
                var userRepository = vakapayRepositoryFactory.GetUserRepository(ConnectionDb);

                var search =
                    new Dictionary<string, string>
                    {
                        {"Email", email}
                    };

                var userCheck = userRepository.FindWhere(userRepository.QuerySearch(search));

                if (userCheck == null)
                {
                    //login first
                    var newUser = new User
                    {
                        Id = CommonHelper.GenerateUuid(),
                        Email = email,
                        Phone = phone,
                        Fullname = fullName,
                        Status = Status.StatusActive,
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
                    var resultCreatWallet = _walletBusiness.MakeAllWalletForNewUser(newUser);

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
    }
}