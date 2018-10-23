using Dapper;
using System;
using System.Data;
using System.Threading.Tasks;
using Vakapay.Models.Domains;
using Vakapay.Models.Entities;
using Vakapay.Models.Repositories;
using Vakapay.Repositories.Mysql.Base;

namespace Vakapay.Repositories.Mysql
{
    public class VakacoinAccountRepository : BlockchainAddressRepository<VakacoinAccount>, IVakacoinAccountRepository
    {
        public VakacoinAccountRepository(string connectionString) : base(connectionString)
        {
        }

        public VakacoinAccountRepository(IDbConnection dbConnection) : base(dbConnection)
        {
        }

        public override Task<ReturnObject> InsertAddress(string address, string walletId, string other)
        {
            throw new NotImplementedException();
        }
    }
}