using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace TestApp
{
    using Bonsai.Extensions.DependencyInjection;

    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .ConfigureServices(services => services.AddBonsai())
                .UseStartup<Startup>();
    }
}