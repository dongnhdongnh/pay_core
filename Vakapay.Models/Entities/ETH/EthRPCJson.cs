using System;

namespace Vakapay.Models.Entities
{
	public class EthRPCJson
	{
		public class Sender
		{
			public Sender()
			{ }
			public Sender(string id, string method)
			{
				this.jsonrpc = "2.0";
				this.id = id;
				this.method = method;
			}
			public string jsonrpc;
			public string method;
			public Object[] param;
			public string id;
			public string GetJSon()
			{
				string output = JsonHelper.SerializeObject(this);
				output = output.Replace("param", "params");
				return output;
			}
		}

		public class Getter
		{
			public Getter(string input)
			{
				if (input == null)
					return;
				Getter thing = JsonHelper.DeserializeObject<Getter>(input);
				this.id = thing.id;
				this.jsonrpc = thing.jsonrpc;
				this.result = thing.result;
			}
			public string id;
			public string jsonrpc;
			public Object result;
		}

		public class TransactionInfor
		{
			public string from;
			public string to;
			public string value;
			public string hash;
			public string blockHash;
			public string blockNumber;
			public string transactionIndex;
			public string gas;
			public string gasPrice;
			public string input;
			public string nonce;
		}
	}
}
