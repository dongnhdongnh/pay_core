using System.Collections.Generic;
using System.Data;
using Vakapay.Models.Domains;
using Vakapay.Models.Entities;
using Vakapay.Models.Repositories;
using Vakapay.Repositories.Mysql.Base;

namespace Vakapay.Repositories.Mysql
{
    public class EthereumAddressRepository : MysqlBaseConnection<EthereumAddressRepository>, IEthereumAddressRepository
    {
        public EthereumAddressRepository(string connectionString) : base(connectionString)
        {
        }

        public EthereumAddressRepository(IDbConnection dbConnection) : base(dbConnection)
        {
        }

        public ReturnObject Update(EthereumAddress objectUpdate)
        {
            throw new System.NotImplementedException();
        }

        public ReturnObject Delete(string Id)
        {
            throw new System.NotImplementedException();
        }

        public ReturnObject Insert(EthereumAddress objectInsert)
        {
            throw new System.NotImplementedException();
        }

        public EthereumAddress FindById(string Id)
        {
            throw new System.NotImplementedException();
        }

        public List<EthereumAddress> FindBySql(string sqlString)
        {
            throw new System.NotImplementedException();
        }
    }
}