using NUnit.Framework;
using Vakapay.Commons.Constants;
using Vakapay.Commons.Helpers;
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
				ConnectionString = AppSettingHelper.GetDBConnection()
            };
            
            VakapayRepositoryMysqlPersistenceFactory persistenceFactory = new VakapayRepositoryMysqlPersistenceFactory(repsitoryConfig);
            _mailBusiness = new SendMailBusiness.SendMailBusiness(persistenceFactory);
        }

        [Test]
        public void CreateEmailTest()
        {
            Assert.AreEqual(Status.STATUS_SUCCESS, _mailBusiness.CreateEmailQueueAsync(new EmailQueue
            {
                Id = CommonHelper.GenerateUuid(),
                ToEmail = "doantoansai1992@gmail.com",
                Subject = "Unit test subject",
                Status = Status.STATUS_PENDING,
                CreatedAt = CommonHelper.GetUnixTimestamp(),
                UpdatedAt = CommonHelper.GetUnixTimestamp(),
                Template = EmailTemplate.Received,
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