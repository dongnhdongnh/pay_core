using Microsoft.AspNetCore.Http;

namespace Vakapay.ApiServer.Helpers
{
    public class HelpersApi
    {
        public static string getIp(HttpRequest request)
        {
            string ip = request.Headers["X-Forwarded-For"].ToString();

            if (!string.IsNullOrEmpty(ip))
                ip = request.Headers["X-Real-IP"].ToString();

            return ip;
        }
    }
}