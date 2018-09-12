using System;
using System.Collections.Generic;
using System.Text;
using Vakapay.Commons.Helpers;

namespace Vakapay.Models.Entities.ETH
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
				Console.WriteLine("==getter " + input);
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
	}
}
