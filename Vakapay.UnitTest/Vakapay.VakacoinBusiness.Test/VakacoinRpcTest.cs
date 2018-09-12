using NUnit.Framework;
using Vakapay.Models.Domains;

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
        
        [TestCase( Status.StatusSuccess , "useraaaaaaaa", "useraaaaaaab", "0.0001 VAKA", "unittest", "5JtUScZK2XEp3g9gh7F8bwtPTRAkASmNrrftmx4AxDKD5K4zDnr")]
        [TestCase( Status.StatusSuccess , "useraaaaaaab", "useraaaaaaaa", "0.0001 VAKA", "unittest", "5JUNYmkJ5wVmtVY8x9A1KKzYe9UWLZ4Fq1hzGZxfwfzJB8jkw6u")]
        [TestCase( Status.StatusError , "useraaaaaaaa", "useraaaaaaab", "0.0001 VAKA", "wrong private key", "5JUNYmkJ5wVmtVY8x9A1KKzYe9UWLZ4Fq1hzGZxfwfzJB8jkw6u")] // wrong private key
        [TestCase( Status.StatusError , "useraaaaaaaa", "useraaaaaaab", "0.0001 LIEM", "wrong symbol", "5JtUScZK2XEp3g9gh7F8bwtPTRAkASmNrrftmx4AxDKD5K4zDnr")] // wrong symbol
        [TestCase( Status.StatusError , "useraaaaaaaa", "useraaaaaaab", "0.001 VAKA", "wrong precision(4)", "5JtUScZK2XEp3g9gh7F8bwtPTRAkASmNrrftmx4AxDKD5K4zDnr")] // wrong precision(4)
        [TestCase( Status.StatusError , "useraaaaaaaa", "useraaaaaaab", "0.0000 VAKA", "Zero Amount", "5JtUScZK2XEp3g9gh7F8bwtPTRAkASmNrrftmx4AxDKD5K4zDnr")] // Zero amount
        [TestCase( Status.StatusError , "", "useraaaaaaab", "0.0001 VAKA", "Sender blank", "5JtUScZK2XEp3g9gh7F8bwtPTRAkASmNrrftmx4AxDKD5K4zDnr")] // Sender Blank
        [TestCase( Status.StatusError , "useraaaaaaaa", "", "0.0001 VAKA", "Receiver blank", "5JtUScZK2XEp3g9gh7F8bwtPTRAkASmNrrftmx4AxDKD5K4zDnr")] // Receiver Blank
        [TestCase( Status.StatusError , "useraaaaaaaa", "vaka.n.exist", "0.0001 VAKA", "Receiver not exist", "5JtUScZK2XEp3g9gh7F8bwtPTRAkASmNrrftmx4AxDKD5K4zDnr")] // Receiver not exist
        public void SendTransaction(string status, string from, string to, string amount, string memo, string privatekey)
        {
            Assert.AreEqual( status, _rpc.SendTransaction(from, to, amount, 
                memo, privatekey).Status);
        }        
        
        
    }
}