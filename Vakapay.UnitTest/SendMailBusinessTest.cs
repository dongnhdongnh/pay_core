using NUnit.Framework;
using Vakapay.Commons.Helpers;
using Vakapay.Models.Domains;
using Vakapay.Models.Entities;
using Vakapay.Models.Repositories;
using Vakapay.Repositories.Mysql;

namespace Vakapay.UnitTest
{
    [TestFixture]
    public class SendMailBusinessTest
    {
        private SendMailBusiness.SendMailBusiness _mailBusiness;
        [SetUp]
        public void Setup()
        {
            var repsitoryConfig = new RepositoryConfiguration
            {
                ConnectionString = "server=localhost;userid=root;password=Abcd@1234;database=vakapay;port=3306;Connection Timeout=120;SslMode=none"
            };
            
            VakapayRepositoryMysqlPersistenceFactory persistenceFactory = new VakapayRepositoryMysqlPersistenceFactory(repsitoryConfig);
            _mailBusiness = new SendMailBusiness.SendMailBusiness(persistenceFactory);
        }

        [Test]
        public void CreateEmailTest()
        {
            Assert.AreEqual(Status.StatusSuccess, _mailBusiness.CreateEmailQueueAsync(new EmailQueue
            {
                Id = CommonHelper.GenerateUuid(),
                ToEmail = "doantoansai1992@gmail.com",
                Subject = "Unit test subject",
                Status = Status.StatusPending,
                CreatedAt = CommonHelper.GetUnixTimestamp(),
                UpdatedAt = CommonHelper.GetUnixTimestamp(),
                Template = EmailTemplate.RECEIVED,
                SignInUrl = "google.com.vn",
                Amount = 1,
//                SentOrReceived = EmailConfig.SentOrReceived_Sent,
                NetworkName = "EOS"
            }).Result.Status);
        }
        
        // Cannot find template because they are deployed in other location
//        [TestCase(Status.StatusSuccess, "71592f66-e47f-4261-830f-8b2779221ad4", "support@jdcoin.com", "Vakapay", "https://api.elasticemail.com/v2/email/send")]
//        public void SendMailTest(string status, string apikey, string from, string fromName, string apiAddress)
//        {
//            Assert.AreEqual(status, _mailBusiness.SendEmailAsync(apikey, from, fromName, apiAddress).Result.Status);
//        }
    }
}