using System.Collections.Generic;
using System.Data;
using Vakapay.Models.Domains;
using Vakapay.Models.Entities;
using Vakapay.Models.Repositories;
using Vakapay.Repositories.Mysql.Base;

namespace Vakapay.Repositories.Mysql
{
    public class UserRepository :MysqlBaseConnection<UserRepository>, IUserRepository
    {
        public UserRepository(string connectionString) : base(connectionString)
        {
        }

        public UserRepository(IDbConnection dbConnection) : base(dbConnection)
        {
        }

        public ReturnObject Update(User objectUpdate)
        {
            throw new System.NotImplementedException();
        }

        public ReturnObject Delete(string Id)
        {
            throw new System.NotImplementedException();
        }

        public ReturnObject Insert(User objectInsert)
        {
            throw new System.NotImplementedException();
        }

        public User FindById(string Id)
        {
            throw new System.NotImplementedException();
        }

        public List<User> FindBySql(string sqlString)
        {
            throw new System.NotImplementedException();
        }
    }
}