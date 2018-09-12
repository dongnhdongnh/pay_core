using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VakacoinCore.Response.Table;
using VakacoinCore.Lib;

namespace VakacoinCore
{
    public class TableAPI : BaseAPI
    {
        public TableAPI() {}

        public TableAPI(string host) : base(host) {}

        public async Task<List<GlobalRow>> GetGlobalRowsAsync()
        {
            return await GetTableRowsAsync<GlobalRow>();
        }
        public List<GlobalRow> GetGlobalRows()
        {
            return GetGlobalRowsAsync().Result;
        }
        public async Task<List<NameBidsRow>> GetNameBidRowsAsync()
        {
            return await GetTableRowsAsync<NameBidsRow>();
        }
        public List<NameBidsRow> GetNameBidRows()
        {
            return GetNameBidRowsAsync().Result;
        }
        public async Task<List<ProducerRow>> GetProducerRowsAsync()
        {
            return await GetTableRowsAsync<ProducerRow>();
        }
        public List<ProducerRow> GetProducerRows()
        {
            return GetProducerRowsAsync().Result;
        }
        public async Task<List<VoterRow>> GetVoterRowsAsync()
        {
            return await GetTableRowsAsync<VoterRow>();
        }
        public List<VoterRow> GetVoterRows()
        {
            return GetVoterRowsAsync().Result;
        }
        public async Task<List<T>> GetTableRowsAsync<T>() where T : IVAKATable
        {
            return await new VAKA_Table<T>(HOST).GetRowsFromAPIAsync();
        }
        public List<T> GetTableRows<T>() where T : IVAKATable
        {
            return GetTableRowsAsync<T>().Result;
        }
    }
}
