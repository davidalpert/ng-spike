using System;
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
            string uri = "http://localhost:8080/";

            using (WebApp.Start<Startup>(uri))
            {
                Console.WriteLine("Started; listening on: {0}", uri);
                Console.ReadKey();
                Console.WriteLine("Stopping");
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
            app.UseFileServer(o => { o.FileSystem = new PhysicalFileSystem(".\\www"); });
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
