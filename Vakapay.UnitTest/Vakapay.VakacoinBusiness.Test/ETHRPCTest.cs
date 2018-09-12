using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Vakapay.EthereumBusiness;

namespace Vakapay.VakacoinBusiness.Test
{

	[TestFixture]
	class ETHRPCTest
	{

		EthereumRpc _etheRpc;
		[SetUp]
		public void Setup()
		{
			_etheRpc = new EthereumRpc("HELOO");
		}

		[TestCase(0)]
		public void GetBlock(int id)
		{
			var output = _etheRpc.FindBlockByNumber(id);
			//Trace.Write(JsonHelper.SerializeObject(output));
			//	Console.WriteLine(JsonHelper.SerializeObject(output));
			Assert.IsNotNull(output);
		}

		[Test]
		public void SendTransaction()
		{
			var output = _etheRpc.SendTransactionWithPassphrase("0x12890d2cce102216644c59dae5baed380d84830c", "0x3a2e25cfb83d633c184f6e4de1066552c5bf4517", 10, "password");
			//var output = _etheRpc.SendTransaction("0x12890d2cce102216644c59dae5baed380d84830c", "0x3a2e25cfb83d633c184f6e4de1066552c5bf4517", 10);
			//Trace.Write(JsonHelper.SerializeObject(output));
			//	Console.WriteLine(JsonHelper.SerializeObject(output));
			Assert.IsNotNull(output);
		}
	}
}
