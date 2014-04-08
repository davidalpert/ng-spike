using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Owin.Diagnostics;
using Microsoft.Owin.FileSystems;
using Microsoft.Owin.Hosting;
using Microsoft.Owin.StaticFiles;
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

            app.UseWelcomePage();
        }

        private static void ConfigureStaticFiles(IAppBuilder app)
        {
            app.UseFileServer(o =>
            {
                o.FileSystem = new PhysicalFileSystem(".\\www");
                o.EnableDefaultFiles = true;
#if DEBUG
                o.EnableDirectoryBrowsing = true;
#endif
            });
        }
    }
}
