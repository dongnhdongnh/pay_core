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

	}
}
