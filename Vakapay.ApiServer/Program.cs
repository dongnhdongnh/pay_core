using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace Vakapay.ApiServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseUrls("https://0.0.0.0:5001")
                .UseStartup<Startup>()
                .Build();
    }
}