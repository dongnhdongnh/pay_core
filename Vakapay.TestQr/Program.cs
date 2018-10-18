using System;
using Vakapay.Commons.Helpers;

namespace Vakapay.TestQr
{
    class Program
    {
        static void Main(string[] args)
        {
            var google = new GoogleAuthen.TwoFactorAuthenticator();
            //var result = google.GenerateSetupCode("test1", "1234567890asdasd", 300, 300);
            //Console.WriteLine(JsonHelper.SerializeObject(result));
            var valid = google.ValidateTwoFactorPIN("1234567890asdasd", "627833");
            Console.WriteLine(valid);

        }
    }
}