using System;
using NLog;
using Vakapay.BlockchainBusiness.Base;
using Vakapay.Commons.Helpers;
using Vakapay.Models.Domains;
using Vakapay.Models.Entities;
using Vakapay.Models.Repositories;

namespace Vakapay.BitcoinBusiness
{
    public class BitcoinBusinessNew : AbsBlockchainBusiness
    {
        public BitcoinBusinessNew(IVakapayRepositoryFactory vakapayRepositoryFactory, bool isNewConnection = true) :
            base(vakapayRepositoryFactory, isNewConnection)
        {
            // <summary>
           
        }

       
    }
}