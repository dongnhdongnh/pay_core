using System;
using Vakapay.Models.Domains;
using Vakapay.Models.Entities;

namespace Vakapay.SendEmail
{
    class Program
    {
        static void Main(string[] args)
        {
            //scan email EmailQueue
            var send = new SendEmail();
            var model = new EmailQueue();
            send.send(model);
        }
    }
}