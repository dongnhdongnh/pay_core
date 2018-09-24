using System;
using System.Data;
using System.Threading.Tasks;
using NLog;
using Vakapay.BlockchainBusiness;
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