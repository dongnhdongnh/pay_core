using System;
using System.Collections.Generic;
using System.Data;
using Dapper;
using Vakapay.Models.Domains;
using Vakapay.Models.Entities;
using Vakapay.Models.Repositories;
using Vakapay.Repositories.Mysql.Base;

namespace Vakapay.Repositories.Mysql
{
    public class BitcoinAddressRepository : MysqlBaseConnection<BitcoinAddressRepository>, IBitcoinAddressRepository
    {
        public BitcoinAddressRepository(string connectionString) : base(connectionString)
        {
        }

        public BitcoinAddressRepository(IDbConnection dbConnection) : base(dbConnection)
        {
        }

        public ReturnObject Update(BitcoinAddress objectUpdate)
        {
            throw new System.NotImplementedException();
        }

        public ReturnObject Delete(string Id)
        {
            throw new System.NotImplementedException();
        }

        public ReturnObject Insert(BitcoinAddress objectInsert)
        {
            try
            {
                if (Connection.State != ConnectionState.Open)
                    Connection.Open();
                var result = Connection.InsertTask<string, BitcoinAddress>(objectInsert);
                var status = !String.IsNullOrEmpty(result) ? Status.StatusSuccess : Status.StatusError;
                return new ReturnObject
                {
                    Status = status,
                    Message = status == Status.StatusError ? "Cannot insert" : "Insert Success",
                    Data = result
                };
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public BitcoinAddress FindById(string Id)
        {
            throw new System.NotImplementedException();
        }

        public List<BitcoinAddress> FindBySql(string sqlString)
        {
            throw new System.NotImplementedException();
        }
    }
}