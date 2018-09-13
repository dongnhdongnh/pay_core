using System;
using VakacoinCore.Params;
using VakacoinCore.Response.API;
using VakacoinCore.Serialization;
using Action = VakacoinCore.Params.Action;
namespace VakacoinCore.Utilities
{
    public class ActionUtility
    {
        private ChainAPI chainAPI;
        public ActionUtility(string host)
        {
            chainAPI = new ChainAPI(host);
        }

        public Action GetActionObject(string accountName, string actionName, string permissionName, string code, object args)
        {
            Action action;
            if (accountName == "")
            {
                action = new Action(){ 
                    account = new AccountName(code),
                    name = new ActionName(actionName),
                    authorization = new VakacoinCore.Params.Authorization[0]
                };
            }
            else
            {
                //prepare action object
                action = new Action()
                {
                    account = new AccountName(code),
                    name = new ActionName(actionName),
                    authorization = new[]
                    {
                        new VakacoinCore.Params.Authorization
                        {
                            actor = new AccountName(accountName),
                            permission = new PermissionName(permissionName)
                        }
                    }
                };
            }

            //convert action arguments to binary and save it in action.datareturn await new VAKA_Object<PushTransaction>(HOST).GetObjectsFromAPIAsync(new PushTransactionParam { packed_trx = Hex.ToString(packedTransaction), signatures = signatures, packed_context_free_data = string.Empty, compression = "none" });
            var abiJsonToBin = chainAPI.GetAbiJsonToBin(code, actionName, args);
            action.data = new BinaryString(abiJsonToBin.binargs);
            return action;
        }
    }
}
