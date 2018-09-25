using System.Data;
using Vakapay.Models.Domains;
using Vakapay.Models.Entities;
using Vakapay.Models.Repositories;

namespace Vakapay.SendMailBusiness
{
    public class SendMailBusiness
    {
        private readonly IVakapayRepositoryFactory _vakapayRepositoryFactory;

        private readonly IDbConnection _connectionDb;
        
        public SendMailBusiness(IVakapayRepositoryFactory vakapayRepositoryFactory, bool isNewConnection = true)
        {
            _vakapayRepositoryFactory = vakapayRepositoryFactory;
            _connectionDb = isNewConnection
                ? vakapayRepositoryFactory.GetDbConnection()
                : vakapayRepositoryFactory.GetOldConnection();
        }

        public ReturnObject CreateEmailQueue(EmailQueue emailQueue)
        {
            return null;
        }
        
        
    }
}