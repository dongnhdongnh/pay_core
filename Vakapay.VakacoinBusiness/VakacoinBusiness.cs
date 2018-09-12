using System;
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
        public ReturnObject CreateNewAccount()
        {
            try
            {

                
                return new ReturnObject { };
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