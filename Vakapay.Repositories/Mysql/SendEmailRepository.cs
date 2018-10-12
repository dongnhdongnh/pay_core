using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using NLog;
using Vakapay.Commons.Constants;
using Vakapay.Commons.Helpers;
using Vakapay.Models.Domains;
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

        public EmailQueue FindPendingEmail()
        {
            return FindEmailByStatus(Status.STATUS_PENDING);
        }
        
        private EmailQueue FindEmailByStatus(string status)
        {
            try
            {
                if (Connection.State != ConnectionState.Open)
                    Connection.Open();
                //Console.WriteLine("FIND TRANSACTION BY STATUS");
                var sqlString = $"Select * from {TableName} where Status = @status and InProcess = 0";
                var result = Connection.QueryFirstOrDefault<EmailQueue>(sqlString, new {status = status});
                return result;
            }
            catch (Exception e)
            {
                LogManager.GetCurrentClassLogger().Error(e);
                return null;
            }
        }
        
        public async Task<ReturnObject> LockForProcess(EmailQueue email)
        {
            //Console.WriteLine("LockForProcess");
            var _setQuery = new Dictionary<string, string>();
            _setQuery.Add(nameof(email.Version), (email.Version + 1).ToString());
            _setQuery.Add(nameof(email.InProcess), "1");
            var _updateQuery = new Dictionary<string, string>();
            _updateQuery.Add(nameof(email.Id), email.Id);
            _updateQuery.Add(nameof(email.Version), email.Version.ToString());
            _updateQuery.Add(nameof(email.InProcess), "0");
            return ExcuteSQL(SqlHelper.Query_Update(TableName, _setQuery, _updateQuery));
        }

        public async Task<ReturnObject> SafeUpdate(EmailQueue email)
        {
            var _setQuery = new Dictionary<string, string>();
            _setQuery.Add(nameof(email.Version), (email.Version + 1).ToString());
            _setQuery.Add(nameof(email.InProcess), "0");
            _setQuery.Add(nameof(email.Status), email.Status);
            _setQuery.Add(nameof(email.UpdatedAt), email.UpdatedAt.ToString());
            
            var _updateQuery = new Dictionary<string, string>();
            _updateQuery.Add(nameof(email.Id), email.Id);
            _updateQuery.Add(nameof(email.Version), email.Version.ToString());
            _updateQuery.Add(nameof(email.InProcess), "1");
            return ExcuteSQL(SqlHelper.Query_Update(TableName, _setQuery, _updateQuery));
        }

        public async Task<ReturnObject> ReleaseLock(EmailQueue email)
        {
            var _setQuery = new Dictionary<string, string>();
            _setQuery.Add(nameof(email.Version), (email.Version + 1).ToString());
            _setQuery.Add(nameof(email.InProcess), "0");
            var _updateQuery = new Dictionary<string, string>();
            _updateQuery.Add(nameof(email.Id), email.Id);
            _updateQuery.Add(nameof(email.Version), email.Version.ToString());
            _updateQuery.Add(nameof(email.InProcess), "1");
            return ExcuteSQL(SqlHelper.Query_Update(TableName, _setQuery, _updateQuery));
        }
    }
}