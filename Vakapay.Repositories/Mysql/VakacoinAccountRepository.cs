using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Vakapay.Models.Domains;
using Vakapay.Models.Entities;
using Vakapay.Models.Repositories;
using Vakapay.Repositories.Mysql.Base;

namespace Vakapay.Repositories.Mysql
{
    public class VakacoinAccountRepository : MysqlBaseConnection, IVakacoinAccountRepository
    {
        public VakacoinAccountRepository(string connectionString) : base(connectionString)
        {
        }

        public VakacoinAccountRepository(IDbConnection dbConnection) : base(dbConnection)
        {
        }

        public ReturnObject Update(VakacoinAccount objectUpdate)
        {
            throw new System.NotImplementedException();
        }

        public ReturnObject Delete(string Id)
        {
            throw new System.NotImplementedException();
        }

        public ReturnObject Insert(VakacoinAccount objectInsert)
        {
            try
            {
                if (Connection.State != ConnectionState.Open)
                    Connection.Open();
                var result = Connection.InsertTask<string, VakacoinAccount>(objectInsert);
                var status = !String.IsNullOrEmpty(result) ? Status.StatusSuccess : Status.StatusError;
                return new ReturnObject
                {
                    Status = status,
                    Message = status == Status.StatusError ? "Cannot insert" : "Insert Success"
                };
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public VakacoinAccount FindById(string Id)
        {
            throw new System.NotImplementedException();
        }

        public List<VakacoinAccount> FindBySql(string sqlString)
        {
            throw new System.NotImplementedException();
        }

        public VakacoinAccount FindByAddress(string address)
        {
            throw new NotImplementedException();
        }

        public Task<ReturnObject> InsertAddress(string address, string walletId, string other)
        {
            throw new NotImplementedException();
        }
    }
}