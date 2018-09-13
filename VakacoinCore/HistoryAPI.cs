using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cryptography.ECDSA;
using VakacoinCore.Params;
using VakacoinCore.Response.API;
using VakacoinCore.Serialization;
using VakacoinCore.Utilities;
using Action = VakacoinCore.Params.Action;
using VakacoinCore.Lib;

namespace VakacoinCore
{
    public class HistoryAPI : BaseAPI
    {
        public HistoryAPI(){}

        public HistoryAPI(string host) : base(host) {}
        
        public async Task<Actions> GetActionsAsync(int pos, int offset, string accountName)
        {
            return await new VAKA_Object<Actions>(HOST).GetObjectsFromAPIAsync(new ActionsParam { pos = pos, offset = offset, account_name = accountName });
        }
        public Actions GetActions(int pos, int offset, string accountName)
        {
            return GetActionsAsync(pos, offset, accountName).Result;
        }
        public async Task<TransactionResult> GetTransactionAsync(string id, uint? blockNumHint)
        {
            return await new VAKA_Object<TransactionResult>(HOST).GetObjectsFromAPIAsync(new TransactionResultParam { id = id, block_num_hint = blockNumHint });
        }
        public TransactionResult GetTransaction(string id, uint? blockNumHint)
        {
            return GetTransactionAsync(id, blockNumHint).Result;
        }
    }
}
