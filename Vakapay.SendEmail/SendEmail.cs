using System;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Text;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Vakapay.Models.Domains;
using Vakapay.Models.Entities;

namespace Vakapay.SendEmail
{
    public class SendEmail
    {
        public ReturnObject send(EmailQueue model)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("Configs.json");

            IConfiguration configuration = builder.Build();

            var values = new NameValueCollection
            {
                {"apikey", configuration.GetSection("Elastic:api").Value},
                {"from", configuration.GetSection("Elastic:email").Value},
                {"fromName", "VakaxaPay"},
                {"to", model.ToEmail},
                {"subject", model.Subject},
                {"bodyHtml", model.Content},
                {"isTransactional", "true"}
            };

            var address = "https://api.elasticemail.com/v2/email/send";

            using (var client = new WebClient())
            {
                try
                {
                    byte[] apiResponse = client.UploadValues(address, values);

                    var result = JsonConvert.DeserializeObject<JObject>(Encoding.UTF8.GetString(apiResponse));

                    var status = (bool) result["success"] ? Status.StatusSuccess : Status.StatusError;

                    return new ReturnObject
                    {
                        Status = status,
                        Data = Encoding.UTF8.GetString(apiResponse)
                    };
                }
                catch (Exception ex)
                {
                    return new ReturnObject
                    {
                        Status = Status.StatusError,
                        Data = ex.Message
                    };
                }
            }
        }
    }
}