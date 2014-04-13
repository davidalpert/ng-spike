using System;
using Microsoft.Owin.Diagnostics;
using Microsoft.Owin.StaticFiles;
using Owin;

namespace HockeyStats.CLI.Extensions
{
    public static class FluentOwinMiddlewareExtensions
    {
        public static void UseWelcomePage(this IAppBuilder app, Action<WelcomePageOptions> configure)
        {
            var options = new WelcomePageOptions();
            configure(options);
            app.UseWelcomePage(options);
        }

        public static void UseFileServer(this IAppBuilder app, Action<FileServerOptions> configure)
        {
            var options = new FileServerOptions();
            configure(options);
            app.UseFileServer(options);
        }
    }
}