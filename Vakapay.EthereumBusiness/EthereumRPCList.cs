using System;
using System.Collections.Generic;
using System.Text;
using Vakapay.Models.Entities.ETH;
namespace Vakapay.EthereumBussiness
{

	public class EthereumRPCList
	{
		static Dictionary<RPCName, EthRPCJson.Sender> _datas;
		static Dictionary<RPCName, EthRPCJson.Sender> Datas
		{
			get
			{
				if (_datas == null)
				{
					_datas = new Dictionary<RPCName, EthRPCJson.Sender>();
					_datas[RPCName.eth_accounts] = new EthRPCJson.Sender("1", "eth_accounts");
					_datas[RPCName.eth_getBlockByNumber] = new EthRPCJson.Sender("1", "eth_getBlockByNumber");
				}
				return _datas;
			}
		}
		public enum RPCName
		{
			eth_accounts,
			eth_getBlockByNumber
		}
		//private readonly EthRPCJson.Sender Eth_accounts()
		//	{

		//	}

		public static EthRPCJson.Sender GetSender(RPCName rpcName)
		{
			return Datas[rpcName];
		}
	}
}
