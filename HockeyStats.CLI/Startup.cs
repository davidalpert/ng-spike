using System;
using System.IO;
using System.Web.Http;
using HockeyStats.CLI.Extensions;
using Microsoft.Owin.FileSystems;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Owin;

namespace HockeyStats.CLI
{
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

            ConfigureWebApiJsonSerializer(config);

            app.UseWebApi(config);
        }

        private static void ConfigureWebApiJsonSerializer(HttpConfiguration config)
        {
            // doesn't seem to work?
            var jsonSerializerSettings = config.Formatters.JsonFormatter.SerializerSettings;
            jsonSerializerSettings.Formatting = Formatting.Indented;
            jsonSerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
        }
    }
}