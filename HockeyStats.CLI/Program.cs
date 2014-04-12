using System;
using System.IO;
using System.Web.Http;
using Microsoft.Owin.FileSystems;
using Microsoft.Owin.Hosting;
using Owin;

namespace HockeyStats.CLI
{
    class Program
    {
        static void Main(string[] args)
        {
            var uri = new Uri("http://localhost:8080/");

            Console.WriteLine("Configuring Katana...");
            Console.WriteLine();
            using (WebApp.Start<Startup>(uri.ToString()))
            {
                Console.WriteLine();
                Console.WriteLine("Starting up.");
                Console.WriteLine();
                Console.WriteLine("Katana listening on: {0}", uri);
                Console.WriteLine();
                Console.WriteLine("Press Ctrl-C to quit.");
                Console.ReadKey();
                Console.WriteLine();
                Console.WriteLine("Shutting down.");
            }
        }
    }

    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureStaticFiles(app);

            ConfigureWebAPI(app);

            app.UseWelcomePage();
        }

        private static void ConfigureStaticFiles(IAppBuilder app)
        {
#if DEBUG
            var wwwDir = new DirectoryInfo(@"..\..\www");
#else
            var wwwDir = new DirectoryInto(@".\www");
#endif
            Console.WriteLine("Serving files from: {0}", wwwDir.FullName);
            app.UseFileServer(o => { o.FileSystem = new PhysicalFileSystem(wwwDir.FullName); });
        }

        private static void ConfigureWebAPI(IAppBuilder app)
        {
            var config = new HttpConfiguration();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate:"api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
                );

            app.UseWebApi(config);
        }
    }
}
