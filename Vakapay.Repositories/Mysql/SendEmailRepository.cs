using System.Data;
using Vakapay.Models.Entities;
using Vakapay.Models.Repositories;
using Vakapay.Repositories.Mysql.Base;

namespace Vakapay.Repositories.Mysql
{
    public class SendEmailRepository : MySqlBaseRepository<EmailQueue>, ISendEmailRepository
    {
        public SendEmailRepository(string connectionString) : base(connectionString)
        {
        }

        public SendEmailRepository(IDbConnection dbConnection) : base(dbConnection)
        {
        }
    }
}