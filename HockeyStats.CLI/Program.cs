using System;
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
            app.UseFileServer(o =>
            {
                o.FileSystem = new PhysicalFileSystem(".\\www");
            });

            app.UseWelcomePage();
        }
    }
}
