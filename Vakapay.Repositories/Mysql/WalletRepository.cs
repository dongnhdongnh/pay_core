using System;
using System.Collections.Generic;
using System.Data;
using System.Security;
using Dapper;
using Vakapay.Models.Domains;
using Vakapay.Models.Entities;
using Vakapay.Models.Repositories;
using Vakapay.Repositories.Mysql.Base;

namespace Vakapay.Repositories.Mysql
{
    public class WalletRepository : MysqlBaseConnection ,IWalletRepository
    {
        public WalletRepository(string connectionString) : base(connectionString)
        {
        }

        public WalletRepository(IDbConnection dbConnection) : base(dbConnection)
        {
        }

        public ReturnObject Update(Wallet objectUpdate)
        {
            throw new System.NotImplementedException();
        }

        public ReturnObject Delete(string Id)
        {
            throw new System.NotImplementedException();
        }

        public ReturnObject Insert(Wallet objectInsert)
        {
            try
            {
                if(Connection.State != ConnectionState.Open)
                    Connection.Open();
                var result = Connection.InsertTask<string, Wallet>(objectInsert);
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

        public Wallet FindById(string Id)
        {
            throw new System.NotImplementedException();
        }

        public List<Wallet> FindBySql(string sqlString)
        {
            throw new System.NotImplementedException();
        }

        public ReturnObject UpdateBalanceWallet(decimal amount, string Id, int version)
        {
            throw new System.NotImplementedException();
        }
    }
}