using System;
using Vakapay.Commons.Helpers;
using Vakapay.Models.Domains;
using Vakapay.Models.Repositories;

namespace Vakapay.VakacoinBusiness
{
    using BlockchainBusiness;
    public class VakacoinBusiness : BlockchainBusiness
    {
        private VakacoinRpc VakacoinRpc { get; set; }

        public VakacoinBusiness(IVakapayRepositoryFactory vakapayRepositoryFactory, bool isNewConnection = true)
            : base(vakapayRepositoryFactory, isNewConnection)
        {
            VakacoinRpc = new VakacoinRpc("http://127.0.0.1:8000");
        }
        
        /// <summary>
        /// call RPC Vakacoin to create a new account
        /// save account name to database
        /// </summary>
        /// <returns></returns>
        public ReturnObject CreateNewAddAddress(string walletId)
        {
            try
            {
                string accountName = "";
                do
                {
                    accountName = CommonHelper.RandomAccountNameVakacoin();
                } while ( VakacoinRpc.CheckAccountExist(accountName) == true );

                return new ReturnObject { };
            }
            catch (Exception e)
            {
                return new ReturnObject
                {
                    Status = Status.StatusError,
                    Message = e.Message
                };
                ;
            }
            
        }
        
    }
}