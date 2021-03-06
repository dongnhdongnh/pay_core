using NUnit.Framework;
using Vakapay.Commons.Constants;
using Vakapay.VakacoinBusiness;

namespace Vakapay.UnitTest
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
        [TestCase("ajajajajajaj", false)]
        [TestCase("vaka.n.exist", false)]
        [TestCase("ERROR", false)]
        [TestCase("vaka.no.exist.account.name.too.long", false)]
        [TestCase("@@@", false)]
        public void CheckAccountExist(string xyz, bool result)
        {
            Assert.AreEqual(result, _rpc.CheckAccountExist(xyz));
        }

        [TestCase(Status.STATUS_SUCCESS, "useraaaaaaaa", "useraaaaaaab", "0.0001 VAKA", "unittest",
            "5JtUScZK2XEp3g9gh7F8bwtPTRAkASmNrrftmx4AxDKD5K4zDnr")]
        [TestCase(Status.STATUS_SUCCESS, "useraaaaaaab", "useraaaaaaaa", "0.0001 VAKA", "unittest",
            "5JUNYmkJ5wVmtVY8x9A1KKzYe9UWLZ4Fq1hzGZxfwfzJB8jkw6u")]
        [TestCase(Status.STATUS_ERROR, "useraaaaaaaa", "useraaaaaaab", "0.0001 VAKA", "wrong private key",
            "5JUNYmkJ5wVmtVY8x9A1KKzYe9UWLZ4Fq1hzGZxfwfzJB8jkw6u")] // wrong private key
        [TestCase(Status.STATUS_ERROR, "useraaaaaaaa", "useraaaaaaab", "0.0001 LIEM", "wrong symbol",
            "5JtUScZK2XEp3g9gh7F8bwtPTRAkASmNrrftmx4AxDKD5K4zDnr")] // wrong symbol
        [TestCase(Status.STATUS_ERROR, "useraaaaaaaa", "useraaaaaaab", "0.001 VAKA", "wrong precision(4)",
            "5JtUScZK2XEp3g9gh7F8bwtPTRAkASmNrrftmx4AxDKD5K4zDnr")] // wrong precision(4)
        [TestCase(Status.STATUS_ERROR, "useraaaaaaaa", "useraaaaaaab", "0.0000 VAKA", "Zero Amount",
            "5JtUScZK2XEp3g9gh7F8bwtPTRAkASmNrrftmx4AxDKD5K4zDnr")] // Zero amount
        [TestCase(Status.STATUS_ERROR, "", "useraaaaaaab", "0.0001 VAKA", "Sender blank",
            "5JtUScZK2XEp3g9gh7F8bwtPTRAkASmNrrftmx4AxDKD5K4zDnr")] // Sender Blank
        [TestCase(Status.STATUS_ERROR, "useraaaaaaaa", "", "0.0001 VAKA", "Receiver blank",
            "5JtUScZK2XEp3g9gh7F8bwtPTRAkASmNrrftmx4AxDKD5K4zDnr")] // Receiver Blank
        [TestCase(Status.STATUS_ERROR, "useraaaaaaaa", "vaka.n.exist", "0.0001 VAKA", "Receiver not exist",
            "5JtUScZK2XEp3g9gh7F8bwtPTRAkASmNrrftmx4AxDKD5K4zDnr")]
        // Receiver not exist
        public void SendTransaction(string status, string from, string to, string amount, string memo,
            string privatekey)
        {
            Assert.AreEqual(status, _rpc.SendTransaction(from, to, amount,
                memo, privatekey).Status);
        }

        [TestCase(Status.STATUS_ERROR, "useraaaaaaaa", "VAKA69X3383RzBZj41k73CSjUNXM5MYGpnDxyPnWUKPEtYQmTBWz4D")]
        [TestCase(Status.STATUS_ERROR, "useraaaaaaa9", "VAKA69X3383RzBZj41k73CSjUNXM5MYGpnDxyPnWUKPEtYQmTBWz4D")]
        [TestCase(Status.STATUS_ERROR, "userx", "VAKA69X3383RzBZj41k73CSjUNXM5MYGpnDxyPnWUKPEtYQmTBWz4D")]
        public void CreateAccount(string status, string username, string publicKey)
        {
            Assert.AreEqual(status, _rpc.CreateAccount(username, publicKey).Status);
        }

        [TestCase(Status.STATUS_SUCCESS, "VAKA69X3383RzBZj41k73CSjUNXM5MYGpnDxyPnWUKPEtYQmTBWz4D")]
        [TestCase(Status.STATUS_ERROR, "VAKA69X3383RzBZj41k73CSjUNXM5MYGpnDxyPnWUKPEtYQmTBWz4E")]
        public void CreateRandomAccount(string status, string publicKey)
        {
            Assert.AreEqual(status, _rpc.CreateRandomAccount(publicKey).Status);
        }

        [TestCase(Status.STATUS_SUCCESS, "useraaaaaeek", "100.0000 VAKA", true)]
        [TestCase(Status.STATUS_SUCCESS, "useraaaaaeek", "1.0000 VAKA", false)]
        [TestCase(Status.STATUS_ERROR, "vaka.n.exist", "10.0000 VAKA", false)]
        public void GetCurrencyBalance(string status, string username, string amount, bool equal)
        {
            if (status == Status.STATUS_SUCCESS)
            {
                Assert.AreEqual(equal, amount == _rpc.GetCurrencyBalance(username).Data);
            }
            else
            {
                Assert.AreEqual(status, _rpc.GetCurrencyBalance(username).Status);
            }
        }
    }
}