using System;
using NUnit.Framework;
using Vakapay.Commons.Helpers;
using Vakapay.Models.Domains;
using Vakapay.Models.Entities;
using Vakapay.Models.Repositories;
using Vakapay.Repositories.Mysql;

namespace Vakapay.UnitTest
{
    using VakacoinBusiness;
    [TestFixture]
    public class VakacoinBusinessTest
    {
        private VakacoinBusiness _vb;
        const string ConnectionString = "server=localhost;userid=root;password=Concuacang123!;database=vakapay;port=3306;Connection Timeout=120;SslMode=none";
 
        [SetUp]
        public void Setup()
        {
            var repositoryConfig = new RepositoryConfiguration
            {
                ConnectionString = VakacoinBusinessTest.ConnectionString// "server=127.0.0.1;userid=root;password=Concuacang123!;database=vakapay;port=3306;Connection Timeout=120;SslMode=none"
            };

            var PersistenceFactory = new VakapayRepositoryMysqlPersistenceFactory(repositoryConfig);
            _vb = new VakacoinBusiness(PersistenceFactory);
        }
 
        [Test]
        public void OnePlusOneEqualTwo()
        {
//            Assert.AreEqual(2, _vb.Add(1, 1));
        }
 
        [Test]
        public void TwoPlusTwoEqualFour()
        {
//            Assert.AreEqual(4, _vb.Add(2, 2));
        }
 
        [Test]
        public void AddAccount()
        {
            _vb.AddAccount(CommonHelper.GenerateUuid(),"useraaaaaaaa", "5JtUScZK2XEp3g9gh7F8bwtPTRAkASmNrrftmx4AxDKD5K4zDnr", "VAKA69X3383RzBZj41k73CSjUNXM5MYGpnDxyPnWUKPEtYQmTBWz4D");
            _vb.AddAccount(CommonHelper.GenerateUuid(),"useraaaaaaab", "5JUNYmkJ5wVmtVY8x9A1KKzYe9UWLZ4Fq1hzGZxfwfzJB8jkw6u", "VAKA7yBtksm8Kkg85r4in4uCbfN77uRwe82apM8jjbhFVDgEgz3w8S");
            _vb.AddAccount(CommonHelper.GenerateUuid(),"useraaaaaaac", "5K6LU8aVpBq9vJsnpCvaHCcyYwzPPKXfDdyefYyAMMs3Qy42fUr", "VAKA7WnhaKwHpbSidYuh2DF1qAExTRUtPEdZCaZqt75cKcixuQUtdA");
            _vb.AddAccount(CommonHelper.GenerateUuid(),"useraaaaaaad", "5KdRpt1juJfbPEryZsQYxyNxSTkXTdqEiL4Yx9cAjdgApt4ANce", "VAKA7Bn1YDeZ18w2N9DU4KAJxZDt6hk3L7eUwFRAc1hb5bp6xJwxNV");
            _vb.AddAccount(CommonHelper.GenerateUuid(),"useraaaaaaae", "5JRMbcnc35NkvxKTZUnoe3W4ENQCjhMUFwjN5jQmAqN9D7N6y3N", "VAKA6cNcTC6WTFkKV4C8DoxcTXdDTDKvj3vgZEVDGVFckK1eTNJQtf");
            _vb.AddAccount(CommonHelper.GenerateUuid(),"useraaaaaaaf", "5HqyipkJSm5fwYhbhGC3vmmoBwabtgJSPecnvmN2mMrCTQfWBSS", "VAKA8UkmsnCo4GxDihbKwgoZY6f2QLSMEqBZ2frGLckxrCHrz15r7X");
            _vb.AddAccount(CommonHelper.GenerateUuid(),"useraaaaaaag", "5KPr55J2UQNUh3xP5Q6ebqqV6MK5usrXxG4qqRfpaLieGa8VpCm", "VAKA8Smcv2eMoFcp1EQSBxcAeuBowSS9xesuHjhvTnK4AACjRycTVA");
            _vb.AddAccount(CommonHelper.GenerateUuid(),"useraaaaaaah", "5JV9UNEpPKa4sqxSxvGWYPY9ZBTzAttyq7ShPvLUJSetwAeSXFW", "VAKA57VTWSiPyx45cSWGdGNtAZnmpqMrAvASQmL9hmXnoLNrgadwf7");
            _vb.AddAccount(CommonHelper.GenerateUuid(),"useraaaaaaai", "5K4GSGP2r1Yu3RqmPZPF8Hv6Zrv2YWsUEoCqwwHxKsZavz2tChg", "VAKA5dt9CWCKM1scrWpFsRbzY71Up9UYFmJs1ySFKLJDGdYJmgEH3f");
            _vb.AddAccount(CommonHelper.GenerateUuid(),"useraaaaaaaj", "5K4MmsY7Th8DqjEY2vbM7npaxSQ56XzvNULkJeqKmbYoVRmPPpB", "VAKA8FdMPpPxpG5QAqGLncY5kBrEQ9NXPKCKnLH6oWDMPR8q8BrEmT");
            _vb.AddAccount(CommonHelper.GenerateUuid(),"useraaaaaaak", "5K4d3ck3e36DoLDQDAqE2uHE6X831RYS8Ac5Hdir4CmT7WbvQJB", "VAKA6iwndPo58Y2ihWshfhnFbEBJHGkZtujR1bn7bVLngnTWFA8Hm3");
            _vb.AddAccount(CommonHelper.GenerateUuid(),"useraaaaaaal", "5KWg3urAyLF2tt1Rz8ckuK7QSiKU1CvXKhhzBPfFQcT83vjyCD2", "VAKA6QBgrm2h5f9B2RxLVXeD3HrchTUgJLtuYWPDQvi5T73enWgvVC");
//            Assert.AreEqual(result.Status, Status.StatusSuccess);
        }
        
        [TestCase(1000)]
        public void FakePeningTransactionBackandForth(int numOfTrans)
        {
            var _trans = new VakacoinWithdrawTransaction()
            {
                FromAddress = "useraaaaaaaa",
                ToAddress   = "useraaaaaaab",
                Amount = (decimal) 0.0001
            };
            ReturnObject outPut = null;
            for (int i = 0; i < numOfTrans; i++)
            {
                if (i % 2 == 0)
                {
                    _vb.FakePendingTransaction(new VakacoinWithdrawTransaction()
                    {
                        FromAddress = "useraaaaaaaa",
                        ToAddress   = "useraaaaaaab",
                        Amount = (decimal) 0.0001
                    });
                }
                else
                {
                    _vb.FakePendingTransaction(new VakacoinWithdrawTransaction()
                    {
                        FromAddress = "useraaaaaaab",
                        ToAddress   = "useraaaaaaaa",
                        Amount = (decimal) 0.0001
                    });
                }
            }

            Console.WriteLine(JsonHelper.SerializeObject(outPut));
        }
        
        [TestCase(1000)]
        public void FakePeningTransactionOneway(int numOfTrans)
        {
            var _trans = new VakacoinWithdrawTransaction()
            {
                FromAddress = "useraaaaaaaa",
                ToAddress   = "useraaaaaaab",
                Amount = (decimal) 0.0001
            };
            ReturnObject outPut = null;
            for (int i = 0; i < numOfTrans; i++)
            {
                outPut = _vb.FakePendingTransaction(_trans);
                Assert.AreEqual(outPut.Status, Status.StatusSuccess);
            }

            Console.WriteLine(JsonHelper.SerializeObject(outPut));
        }
    }
}