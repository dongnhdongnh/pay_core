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
    public class SendSmsRepository : MySqlBaseRepository<SmsQueue>, ISendSmsRepository
    {
        public SendSmsRepository(string connectionString) : base(connectionString)
        {
        }

        public SendSmsRepository(IDbConnection dbConnection) : base(dbConnection)
        {
        }

        public SmsQueue FindPendingSms()
        {
            return FindSmsByStatus(Status.STATUS_PENDING);
        }

        private SmsQueue FindSmsByStatus(string status)
        {
            try
            {
                if (Connection.State != ConnectionState.Open)
                    Connection.Open();

                var sqlString = $"Select * from {TableName} where Status = @status and InProcess = 0";
                var result = Connection.QueryFirstOrDefault<SmsQueue>(sqlString, new {status = status});
                return result;
            }
            catch (Exception e)
            {
                LogManager.GetCurrentClassLogger().Error(e);
                return null;
            }
        }

        public async Task<ReturnObject> LockForProcess(SmsQueue sms)
        {
            var _setQuery = new Dictionary<string, string>();
            _setQuery.Add(nameof(sms.Version), (sms.Version + 1).ToString());
            _setQuery.Add(nameof(sms.InProcess), "1");
            var _updateQuery = new Dictionary<string, string>();
            _updateQuery.Add(nameof(sms.Id), sms.Id);
            _updateQuery.Add(nameof(sms.Version), sms.Version.ToString());
            _updateQuery.Add(nameof(sms.InProcess), "0");
            return ExcuteSQL(SqlHelper.Query_Update(TableName, _setQuery, _updateQuery));
        }

        public async Task<ReturnObject> SafeUpdate(SmsQueue sms)
        {
            var _setQuery = new Dictionary<string, string>();
            _setQuery.Add(nameof(sms.Version), (sms.Version + 1).ToString());
            _setQuery.Add(nameof(sms.InProcess), "0");
            _setQuery.Add(nameof(sms.Status), sms.Status);
            _setQuery.Add(nameof(sms.UpdatedAt), sms.UpdatedAt.ToString());

            var _updateQuery = new Dictionary<string, string>();
            _updateQuery.Add(nameof(sms.Id), sms.Id);
            _updateQuery.Add(nameof(sms.Version), sms.Version.ToString());
            _updateQuery.Add(nameof(sms.InProcess), "1");
            return ExcuteSQL(SqlHelper.Query_Update(TableName, _setQuery, _updateQuery));
        }

        public async Task<ReturnObject> ReleaseLock(SmsQueue sms)
        {
            var _setQuery = new Dictionary<string, string>();
            _setQuery.Add(nameof(sms.Version), (sms.Version + 1).ToString());
            _setQuery.Add(nameof(sms.InProcess), "0");
            var _updateQuery = new Dictionary<string, string>();
            _updateQuery.Add(nameof(sms.Id), sms.Id);
            _updateQuery.Add(nameof(sms.Version), sms.Version.ToString());
            _updateQuery.Add(nameof(sms.InProcess), "1");
            return ExcuteSQL(SqlHelper.Query_Update(TableName, _setQuery, _updateQuery));
        }
    }
}