using NUnit.Framework;

namespace Vakapay.VakacoinBusiness.Test
{
    [TestFixture]
    public class VakacoinRpcTest
    {
        private VakacoinRpc _rpc;
        
        [SetUp]
        public void Setup()
        {
            _rpc = new VakacoinRpc("http://127.0.0.1:8000");
        }
        
        [TestCase("vaka", true)]
        [TestCase("vaka.n.exist", false)]
        [TestCase("vaka.n.exist.AccountNameTooLong", false)]
        [TestCase("vaka.no.exist.account.name.too.long", false)]
        [TestCase("@@@", false)]
        public void CheckAccountExist(string xyz, bool result)
        {
            Assert.AreEqual(result, _rpc.CheckAccountExist(xyz));
        }
    }
}