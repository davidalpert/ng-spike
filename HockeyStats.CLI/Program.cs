using System;
using Microsoft.Owin.Hosting;

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
}
